jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        "3": {
            "show": ".round-select",
            "hide": ".group-select"
        },
        "6": {
            "show": ".round-select",
            "hide": ".group-select"
        },
        "4": {
            "show": ".group-select",
            "hide": ".round-select"
        },
    };
    var bracketInfo;
    var broadcasterInfo;
    var maxBrackets = 2;
    var userInfo;

    $("#TournamentEdit .bracket-section .type select").on("change", BracketInfoChange);

    $("#TournamentEdit .addBracket").on("click", AddBracket);
    $("#TournamentEdit .addUser").on("click", AddUser);
    $("#TournamentEdit .add-broadcaster").on("click", AddBroadcaster);

    $("#TournamentEdit .remove").on("click", RemoveBracket);
    $("#TournamentEdit .broadcaster-section .remove").on("click", RemoveBroadcaster);
    
    function AddBracket() {
        var newBracket = bracketInfo.replace(/%n%/g, $("#TournamentEdit .bracket-section .info").length);
        $("#TournamentEdit .bracket-section .info").append("<ul class='bracket'>" + newBracket + "</ul>");

        ConsructBracketData();
    }

    function RemoveBracket() {
        $(this).closest(".bracket").remove();

        ConsructBracketData();
    }

    function ConsructBracketData() {
        // Need to reorder and rename all brackets
        $("#TournamentEdit .bracket-section .bracket").each(function (i, e) {
            type = $(e).find(".type");
            advance = $(e).find(".advance-players");
            round = $(e).find(".round-select");
            group = $(e).find(".group-select");

            type.find("select").attr("name", type.find("select").attr("name").replace(/[1234567890]/g, i));
            advance.find("select").attr("name", advance.find("select").attr("name").replace(/[1234567890]/g, i));
            round.find("select").attr("name", round.find("select").attr("name").replace(/[1234567890]/g, i));
            group.find("select").attr("name", group.find("select").attr("name").replace(/[1234567890]/g, i));
        });

        //Reset all the events 
        $("#TournamentEdit .bracket-section .type select").off("change");
        $("#TournamentEdit .bracket-section .type select").on("change", BracketInfoChange);
        $("#TournamentEdit .bracket-section .remove").off("click").on("click", RemoveBracket);
        LimitBrackets();
    }

    function AddUser() {
        var newBracket = userInfo.replace(/%n%/g, $("#TournamentEdit .user-section .user").length);

        $("#TournamentEdit .user-section .info").append("<ul data-columns='3' class='user'>" + newBracket + "</ul>");
    }

    function AddBroadcaster() {
        var broadcaster = broadcasterInfo.replace(/%n%/g, $("#TournamentEdit .broadcaster-section .broadcaster").length);
        $("#TournamentEdit .broadcaster-section .info").append("<ul data-columns='3' class='broadcaster'>" + broadcaster + "</ul>");

        $("#TournamentEdit .broadcaster-section .broadcaster .remove").off("click").on("click", RemoveBroadcaster);
    }

    function RemoveBroadcaster() {
        $(this).closest(".broadcaster").remove();

        // Has to refactor all elements in this to fix errors with linking and array data
        $("#TournamentEdit .broadcaster-section .broadcaster").each(function (i, e) {
            $(e).find("li input").attr("name", $(e).find("li input").attr("name").replace(/[1234567890]/g, i));
            $(e).find("li select").attr("name", $(e).find("li select").attr("name").replace(/[1234567890]/g, i));
        });
    }

    function BracketInfoChange() {
        var bracketData = $(this).closest(".bracket");
        var info = BracketTypesDictionary[$(this).val()];

        if (info) {
            // Show and hide the data
            bracketData.find(info.show).removeClass("hide").find("select");
            bracketData.find(info.hide).addClass("hide").find("select").val(0);
        }
        else {
            bracketData.find(".round-select").addClass("hide").val(0);
            bracketData.find(".group-select").addClass("hide").val(0);
        }
    }

    // This function will limit the brackets in the tournament
    function LimitBrackets() {
        brackets = $("#TournamentEdit .bracket").length;

        $("#TournamentEdit .bracket-section .addBracket").off("click").addClass("hide");

        if (brackets < maxBrackets) {
            $("#TournamentEdit .bracket-section .addBracket").on("click", AddBracket).removeClass("hide");
        }

        // Display the new field only if there is multiple brackets
        if (brackets > 1) {
            $("#TournamentEdit .bracket-section .advance-players").not(":last").removeClass("hide");
        }
        else {
            $("#TournamentEdit .bracket-section .advance-players").addClass("hide");
        }
    }

    (function ($) {
        if ($("#TournamentEdit").length == 1) {
            // Save the original data
            bracketInfo = $("#TournamentEdit .bracket-section .orig").removeClass("hide").html();
            broadcasterInfo = $("#TournamentEdit .broadcaster-section .orig").removeClass("hide").html();
            userInfo = $("#TournamentEdit .user-section .orig").removeClass("hide").html();

            // Delete the original displayed data.
            $("#TournamentEdit .orig").remove();

            // Trigger all the fields
            $("#TournamentEdit .bracket-section .bracket select").trigger("change");

            LimitBrackets();
        }
    })($);
});