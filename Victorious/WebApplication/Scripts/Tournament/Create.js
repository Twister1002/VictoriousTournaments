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

    //TODO: Fix the issue with %n% not being replaced.
    $("#TournamentEdit .bracketSection .type select").on("change", BracketInfoChange);
    $("#TournamentEdit .icon-plus").on("click", function () {
        var newBracket = bracketInfo.replace(/%n%/g, $("#TournamentEdit .brackets").length);
        $("#TournamentEdit .bracketSection").append("<ul class='brackets'>" + newBracket + "</ul>");

        bracketsCreated++;

        //Reset all the events 
        $("#TournamentEdit .bracketSection .type select").off("change");
        $("#TournamentEdit .bracketSection .type select").on("change", BracketInfoChange);
    });

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

    (function ($) {
        if ($("#TournamentEdit").length == 1) {
            // Remove the bracket selection and save it.
            bracketInfo = $("#TournamentEdit .bracketSection .bracketOrig").removeClass("hide").html();
            $("#TournamentEdit .bracketSection .bracketOrig").remove();

            $("#TournamentEdit .bracketSection .brackets .type select").trigger("change");
        }
    })($);
});