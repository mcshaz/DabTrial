;(function ($) {
    //functions from unobtrusive:
    function setValidationValues(options, ruleName, value) {
        options.rules[ruleName] = value;
        if (options.message) {
            options.messages[ruleName] = options.message;
        }
    }
    function setMultiValidationValues(options, ruleName, values) {
        var i = 0, thisRule;
        for (; i < values.length; i++) {
            thisRule = (i == 0) ? ruleName : ruleName + i;
            options.messages[thisRule] = values[i].message;
            delete values[i].message;
            options.rules[thisRule] = values[i];
            if (ruleName !== thisRule) {
                (function addValidatorMethod() {
                    var counter = 0;
                    if (!$.validator.methods[ruleName]) {
                        if (++counter > 10) { throw new ReferenceError(ruleName + " is not defined"); }
                        setTimeout(addValidatorMethod, 100);
                        return;
                    }
                    if (!$.validator.methods[thisRule]) { $.validator.addMethod(thisRule, $.validator.methods[ruleName]); }
                })();
            }
        }
    }
    function transformValidationValues(options) {
        var rules = $.parseJSON(options.message),
            propNames = [], p, utilObj,i = 0,j, returnVar=[];
        for (p in options.params) {
            if (options.params.hasOwnProperty(p)) {
                utilObj = {};
                utilObj.key = p;
                utilObj.vals = $.parseJSON(options.params[p]);
                propNames.push(utilObj);
            }
        }
        for (; i < rules.length; i++) {
            utilObj = {};
            utilObj.message = rules[i];
            for (j=0; j < propNames.length; j++) {
                utilObj[propNames[j].key] = propNames[j].vals[i];
            }
            returnVar.push(utilObj);
        }
        return returnVar;
    }

    function splitAndTrim(value) {
        return value.replace(/^\s+|\s+$/g, "").split(/\s*,\s*/g);
    }

    var formEls;
    function getUniqueFormId(form) {
        if (typeof (formEls === 'undefined')) {
            formEls = document.getElementsByTagName('form');
        }
        return 'form' + $.inArray(form, formEls);
        //return 'form' + Array.prototype.indexOf.call(formEls, form);
    }

    function getModelPrefix(fieldName) {
        return fieldName.substr(0, fieldName.lastIndexOf(".") + 1);
    }

    function appendModelPrefix(value, prefix) {
        if (value.indexOf("*.") === 0) {
            value = value.replace("*.", prefix);
        }
        return value;
    }
    //adapted from validate
    function isCheckbox(element) {
        return /checkbox/i.test(element.type);
    }
    function findByName(name, form) {
        // select by name and filter by form for performance over form.find("[name=...]")
        return $(document.getElementsByName(name)).map(function (index, element) {
            return element.form == form && element.name == name && element || null;
        });
    }
    //------------------------
    //dateHandling
//    if (isNaN(Date.parse("23/2/2012 13:12"))) {
        //taken from http://ajax.aspnetcdn.com/ajax/jquery.validate/1.11.1/additional-methods.js
        //modified to allow time
        jQuery.validator.addMethod("dateITA", function (value, element) {
            var check = false,
                match = /^(\d{1,2})\/(\d{1,2})\/(\d{4})( ?(\d{1,2}):(\d{1,2})(:(\d{1,2}))?(:(\d{1,}))?)?( ?([aApP])\.?[mM]\.?)?$/.exec(value),
                dd, MM, yyyy, aaaa, hh, mm, ss, SS, xdata, suf;
            if (match && match.length > 4) {
                dd = parseInt(match[1], 10);
                MM = parseInt(match[2], 10);
                yyyy = parseInt(match[3], 10);
                hh = parseInt(match[5] || 0, 10);
                suf = (match[12] || '').toLowerCase();
                if (hh === 12 && suf === 'a') {
                    hh -= 12;
                } else if (hh!==12 && suf === 'p') {
                    hh+=12
                };
                mm = parseInt(match[6] || 0, 10);
                ss = parseInt(match[8] || 0, 10);
                SS = parseInt(match[10] || 0, 10);
                xdata = new Date(yyyy, MM-1, dd,hh,mm,ss,SS);
                check = ((xdata.getFullYear() === yyyy) && (xdata.getMonth() === MM - 1) && (xdata.getDate() === dd)
                    && (xdata.getHours() === hh) && (xdata.getMinutes() === mm) && (xdata.getSeconds() === ss) && (xdata.getMilliseconds() === SS));
            }
            return this.optional(element) || check;
        }, "Please enter a correct date");
        $.validator.methods.date = $.validator.methods.dateITA;
//    };
    //------------------------
    $.validator.unobtrusive.adapters.addBool("mustbetrue", "required");
    //----------------------
    $.validator.addMethod("isvalidregex", function (value, element) {
        //return this.optional(element)
        try { var clientRegex = new RegExp(value); }
        catch (ex) { return false; }
        return true;
    });
    $.validator.unobtrusive.adapters.addBool("isvalidregex");
    //------------------------
    function fileinput(element, validationFunc)
    {
        var isValid = "pending",
            i=0,
            totalLoops = (element.files || []).length;
        while (i<totalLoops && (isValid!==false)) {
            isValid = validationFunc.call(element, element.files[i]);
            i += 1;
        }
        return isValid;
    };
    //------------------------
    $.validator.addMethod("maxfilesize", function (value, element, maxSize) {
        return fileinput(element, function(f){
            return f.size < maxSize;
        })
    });
    $.validator.unobtrusive.adapters.add("maxfilesize", ["size"], function (options) {
        setValidationValues(options, "maxfilesize", parseInt(options.params.size, 10));
    });
    //------------------------
    $.validator.addMethod("filetypes", function (value, element, acceptedTypes) {
        var getFileType = /\.[^.]+$/;
        return fileinput(element, function (f) {
            var filetype = getFileType.exec(f.name);
            return filetype && $.inArray(filetype[0].substr(1), acceptedTypes) > -1;
        })
    });
    $.validator.unobtrusive.adapters.add("filetypes", ["types"], function (options) {
        var acceptedTypes = options.params.types.split(",");
        options.message = options.message.replace("{0}", acceptedTypes.join(", "));
        setValidationValues(options, "filetypes", acceptedTypes);
    });
    //------------------------
    $.validator.addMethod("datetonow", function (value, element, option) {
        //return this.optional(element)
        var dateVal = parseDate($(element)),
            now = new Date();
        if (isNaN(dateVal)) { return "pending"; }
        if (option === "before") { return dateVal < now; }
        return dateVal > now;
    });
    $.validator.unobtrusive.adapters.addSingleVal("datetonow", "opt");
    //------------------------
    $.validator.addMethod("regexcount", function (value, element, params) {
        var matches = (value.match(params.regex)||[]).length
        return  matches >= params.min && matches <= params.max;
    });
    $.validator.unobtrusive.adapters.add("regexcount", ["min", "max", "regex", "regexopt"], function (options) {
        var args = transformValidationValues(options), i=0;
        for (; i < args.length; i++) {
            args[i].regex = new RegExp(args[i].regex, args[i].regexopt);
            delete args[i].regexopt;
        }
        setMultiValidationValues(options, "regexcount", args);
    });
    //------------------------
    $.validator.addMethod("comesafter", function (value, element, otherDate) {
        var otherDate = parseDate(otherDate),
            thisDate = parseDate(element);
        if (!thisDate || !otherDate) { return "pending"; }
        return thisDate >= otherDate;
    });
    $.validator.addMethod("comesbefore", function (value, element, otherDate) {
        var otherDate = parseDate(otherDate),
            thisDate = parseDate(element);
        if (!thisDate || !otherDate) { return "pending"; }
        return thisDate <= otherDate;
    });
    $.validator.unobtrusive.adapters.add("comesbefore", ["otherdateprop", "comparisondate"], function (options) {
        if (options.params.otherdateprop) {
            var prefix = getModelPrefix(options.element.name),
                fullOtherName = appendModelPrefix(options.params.otherdateprop, prefix),
                $otherdate = $(options.form).find(":input[name='" + fullOtherName + "']:first"); //could use findByName, but need to extend input tag
            setValidationValues(options, "comesbefore", $otherdate);
        } else {
            setValidationValues(options, "comesbefore", options.params.comparisondate);
        }
    });
    $.validator.unobtrusive.adapters.add("comesafter", ["otherdateprop", "comparisondate"], function (options) {
        if (options.params.otherdateprop) {
            var prefix = getModelPrefix(options.element.name),
                fullOtherName = appendModelPrefix(options.params.otherdateprop, prefix),
                $otherdate = $(options.form).find(":input[name='" + fullOtherName + "']:first"); //could use findByName, but need to extend input tag
            setValidationValues(options, "comesafter", $otherdate);
        } else {
            setValidationValues(options, "comesafter", options.params.comparisondate);
        }
    });
    //------------------------
    $.validator.addMethod("dateinterval", function (value, element, params) {
        var otherDate = params.$otherdate ? parseDate(params.$otherdate) : new Date(),
            thisDate = parseDate($(element));
        if (!otherDate || isNaN(otherDate) || !thisDate || isNaN(thisDate)) { return "pending"; }
        interval = dateDifference(thisDate, otherDate, params.timeunit);
        return (interval >= params.mininterval && interval <= params.maxinterval);
    });
    $.validator.unobtrusive.adapters.add("dateinterval", ["otherdateprop", "min", "max", "unit"], function (options) {
        var other = options.params.otherdateprop,
            params = { mininterval: parseInt(options.params.min, 10),
                maxinterval: parseInt(options.params.max, 10),
                timeunit: options.params.unit
            }
        if (other) {
            var prefix = getModelPrefix(options.element.name),
                fullOtherName = appendModelPrefix(other, prefix);
            params.$otherdate = $(options.form).find(":input[name='" + fullOtherName + "']:first"); //could use findByName, but need to extend input tag
        }
        setValidationValues(options, "dateinterval", params);
    });
    //------------------------
    $.validator.addMethod("trueonlyif", function (value, element, params) {
        var isInList = function () {
            var val = this.value,
                type = this.type;
            if (type && (type === "radio" || type === "checkbox") && !this.checked) { return false; }
            return (params.othervals.indexOf(val)>-1);
        };
        return (!element.checked || params.$otherprop.is(isInList));
    });
    $.validator.unobtrusive.adapters.add("trueonlyif", ["otherprop", "othervals"], function (options) {
        var other = options.params.otherprop,
            params = {
                othervals: options.params.othervals.split("||")
            },
            prefix = getModelPrefix(options.element.name),
            fullOtherName = appendModelPrefix(other, prefix);
        params.$otherprop = $(options.form).find(":input[name='" + fullOtherName + "']"); //could use findByName, but need to extend input tag
        setValidationValues(options, "trueonlyif", params);
    });
    //------------------------
    $.validator.addMethod("cgarange", function (value, element, params) {
        var dob = parseDate($(element)),
            weeksGest = params.weeksGestEl.value || 40,
            cga;
        if (isNaN(dob) || !weeksGest) { return "pending"; }
        cga = correctedAgeInWeeks(dob, weeksGest);
        return (cga >= params.min && cga <= params.max);
    });
    $.validator.unobtrusive.adapters.add("cgarange", ["min", "max", "weeksgestationprop"], function (options) {
        var params = { min: parseInt(options.params.min, 10),
            max: parseInt(options.params.max, 10)
        },
            prefix = getModelPrefix(options.element.name),
            fullWeeksGestName = appendModelPrefix(options.params.weeksgestationprop, prefix);
        params.weeksGestEl = $(options.form).find(":input[name='" + fullWeeksGestName + "']")[0];
        setValidationValues(options, "cgarange", params);
    });
    //-------------------------
    $.validator.addMethod("wt4age", function (value, element, params) {
        var maleGender,
            zscore = getZ(),
            $wtWarnContainer = $("#wtWarnContainer");
        if (zscore == "pending") { return "pending"; }
        if (-params.zfail > zscore || zscore > params.zfail) {
            $wtWarnContainer.hide();
            return false;
        }
        if (-params.zwarn < zscore && zscore < params.zwarn) {
            $wtWarnContainer.hide();
            return true;
        }
        return warnWeightConfirmed();

        function getZ() {
            var dob = parseDate(params.$dob),
                weeksGest = params.$weeksGest[0].value || 40, cga;
            maleGender = params.$gender.not(":not(:checked)").val() === params.maleVal;
            if (isNaN(dob) || !weeksGest || !(typeof(maleGender) == "boolean")) { return "pending"; };
            cga = correctedAgeInWeeks(dob, weeksGest);
            return gausDist.zWtForAge(element.value, cga, maleGender);
        }
        function wtString() {
            return "<span class='field-validation-error'>"
                    + gausDist.suggestedDeviationMsg(zscore)
                    + "</span> healthy"
                    + (maleGender ? " males" : " females")
                    + " of the same age weigh "
                    + (zscore > 0 ? "more" : "less")
        }

        function warnWeightConfirmed() {
            var $wtWarnMsg = $("#confirmWtWarnMsg");
            function changeWtStr() {
                zscore = getZ();
                if ($.inArray(this, params.$gender) > -1) { maleGender = params.$gender.not(":not(:checked)").val() === params.maleVal; }
                $wtWarnMsg.html(wtString());
            }
            if ($wtWarnMsg.length)
            {
                changeWtStr();
                $wtWarnContainer.show(400);
                return document.getElementById("confirmWtWarnOk").checked;
            }
            $wtWarnMsg = $("<div id='wtWarnContainer' style='display:none'><label><input type='checkbox' id='confirmWtWarnOk' class='input-validation-error' />Confirm weight is correct - <span id='confirmWtWarnMsg'>"
                    + wtString()
                    + "</span></label></div>")
                .insertAfter(".field-validation-valid[data-valmsg-for='" + element.name + "'],.field-validation-error[data-valmsg-for='" + element.name + "']")
                .show(400)
                .find("#confirmWtWarnMsg");
            params.$weeksGest
                .add(params.$gender)
                .add(params.$dob)
                .on("change", changeWtStr);
            return false;
        };
    });
    $.validator.unobtrusive.adapters.add("wtforage", ["zwarn", "zfail", "dobprop", "weeksgestationprop","genderprop","maleval"], function (options) {
        var params = {
            zfail: parseFloat(options.params.zfail, 10),
            zwarn: (options.params.zwarn)?parseFloat(options.params.zwarn, 10):Number.MAX_VALUE,
            maleVal: options.params.maleval
        },
            prefix = getModelPrefix(options.element.name),
            elName = appendModelPrefix(options.params.weeksgestationprop, prefix);
        params.$weeksGest = $(options.form).find(":input[name='" + elName + "']");
        elName = appendModelPrefix(options.params.dobprop, prefix);
        params.$dob = $(options.form).find(":input[name='" + elName + "']");
        elName = appendModelPrefix(options.params.genderprop, prefix);
        params.$gender = $(options.form).find(":input[name='" + elName + "']");
        setValidationValues(options, "wt4age", params);
    });
    //-------------------------
    $.validator.addMethod('requiredgroup', function (value, element, params) {
        var i = 0;
        for (; i < params.length;i++) {
            if (params[i].checked === true) { return true; }
        }
        return false;
    });
    var valGroups = [];
    $.validator.unobtrusive.adapters.add('requiredgroup', function (options) {
        var groupName = options.element.name,
            uniqueGroup = getUniqueFormId(options.form) + groupName;
        if (!valGroups[uniqueGroup]) {
            valGroups[uniqueGroup] = findByName(groupName, options.form);
        }
        //jQuery Validation Plugin 1.9.0 only ever validates first chekcbox in a list
        //could probably work around setting this for every element:
        setValidationValues(options, 'requiredgroup', valGroups[uniqueGroup]);
    });

    //------------------------
    var milliSecs = { s: 1000,
        m: 60000,
        h: 3600000,
        d: 86400000,
        w: 604800000
    };
    function parseDate(dateArg) {
        var dateVal, dateStr, hasDatePicker = /\bhasDatepicker\b/;
        if (dateArg instanceof Date) { return dateArg; }
        if (dateArg.tagName) {
            if (hasDatePicker.test(dateArg.className)) {
                return $(dateArg).datepicker("getDate");
            }
            dateStr = dateArg.value;
        }
        else if (dateArg instanceof jQuery) {
            if (!dateArg.length) { return null; }
            if (hasDatePicker.test(dateArg[0].className)) { return dateArg.datepicker("getDate"); }
            dateStr = dateArg.val();
        }
        else if (typeof (dateArg) === "string") { dateStr = dateArg; }
        else { throw new TypeError("invalid dateArg - must be a DOM element, jQuery element, date or string representation of a date"); }
        if (!dateStr) { return null; }
        dateVal = parseIsoDate(dateStr);
        if (!dateVal) {
            var d = /([0-3]\d)\/([0-1]\d)\/([1-2]\d{3})\s([0-1]\d):([0-5]\d):([0-5]\d)\s([apAP]).[mM]./.exec(dateStr);
            if (d==null || !d.length) { return null; }
            if (d[7].toLowerCase() == "p") {
                d[4] = parseInt(d[4]);
                if (d[4] < 12) { d[4] += 12; }
            }
            dateVal = new Date(d[3], parseInt(d[2]) - 1, d[1], d[4], d[5], d[6]);
        }
        return dateVal;
    }
    function parseIsoDate(input) {
        var d = /(\d{4})-([01]\d)-([0-3]\d)T([0-2]\d):([0-5]\d):([0-5]\d)/.exec(input);
        if (d===null) { return null; }
        var parsed = new Date(input);
        if (isNaN(parsed)) { //ie < 9 FF <4
            parsed = new Date(d[1], parseInt(d[2])-1, d[3], d[4], d[5], d[6]);
        }
        return parsed;
    }
    function dateDifference(thisDate, otherDate, TimeUnitChar) {
        //if otherDate > thisDate, returns +ve, otherwise -ve
        if (TimeUnitChar === "M" || TimeUnitChar === "Y") {
            var monthsDif = (otherDate.getFullYear() - thisDate.getFullYear()) * 12 + otherDate.getMonth() - thisDate.getMonth(),
                dayDif = otherDate.getDate() - thisDate.getDate();
            //get fraction part - more important for min=0
            if (dayDif > 0) {
                monthsDif += dayDif / daysInMonth(otherDate.getFullYear(), otherDate.getMonth() + 1);
            } else if (dayDif < 0) {
                monthsDif += dayDif / daysInMonth(otherDate.getFullYear(), otherDate.getMonth());
            }
            return TimeUnitChar === "M" ? monthsDif : (monthsDif / 12);
        } else {
            var ms = milliSecs[TimeUnitChar];
            return (otherDate.getTime() - thisDate.getTime()) / ms;
        }
    };
    function daysInMonth(month, year) {
        // note this does not refer to 0 based months
        return new Date(year, month, 0).getDate();
    };
    function correctedAgeInWeeks(Dob, weeksGestationAtBirth) //, double correctFromLessThan=40, double correctToLessThan=43
    {
        weeksGestationAtBirth = parseInt(weeksGestationAtBirth, 10);
        if (isNaN(weeksGestationAtBirth)) { throw new TypeError("weeksGestationAtBirth Must be a valid integer"); }
        if (weeksGestationAtBirth >= 43 || weeksGestationAtBirth < 23) { throw new RangeError("weeksGestationAtBirth Must be between 23 and 42"); }
        return dateDifference(Dob, new Date(), 'w') + weeksGestationAtBirth;
    };
} (jQuery));