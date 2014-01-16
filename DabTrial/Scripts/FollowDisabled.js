(function ($) {
    var $div = $('<div class="notRequired">Not Required</div>'),
        $allRows = $('tr.editor-row').has('input,textarea'),
        $table = $allRows.closest('table'),
        tabletop = $table.offset().top-4,
        setdiv = function () {
            var height = 0,
                $rows = $allRows.has(':disabled');
            if (!$rows.length) {
                $div.hide();
                return;
            }
            $rows.each(function () {
                height += $(this).height();
            });
            $div.css({
                marginTop: $rows.offset().top - tabletop + "px",
                lineHeight: height + "px"
            });
            $div.height(height);
            $div.show();
        };
    setdiv();
    $div.width($allRows.width());
    $div.insertBefore($table);
    $allRows.find('input').on('change', setdiv);
    // this would be the best: $table.on('click', 'input', setdiv); but then a problem of execution order
})(jQuery);