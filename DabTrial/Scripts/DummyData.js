;(function ($) {
    var prependElStr = 'label:not(#selectAll),legend,.display-label:not(:has(label))'
        prependText = "Dummy ";
    $(prependElStr).prepend(prependText);
    $(document).ajaxSuccess(function () {
        $(prependElStr).contextFilter('#ajaxFormDialog').prepend(prependText);
    });
})(jQuery);