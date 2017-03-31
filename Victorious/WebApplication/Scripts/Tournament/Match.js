jQuery(document).ready(function () {
    var $ = jQuery;

    // Match Administration
    $(".match-edit-module .matchData .info li").on("click", function () {
        // Remove the class
        $(this).siblings().removeClass("selected-winner");
        $(this).addClass("selected-winner");
    });

    $(".match-edit-module .module-close .close").on("click", function () {
        $(".match-edit-module").removeClass("open");
    });

    $(".matchNum .edit").on("click", function () {
        var matchNum = $(this).closest(".match").data("match");
        var tournamentId = $("#Tournament").data("id");

        $(".match-edit-module").addClass("open");
        //$(".match-edit-module .module-content .match")
        //    .data({ "match": matchNum, "tournamentId":tournamentId })
        //    .html($(this).closest(".match").html())
        //    ;

        $.ajax({
            "url": "/Match/Ajax/Match",
            "type": "POST",
            "data": { "tournament": tournamentId, "match": matchNum },
            "dataType": "json",
            "success": function (json) {
                console.log("Success");
                console.log(json);
            },
            "error": function (json) {
                console.log("error");
                console.log(json);
            }
        });
    });
    $(".match-edit-module .match-submit button").on("click", function () {
        var matchData = $(".match-edit-module .module-content .match");

        $.ajax({
            "url": "/Match/Ajax/Match/Update",
            "type": "POST",
            "data": { "match": matchData.data("match"), "tournamentId": matchData.data("tournamentId"), "seedWin": matchData.find("li.selected-winner").data("seed") },
            "dataType": "json",
            "success": function (json) {
                console.log("Success");
                console.log(json);
            },
            "error": function (json) {
                console.log("error");
                console.log(json);
            }
        });
    });



    // Mouse Events
    $(".matchData .info li").on("mouseover", function () {
        //console.log("Entered: " + $(this).data("seed"));
        var seed = $(this).data("seed");
        if (seed > -1) {
            $(".matchData .info [data-seed='" + seed + "']").addClass("seedHover");
        }
    });
    $(".matchData .info li").on("mouseleave", function () {
        //console.log("Left: " + $(this).data("seed"));
        var seed = $(this).data("seed");
        if (seed > -1) {
            $(".matchData .info [data-seed='" + seed + "']").removeClass("seedHover");
        }
    });
});