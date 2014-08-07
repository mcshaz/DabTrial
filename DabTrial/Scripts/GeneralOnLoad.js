/// <reference path="date-en-AU.js" />
/// <reference path="date-en-AU.js" />
//set jQuery extensions & javascript extensions
; Array.prototype.remove = Array.prototype.remove || function (/*argumentList*/) {
    //http://stackoverflow.com/questions/3954438/remove-item-from-array-by-value#3955096
    var remainingArgLength = arguments.length,
        index,
        what;
    while (remainingArgLength && this.length) {
        what = arguments[--remainingArgLength];
        while ((index = $.inArray(what, this)) != -1) {
            this.splice(index, 1);
        }
    }
    return this;
};
(function ($) {
    function elementText(el, separator) {
        var textContents = [];
        for (var chld = el.firstChild; chld; chld = chld.nextSibling) {
            if (chld.nodeType == 3) {
                textContents.push(chld.nodeValue);
            }
        }
        return textContents.join(separator);
    };
    $.fn.textNotChild = function (elementSeparator, nodeSeparator) {
        if (arguments.length < 2) { nodeSeparator = ""; }
        if (arguments.length < 1) { elementSeparator = ""; }
        return $.map(this, function (el) {
            return elementText(el, nodeSeparator);
        }).join(elementSeparator);
    };
    $.fn.getAttributes = function (/*nameOnly = true*/) {
        var nameOnly = (arguments.length && arguments[0] !== false && arguments[0].nameOnly !== false),
            el = this[0], attributes, i;
        if (!el) { return this; }
        if (nameOnly) {
            attributes = [];
            for (i = 0; i < el.attributes.length; i++) {
                var attr = el.attributes[i];
                if (attr.specified) {
                    attributes.push(attr.name);
                }
            }
        } else {
            attributes = {};
            for (i = 0; i < el.attributes.length; i++) {
                var attr = el.attributes[i];
                if (attr.specified) {
                    attributes[attr.name] = attr.value;
                }
            }
        }
        return attributes;
    };
    $.fn.mergeAttributes = function (element) {
        var attrToMerge;
        if (element instanceof jQuery) { attrToMerge = element.getAttributes(); }
        else if (element.tagName) { attrToMerge = $(element).getAttributes(); }
        else if (element) { attrToMerge = element; }
        if (!attrToMerge) { return this; }
        delete attrToMerge["id"];
        this.each(function () {
            $this = $(this);
            for (var attr in attrToMerge) {
                if (attrToMerge.hasOwnProperty(attr)) {
                    $this.attr(attr, attrToMerge[attr]);
                }
            }
        });
        return this;
    }
    $.fn.hasAncestor = function (selector) {
        var $el = (selector.jQuery) ? selector : $(selector);
        return this.filter(function () {
            return !!$(this).closest(selector).length;
        });
    };
    $.fn.isCheckable = function () {
        if (!this.length) { return; }
        var tag = this[0].tagName.toLowerCase();
        if (tag == "input") {
            var type = this[0].type.toLowerCase();
            return (type === "checkbox" || type === "radio");
        }
        return false;
    };
    $.fn.emptyValues = function () {
        this.each(function () {
            var $t = $(this),
                checkable, oldVal, newVal;
            if ($t.isCheckable()) {
                oldVal = $t.prop("checked");
                newVal = $t.prop("defaultChecked");
                if (newVal !== oldVal) {
                    $t.prop("checked", newVal);
                    $t.change();
                }
            } else {
                oldVal = $t.val();
                newVal = "";
                if (newVal !== oldVal) {
                    $t.val(newVal);
                    $t.change();
                }
            }
        });
        return this;
    };
    $.ajaxPrefilter(function (options, localOptions, jqXHR) {
        var token;
        if (options.type.toLowerCase() !== 'get') {
            token = GetAntiForgeryToken();
            if (!token.name) { throw new ReferenceError("Cannot find antiforgery token");}
            if (options.data.indexOf(token.name) === -1) {
                options.data = options.data + '&' + token.name + '=' + token.value;
            }
            //jqXHR.setRequestHeader(token.name, token.value);
        }
    });
    $.formCtrlTagNames = 'input,button,select,textarea';
}(jQuery));
//begin $(document).ready - not used as content loaded by script execution
(function ($) {
    var $ajaxResultDialog, minWidth, dpDefaults,dataTableEls;
    $.ajaxSetup({
        cache: false
    });
    if ($.datepicker) {
        dpDefaults = {
            dateFormat: "d/m/yy"
        };
        if (!Modernizr.input.placeholder) {
            dpDefaults.onClose = function () { $(this).blur(); }
        }
        $.datepicker.setDefaults(dpDefaults);
    }
    //sortable data grids
    
    if (($.fn.dataTable)) {
        dataTableEls = $.fn.dataTable.fnTables();
        $(".dataTable").filter(function () {
            return $.inArray(this,dataTableEls)===-1 //all elements not initialised
        }).dataTable();
    }
    yepnope({
        test: Modernizr.input.placeholder,
        nope: '/Scripts/jquery-placeholder.js',
        complete: setElementListeners
    });
    /*
    yepnope({
        test: Modernizr.textshadow,
        nope: ['/Scripts/jquery.textshadow.js', '/Content/jquery.textshadow.css']
    });
    */
    $(".emptyFields").on("click", function (event) {
        event.preventDefault();
        emptyFormElements();
        $(".rowInEditor").removeClass("rowInEditor");
    });
    $ajaxResultDialog = $("#ajaxResultDialog")
    if ($ajaxResultDialog.length) {
        minWidth = parseInt($("fieldset").css("min-width")) || 680;
        $ajaxResultDialog.dialog({
            autoOpen: false,
            minWidth: minWidth,
            height: 'auto',
            close: function () { $("#ajaxFormDialog").dialog("close"); }
        });
        $("#ajaxFormDialog").dialog({
            autoOpen: false,
            minWidth: minWidth,
            height: 'auto',
            modal: true,
            close: function (evt, ui) {
                if (evt.originalEvent && $(evt.originalEvent.target).closest(".ui-dialog-titlebar-close").length) {
                    $(".rowInEditor").removeClass("rowInEditor");
                }
            }
        });
    }
    $(".enabledif").each(function() {
        var $that = $(this),
            data = $that.data("enabledif"),
            getVal = function (el) {
                if (el.disabled) { return null; }
                switch (el.type) {
                    case 'hidden':
                        return;
                    case 'checkbox':
                        var boolVal;
                        if (el.value.toLowerCase() === 'true') { boolVal = true; }
                        else if (el.value.toLowerCase() === 'false') { boolVal = false; }
                        if (el.checked) {
                            return (boolVal==='undefined')?el.value:boolVal;
                        }
                        if (boolVal !== 'undefined') {
                            return !boolVal;
                        }
                        break;
                    case 'radio':
                        if (el.checked) {
                            return el.value;
                        }
                        break;
                    case 'select':
                        var returnVal = [], i=0;
                        for (; i < el.options.length; i++) {
                            if (el.options[i].selected) {
                                if (el.multiple) {
                                    returnVal.push(el.options[i].value);
                                } else {
                                    return el.options[i].value;
                                }
                            }
                        }
                        if (returnVal.length === 1) {
                            return returnVal[0];
                        }
                        if (returnVal.length > 1) {
                            return returnVal;
                        }
                        break;
                    default:
                        return el.value;
                }
            },
            testMatch = function ($elGroup, value) {
                var match = false, i, j,
                    arr = ($.isArray(value) ? value : [value]);
                for (i = 0; i < arr.length; i++) {
                    for (j = 0; j < $elGroup.length; j++) {
                        var elVal = getVal($elGroup[j]);
                        if ($.isArray(elVal)) {
                            match = ($.inArray(arr[i], elVal) !== -1);
                        } else {
                            match = (elVal == arr[i]);
                        }
                        if (match) { return match; }
                    }
                }
                return match;
            },
            setDisabled,
            watchedElMatches = $.map(data.propertyPairs, function (pair) {
                var $els, isEqual;
                if (!(pair.name || pair.name.length)) {return;}
                if (pair.name[0] == '#') {
                    $els = $(pair.name);
                } else {
                    $els = $(document.getElementsByName(pair.name))
                                .filter(function (indx, el) {
                                    if (el.type && el.type.toLowerCase() === 'hidden') { return false; }
                                    if ($that[0].form) { return el.form === $that[0].form; }
                                    return true;
                                });
                }
                isEqual = testMatch($els, pair.val);
                $els.on('change', function () {
                    isEqual = testMatch($els, pair.val);
                    setDisabled();
                });
                return function () { return isEqual; };
            }),
            isLogicalAnd = data.logicalOperator == 'And',
            $thatFormCtrls = $that.find($.formCtrlTagNames).addBack($.formCtrlTagNames);
        setDisabled = function () {
            var i = 0, conditionsMet = isLogicalAnd, wasDisabled;
            for (; i < watchedElMatches.length; i++) {
                if (isLogicalAnd) {
                    if (watchedElMatches[i]() == false) {
                        conditionsMet = false;
                        break;
                    }
                } else {
                    if (watchedElMatches[i]() == true) {
                        conditionsMet = true;
                        break;
                    }
                }
            }
            wasDisabled = $.map($thatFormCtrls, function (el) {
                return el.disabled;
            });
            $thatFormCtrls.prop('disabled',!conditionsMet);
            for (i = 0; i < wasDisabled.length; i++) {
                if (wasDisabled[i] === conditionsMet) {
                    $($thatFormCtrls[i]).change();
                }
            }
        };
        setDisabled();
    });
    $(".partialmirror").each(function() {
        var $t = $(this),
            otherPropName = $t.data("partialmirror-watch"),
            rgx = new RegExp($t.data("partialmirror-regex")),
            map = function () {
                var match = rgx.exec(this.value);
                if (match && match.length > 1) {
                    $t.val(match[1]);
                }
            },
            $otherEl = $(document.getElementById(otherPropName)),
            evt;
        if (!$otherEl.length) {
            $otherEl = $(document.getElementsByName(otherPropName.replace(/_/g, '.')));
            if (!$otherEl.length) { console.log(otherPropName + " not found"); return; }
        }
        evt = (($otherEl[0].type || "").toLowerCase() == 'file') ? "change" : "keyup"
        if ($t.data("partialmirror-always")) {
            $otherEl.on(evt, map);
        } else {
            $otherEl.on("focus", function () {
                var val = $t.val(),
                    match = rgx.exec(this.value),
                    that=this;
                if (!val || (match && match[1] == val)) {
                    $otherEl.on(evt, map);
                    $otherEl.on("blur", function () {
                        map.call(that);
                        $otherEl.off(evt, map);
                    });
                }
            });
        }
    });
    $(".expando").each(function () {
        var $t = $(this),
            $toggle = $t.find(".expandoToggle"),
            $slidingDiv = $t.next("div"),
            setClass = function () {
                if ($slidingDiv.is(":hidden")) {
                    $toggle.removeClass("expandoVisible").addClass("expandoHidden");
                } else {
                    $toggle.removeClass("expandoHidden").addClass("expandoVisible");
                }
            };
        $t.on("click", function () {
            $slidingDiv.slideToggle({
                complete: function () {
                    setClass();
                }
            });
        });
        if (!$t.hasClass("defaultOpen")) {
            $slidingDiv.hide();
        }
        setClass();
    });
})(jQuery);
//global functions
function onShowAutoWidthDialog(jqDialog) {
    // fix for auto width in IE
    var parent = jqDialog.parent();
    var contentWidth = jqDialog.width();
    parent.find('.ui-dialog-titlebar').each(function () {
        jQuery(this).width(contentWidth);

    });
    parent.removeClass("autoWidthDialog").width(contentWidth + 26);
    jqDialog.dialog('option', 'position', 'center');

    // fix for scrollbars in IE
    jQuery('body').css('overflow', 'hidden');
    jQuery('.ui-widget-overlay').css('width', '100%');
};
function onHideAutoWidthDialog(jqDialog) {
    // fix for auto width in IE
    var parent = jqDialog.parent();
    parent.find('.ui-dialog-titlebar').each(function () {
        // reset titlebar width
        jQuery(this).css('width', '');
    });
    parent.addClass("autoWidthDialog");

    // fix for scrollbars in IE
    jQuery('body').css('overflow', 'auto');
};
function showDetail() {
    var $this = $(this),
        detail = $this.children()
                    .filter("option[value='" + this.value + "']:first")
                    .data("detail");
    $("#" + $this.attr("detaildisplayid")).html(detail || "");
};
//ajax dialog form functions
function displayAjaxFormDialog(data) {
    var $ajaxForm = $(document.forms).filter("#ajaxForm").html(""), //$("#ajaxForm") (+document.getElementById) does not find any elements in IE!
        $dialogDiv = $("#ajaxFormDialog").html(data),
        $newForm = $("form").hasAncestor($dialogDiv),
        newFormAttr = removeProperty($newForm.getAttributes(), "id", "name", "method", "data-ajax", "data-ajax-loading", "novalidate");
    // formEvents = $._data($ajaxForm[0],"events"),
    $ajaxForm.removeAttr($ajaxForm.getAttributes({ nameOnly: true }).remove("id", "name", "method", "data-ajax", "data-ajax-loading", "novalidate").join(" "))
             .attr(newFormAttr);
    $newForm.children()
            .appendTo($ajaxForm);
    $newForm.replaceWith($ajaxForm);
    setDialogHeader($dialogDiv);
    setElementListeners.call($dialogDiv);
    $ajaxForm.removeData("validator");
    $ajaxForm.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($ajaxForm);
    $dialogDiv.dialog('open');
    return false;
};
function failedAjaxDelete(xhr, status, error) {
    var $dialogDiv = $(".rowInEditor")
                    .addClass("field-validation-error")
                    .closest("#ajaxResultDialog,#ajaxFormDialog"),
        statusCat = parseInt(status.toString().substring(1)),
        msgBase = "Failed to delete the selected row"+
                    (statusCat == 4
                    ?", usually because the information has changed since the page was downloaded."
                    :" because of a server error (which has been logged).");
    if ($dialogDiv.length) {
        if (statusCat == 4) {
            alert(msgBase + " After clicking OK the window will be closed. Please reopen to see updated information");
            $dialogDiv.dialog('close');
            return;
        }
    } else if (statusCat == 4) {
        alert(" After clicking OK the page will be reloaded.");
        location.reload();
    }
    alert(msgBase);
};
function displayAjaxResultDialog(data) {
    if (isJsonError(data)) {
        displayJsonError(data);
        return false;
    }
    var $dialogDiv = $("#ajaxResultDialog").html(data);
    setDialogHeader($dialogDiv);
    $("#ajaxFormDialog").dialog('close');
    setElementListeners.call($dialogDiv);
    $dialogDiv.dialog('open');
    return false;
};
function setDialogHeader($contentDiv) {
    var $header = $contentDiv.find("h1,h2,h3").first(),
        $titleBar = $("#ui-dialog-title-" + $contentDiv[0].id),
        $headerContents = $header.contents();
    if ($headerContents.length) { $titleBar.html("").append($headerContents); }
    else { $titleBar.html("&nbsp;"); }
    $header.remove();
};
function emptyFormElements() {
    var maintainState = /\bmaintainState\b/,
        $formEls = $("input"),
        checkboxNames = [];
    $formEls.each(function (indx,el) {
            if (el.type==="checkbox" && el.name && $.inArray(el.name, checkboxNames) === -1) { checkboxNames.push(el.name); }
        });
    $formEls = $formEls.filter(function () {
        return this.type != "hidden" || this.getAttribute("data-datatype") == "primarykey" || !$.inArray(this.Name, checkboxNames);
    });
    clearFormErrors();
    $formEls = $formEls.add($("select,textarea"));
    $formEls.each(function () {
        if (!this.form || this.type === "submit") return;
        if (this.getAttribute("data-datatype") == "primarykey") { this.value = "-1"; }
        else if (!maintainState.test(this.className)) {
            if (this.isEncrypted) { this.isEncrpted = this.disabled = false;}
            $(this).emptyValues();
        }
        if (this.getAttribute("details")) { showDetail.call(this); }
    });
};
function clearFormErrors() {
    $(".validation-summary-errors").find("li").remove();
    $(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid");
    $(".input-validation-error").removeClass("input-validation-error");
};
function rowDeleted() {
    var $rowToDelete = $(".deleteRow"),
        $table = $rowToDelete.closest("table");
    if ($rowToDelete.hasClass("rowInEditor")) { emptyFormElements(); }
    if ($rowToDelete.length === 0) { return; }
    if ($table.hasClass("dataTable")) {
        $table.dataTable().fnDeleteRow($rowToDelete[0]);
    } else {
        $rowToDelete.remove();
    }
};
function populateFormElements(data) {
    var isDateInput = /\bhasDatepicker\b/,
        isDateData=/^\/Date\((\d+)\)\/$/,
        $that = $($.formCtrlTagNames);
    clearFormErrors();
    $that.each(function () {
        var newVal, type, isChanged, $t,
            type = (this.tagName.toLowerCase() == "input") ? (this.type || "").toLowerCase()
                : "";
        if (!this.form || !this.name || type == "submit") { return; }
        newVal = data[this.name];
        if (typeof (newVal) === 'undefined') { return; }
        if (type === "hidden" && (newVal === true || newVal === false) && $that.is("input[type=checkbox][name='"+this.name+"'],input[type=radio][name='"+this.name+"']")) {
            return;
        }
        if (newVal === null) { newVal = ""; }
        if (isDateData.test(newVal)) {
            newVal = new Date(parseInt(isDateData.exec(newVal)[1]));
            if (isDateInput.test(this.className)) {
                $t = $(this);
                isChanged = $(this).datepicker("getDate") != newVal;
                $(this).datepicker("setDate", newVal);
            } else {
                isChanged = this.value != newVal.toLocaleDateString();
                this.value = newVal.toLocaleDateString();
            }
            if (isChanged) { ($t || $(this)).change(); }
        } else if (type == "checkbox" || type == "radio") {
            if (type == "checkbox") { 
                isChanged = this.checked != newVal;
                this.checked = newVal;
            } else if (type == "radio") {
                isChanged = this.checked != (this.value == newVal);
                this.checked = this.value == newVal;
            }
            if (isChanged) { $(this).change(); }
            if (this.disabled) { this.checked = false; }
            return;
        } else {
            isChanged = this.value != newVal;
            this.value = newVal;
            if (newVal == 'Encrypted')
            {
                this.disabled = this.isEncrypted = true;
            }
            else if(this.isEncrypted)
            {
                this.disabled = this.isEncrypted = false;
            }
            if (this.getAttribute("details")) { showDetail.call(this); }
        }
        if (isChanged) { ($t || $(this)).change(); }
    });
};
function appendSelect(data) {
    if (isJsonError(response)) {
        displayJsonError(response);
        return false;
    }
    $("#" + data.target).append("<option selected='selected' value='" + data.selectItem.value + "'>" + data.selectItem.text + "</option>");
    $("#ajaxFormDialog").dialog('close');
    return true;
};
//http://stackoverflow.com/questions/2808327/how-to-read-modelstate-errors-when-returned-by-json#2808925
function getValidationSummary() {
    var $el = $(".validation-summary-errors > ul");
    if ($el.length == 0) {
        $el = $("<div class='validation-summary-errors'><ul></ul></div>")
                .insertBefore($('fieldset').has('input')[0])
                .find('ul');
    }
    return $el;
};
function handleErrorResponse(response) {
    alert("Appologies - the server has returned an unexpected error. The window will be reloaded. If the error persists after reloading and data is resubmitted, please contact a website administrator.");
    location.reload();
};
function isJsonError(json) { return (json && json.Tag == "ValidationError"); }
function displayJsonError(response, form, summaryElement) {
    var $list;
    if (!isJsonError(response)) { return false; }
    $list = summaryElement || getValidationSummary();
    $list.html('');
    $.each(response.State, function (i, item) {
        var $val, lblTxt = "", errorList = "";
        if (item.Name) {
            $val = $(".field-validation-valid,.field-validation-error")
                        .filter("[data-valmsg-for='" + item.Name + "']")
                        .removeClass("field-validation-valid")
                        .addClass("field-validation-error");
            $("input[name='" + item.Name + "']").addClass("input-validation-error");
            lblTxt = $("label[for='" + item.Name + "']").textNotChild();
            if (lblTxt) { lblTxt += ": "; }
        }
        if ($val && $val.length) {
            $val.text(item.Errors.shift());
            if (!item.Errors.length) { return; }
        }
        $.each(item.Errors, function (c, val) {
            errorList += "<li>" + lblTxt + val + "</li>";
        });
        $list.append(errorList);
    });
    return true;
};
function ajaxRedraw(response) {
    if (isJsonError(response)) {
        displayJsonError(response);
    } else {
        emptyFormElements();
        var dts = $.fn.dataTable.fnTables(true);
        if (dts.length) {
            $(dts[dts.length - 1]).dataTable().fnDraw();
        }
    }
}
function ajaxAddOrChangeRow(response) {
    if (isJsonError(response)) {
        displayJsonError(response);
        return false;
    }
    var $editRow = $(".rowInEditor"),
        $ajaxTBody = $(".ajaxUpdatableTBody").filter(":visible").last(),
        $ajaxTable = $ajaxTBody.parent(),
        rowObject;
    emptyFormElements();
    if ($.fn.dataTable && $.fn.dataTable.fnIsDataTable($ajaxTable)) {
        rowObject = ($.type(response) == 'string')
            ?$.map($(response).find('td'), function (item) {
                return $.trim(item.innerHTML);
            })
            :response;
        if ($editRow.length > 0) {
            $ajaxTable.dataTable().fnUpdate(rowObject, $editRow[0]);
        } else {
            $ajaxTable.dataTable().fnAddData(rowObject);
        }
    } else {
        if ($editRow.length > 0) {
            $editRow.replaceWith(response);
        } else {
            $ajaxTBody.prepend(response);
        }
    }
    //now set up for new entry
    $editRow.removeClass("rowInEditor");
    return true;
};
function closeAndAddOrChangeRow(response) {
    if (ajaxAddOrChangeRow(response)) {
        closeAjaxFormDialog(response);
    }
};
function closeAjaxFormDialog(response) {
    if (isJsonError(response)) {
        displayJsonError(response);
        return false;
    }
    if (response && response.redirectUrl) {
        window.location.href = response.redirectUrl;
    } else {
        $("#ajaxFormDialog").dialog('close');
    }
    return true;
};
function GetAntiForgeryToken() {
    var tokenField = $("input[type='hidden'][name$='RequestVerificationToken']");
    if (tokenField.length == 0) { return null; }
    else {
        return {
            name: tokenField[0].name,
            value: tokenField[0].value
        };
    }
};
//End of ajax form functions
function setToggleVis(e) {
    var $t = $(this),
        $toggleEl = $('.' + $t.data('toggleclass')),
        duration = (e && e.data && e.data.duration)?e.data.duration:400;
    if (duration === "instant") {
        if ($t.prop('checked')) {
            $toggleEl.show();
        } else {
            $toggleEl.hide();
        }
    } else {
        if ($t.prop('checked')) {
            $toggleEl.slideDown(duration);
        } else {
            $toggleEl.slideUp(duration);
        }
    }
};
function setElementListeners() {
    var show = function (input, inst) {
            if ($.trim(input.value)) {
                if (inst.selectedYear != 0 && inst.selectedMonth != 0 && inst.selectedDay != 0) {
                    return { defaultDate: new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay) };
                }
            }
            return { defaultDate: new Date() };
        },
        context = (this==window)?document:this;
        
    //date & time pickers
    if ($.datepicker) {
        var toggleDatePicker = function () {
            var visible = $('#ui-datepicker-div').is(':visible');
            $(this).datepicker(visible ? 'hide' : 'show');
        };
        $(".date", context).not("[type=hidden]").add("input[type=date]")
            .datepicker({ beforeShow: show });
        //.on("mousedown", toggleDatePicker);
        if ($.timepicker) {
            $(".dateTime", context).not("[type=hidden]")
                .datetimepicker({
                    timeFormat: "HH:mm",
                    //seperator: " ",
                    hour: (new Date()).getHours(),
                    beforeShow: show
                });
            //.on("mousedown", toggleDatePicker);
        }
    }
    
    $(".toggleElements", context)
        .each(function(){
            setToggleVis.call(this, { data: { duration: "instant" } });
            $(this).on('click', setToggleVis);
        });
    //select lists with details
    $("select", context)
        .filter("[details='true']")
        .each(showDetail)
        .on("change", showDetail);
    if (!Modernizr.input.placeholder) {
        $("input,textarea", context)
            .placeholder();
    }
    //this class identifies a table (ideally tbody) with edit and update buttons
    $(".ajaxUpdatableTBody", context)
        .on("click", ".deleteAction", function (event) {
            $(".deleteRow").removeClass("deleteRow");
            $(this).closest("tr").addClass("deleteRow");
        })
        .on("click", ".editLink", function (event) {
            $(".rowInEditor").removeClass("rowInEditor");
            $(this).closest("tr").addClass("rowInEditor");
        });
};
//
function removeProperty(obj/*, argList*/) {
    var args = arguments;
    if ($.isArray(obj)) {
        return removeFromArray.apply(obj, args);
    } else {
        var remainingArgLength = args.length;
        while (remainingArgLength > 1) {
            delete obj[args[--remainingArgLength]];
        }
        return obj;
    }
};

