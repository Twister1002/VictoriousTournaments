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
    var bracketsCreated = 0;
    var maxBrackets = 2;
    var userInfo;

    $("#TournamentEdit .bracketSection .type select").on("change", BracketInfoChange);
    $("#TournamentEdit .addBracket").on("click", AddBracket);
    $("#TournamentEdit .addUser").on("click", AddUser);
    
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
            // Remove the bracket selection and save it.
            bracketInfo = $("#TournamentEdit .bracketSection .bracketOrig").removeClass("hide").html();
            $("#TournamentEdit .bracketSection .bracketOrig").remove();

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