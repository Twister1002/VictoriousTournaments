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

    $(".tournament-delete").on("click", function () {
        if (confirm("Are you sure you want to delete this tournament? This can no be reverted.")) {
            $.ajax({
                "url": "/Tournament/Ajax/Delete",
                "type": "POST",
                "data": {"id": $(this).data("id")},
                "dataType": "json",
                "success":function(json){
                    console.log(json);
                },
                "error":function(json) {
                    console.log(json);
                }
            })
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