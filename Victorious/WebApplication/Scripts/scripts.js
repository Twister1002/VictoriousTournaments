jQuery(document).ready(function () {
    var $ = jQuery;

    $(".datepicker")
    .datepicker()
    .prop("readonly", "readonly")
    ;

    // Form Validation
    function Validate(form) {
        var returnVal = true;

        $(".section li", form).each(function (i, e) {
            input = $(this).find("input");
            label = $(this).find("label");

            if (input.prop("required") && input.val().length < 1) {
                label.addClass("required");
                returnVal = false;
            }
            else {
                label.removeClass("required");
            }
        });

        return retrunVal;
    }
});