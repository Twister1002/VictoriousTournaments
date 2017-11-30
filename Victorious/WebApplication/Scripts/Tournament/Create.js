jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        "3": {
            "show": ".roundSelect",
            "hide": ".groupSelect"
        },
        "6": {
            "show": ".roundSelect",
            "hide": ".groupSelect"
        },
        "4": {
            "show": ".groupSelect",
            "hide": ".roundSelect"
        },
    };
    var bracketInfo;
    var broadcasterInfo;
    var bracketsCreated = 0;
    var maxBrackets = 2;
    var userInfo;

    $("#TournamentEdit .bracketSection .type select").on("change", BracketInfoChange);
    $("#TournamentEdit .addBracket").on("click", AddBracket);
    $("#TournamentEdit .addUser").on("click", AddUser);
    $("#TournamentEdit .add-broadcaster").on("click", AddBroadcaster);
    $("#TournamentEdit .broadcaster-section .remove-broadcaster").on("click", RemoveBroadcaster);
    
    function AddBracket() {
        var newBracket = bracketInfo.replace(/%n%/g, $("#TournamentEdit .brackets").length);
        $("#TournamentEdit .bracketSection").append("<ul class='brackets'>" + newBracket + "</ul>");

        bracketsCreated++;

        //Reset all the events 
        $("#TournamentEdit .bracketSection .type select").off("change");
        $("#TournamentEdit .bracketSection .type select").on("change", BracketInfoChange);

        // Display the new field only if there is multiple brackets
        if (bracketsCreated > 1) {
            $("#TournamentEdit .bracketSection .advancePlayers").not(":last").removeClass("hide");
        }
        else {
            $("#TournamentEdit .bracketSection .advancePlayers").addClass("hide");
        }

        LimitBrackets();
    }

    function AddUser() {
        var newBracket = userInfo.replace(/%n%/g, $("#TournamentEdit .userInfo .user").length);

        $("#TournamentEdit .userInfo").append("<ul data-columns='3' class='user'>" + newBracket + "</ul>");
    }

    function AddBroadcaster() {
        var broadcaster = broadcasterInfo.replace(/%n%/g, $("#TournamentEdit .broadcaster-section .broadcaster").length);
        $("#TournamentEdit .broadcaster-section .info").append("<ul data-columns='3' class='broadcaster'>" + broadcaster + "</ul>");

        $("#TournamentEdit .broadcaster-section .broadcaster .remove-broadcaster").off("click").on("click", RemoveBroadcaster);
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
        var bracketData = $(this).closest(".brackets");
        var info = BracketTypesDictionary[$(this).val()];

        if (info) {
            // Show and hide the data
            bracketData.find(info.show).removeClass("hide").find("select");
            bracketData.find(info.hide).addClass("hide").find("select").val(0);
        }
        else {
            bracketData.find(".roundSelect").addClass("hide").val(0);
            bracketData.find(".groupSelect").addClass("hide").val(0);
        }
    }

    // This function will limit the brackets in the tournament
    function LimitBrackets() {
        $("#TournamentEdit .bracketSection .addBracket").off("click").addClass("hide");

        if ($("#TournamentEdit .brackets").length < maxBrackets) {
            $("#TournamentEdit .bracketSection .addBracket").on("click", AddBracket).removeClass("hide");
        }
    }

    (function ($) {
        if ($("#TournamentEdit").length == 1) {
            // Save the original data
            bracketInfo = $("#TournamentEdit .bracketSection .bracketOrig").removeClass("hide").html();
            broadcasterInfo =  $("#TournamentEdit .broadcaster-section .orig").removeClass("hide").html();

            // Delete the displayed data.
            $("#TournamentEdit .bracketSection .bracketOrig").remove();
            $("#TournamentEdit .broadcaster-section .orig").remove();

            // Trigger all the fields
            $("#TournamentEdit .bracketSection .brackets .type select").trigger("change");

            // Show the Advance Players if more than one bracket
            if ($("#TournamentEdit .bracketSection .brackets").length > 1) {
                $("#TournamentEdit .bracketSection .advancePlayers").not(":last").removeClass("hide");
            }

            LimitBrackets();

            // Save the original player data
            userInfo = $("#TournamentEdit .userSection .userOrig").removeClass("hide").html();
            $("#TournamentEdit .userSection .userOrig").remove();
        }
    })($);
});