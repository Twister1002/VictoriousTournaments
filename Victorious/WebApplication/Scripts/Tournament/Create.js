jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        "3": "rr",
        "6": "swiss"
    };
    var bracketInfo;
    var bracketsCreated = 0;

    //TODO: Fix the issue with %n% not being replaced.
    $("#TournamentCreate .bracketSection .type select").on("change", BracketInfoChange);
    $("#TournamentCreate .icon-plus").on("click", function () {
        var newBracket = bracketInfo.replace(/%n%/g, bracketsCreated);
        $("#TournamentCreate .bracketSection").append("<ul class='brackets'>"+newBracket+"</ul>");
        
        bracketsCreated++;

        //Reset all the events 
        $("#TournamentCreate .bracketSection .type select").off("change");
        $("#TournamentCreate .bracketSection .type select").on("change", BracketInfoChange);
    });

    function BracketInfoChange() {
        var bracketData = $(this).closest(".brackets");
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
        if ($("#TournamentCreate").length == 1) {
            // Remove the bracket selection and save it.
            bracketInfo = $("#TournamentCreate .bracketOrig").removeClass("hide").html();
            $("#TournamentCreate .bracketOrig").remove();

            $("#TournamentEdit .bracketSection .brackets .type select").trigger("change");
        }
    })($);
});