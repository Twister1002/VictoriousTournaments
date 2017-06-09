jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        3: "rr",
        6: "swiss"
    };
    var bracketInfo;
    var bracketsCreated = 0;

    //TODO: Fix the issue with %n% not being replaced.
    $("#TournamentCreate #BracketType").on("change", BracketInfoChange);
    $("#TournamentCreate .icon-plus").on("click", function () {
        var newBracket = bracketInfo.replace(/%n%/g, bracketsCreated);
        $("#TournamentCreate .bracketSection").append("<ul class='brackets'>"+newBracket+"</ul>");

        bracketsCreated++;
    });

    function BracketInfoChange() {
        $("#TournamentCreate .bracketInfo").addClass("hide");
        $("." + BracketTypesDictionary[$("#TournamentCreate #BracketType").val()]).removeClass("hide");
    }

    (function ($) {
        if ($("#TournamentCreate").length == 1) {
            BracketInfoChange();
            // Remove the bracket selection and save it.
            bracketInfo = $("#TournamentCreate .bracketOrig").removeClass("hide").html();
            $("#TournamentCreate .bracketOrig").remove();
        }
    })($);
});