jQuery(document).ready(function () {
    var $ = jQuery;

    $(".datepicker")
    .datepicker()
    .prop("readonly", "readonly")
    ;

    $(".matchData .info li").on("mouseover", function () {
        //console.log("Entered: " + $(this).data("seed"));
        var seed = $(this).data("seed");
        if (seed > -1) {
            $(".matchData .info [data-seed='" + seed + "']").addClass("seedHover");
        }
    });
    $(".matchData .info li").on("mouseleave", function() {
        //console.log("Left: " + $(this).data("seed"));
        var seed = $(this).data("seed");
        if (seed > -1) {
            $(".matchData .info [data-seed='" + seed + "']").removeClass("seedHover");
        }
    });

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