﻿jQuery(document).ready(function () {
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

    $(".matchNum .edit").on("click", function () {
        var matchNum = $(this).closest(".match").data("match");
        var tournamentId = $("#Tournament").data("id");

        //$(".match-edit-module").addClass("open");
        //$(".match-edit-module .module-content .match")
        //    .data({ "match": matchNum, "tournamentId":tournamentId })
        //    .html($(this).closest(".match").html())
        //    ;

        $.ajax({
            "url": "/Match/Ajax/Match",
            "type": "POST",
            "data": { "tournament": tournamentId, "match": matchNum},
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

    $(".match-edit-module").on("click", ".matchData .info li", function () {
        // Remove the class
        $(this).siblings().removeClass("selected-winner");
        $(this).addClass("selected-winner");
    });

    $(".match-edit-module .module-close .close").on("click", function () {
        $(".match-edit-module").removeClass("open");
    });

    $(".tournament-delete").on("click", function () {
        if (confirm("Are you sure you want to delete this tournament? This can no be reverted.")) {
            $.ajax({
                "url": "/Tournament/Ajax/Delete",
                "type": "POST",
                "data": {"id": $(this).data("id")},
                "dataType": "json",
                "success": function (json) {
                    console.log("Success");
                    console.log(json);
                },
                "error": function (json) {
                    console.log("error");
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