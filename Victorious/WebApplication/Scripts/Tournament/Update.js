jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        "3": "rr",
        "6": "swiss"
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
        var bracketData = $(this).closest(".bracketSection");
        var rounds = bracketData.find(".roundSelect");

        // Display the round selection for the user and hide it if its not applicable and reset to 0
        if (BracketTypesDictionary.hasOwnProperty($(this).val())) {
            // Display the field
            rounds.removeClass("hide");
            rounds.find("select").val(0);
        }
        else {
            rounds.addClass("hide");
            rounds.find("select").val(0);
        }
    }

    (function ($) {
        if ($("#TournamentEdit").length == 1) {
            // Remove the bracket selection and save it.
            bracketInfo = $("#TournamentEdit .bracketOrig").removeClass("hide").html();
            $("#TournamentEdit .bracketOrig").remove();

            $("#TournamentEdit .bracketSection .brackets .type select").trigger("change");
        }
    })($);
});