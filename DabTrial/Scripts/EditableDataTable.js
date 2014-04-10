;(function() {
    var paths = window.location.pathname.split("/"),
        insertTemplate = '<a class="editLink" href="/{basePath}/{currentPath}/{0}" data-ajax-success="populateFormElements" data-ajax-method="Get" data-ajax-loading="#ajaxRequest_processing" data-ajax="true">edit</a> | <a href="/{basePath}/Details/{0}" data-ajax-success="displayAjaxResultDialog" data-ajax-method="Get" data-ajax-loading="#ajaxRequest_processing" data-ajax="true">details</a> | <a class="deleteAction" href="/{basePath}/Delete/{0}" data-ajax-success="ajaxRedraw" data-ajax-method="Post" data-ajax-loading="#ajaxRequest_processing" data-ajax-failure="failedAjaxDelete" data-ajax-confirm="Are you sure you want to delete Encrypted?" data-ajax="true">delete</a>'
            .replace(/{basePath}/g, paths[1]).replace('{currentPath}',paths[2]),
        r = /\{0\}/g;
    // `data` refers to the data for the cell (defined by `mData`, which
    // defaults to the column being worked with, in this case is the first
    // Using `row[0]` is equivalent.
    window.showEditControls = function (data, type, row) {
        return insertTemplate.replace(r, data);
    };
})();