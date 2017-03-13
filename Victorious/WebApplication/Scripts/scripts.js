jQuery(document).ready(function () {
    var $ = jQuery;

    $(".datepicker")
    .datepicker()
    .prop("readonly", "readonly")
    ;

    //$(".slider")
    //.slider({
    //    range: true,
    //    min: 3,
    //    max: 128,
    //    value: 3,
    //    slide: function (event, ui) {
    //        $(".slider").val(ui.value);
    //    }
    //})
    //.val($(this).slider("value"));

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
});