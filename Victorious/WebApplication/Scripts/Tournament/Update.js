jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        3: "rr",
        6: "swiss"
    };
    var bracketInfo;
    var bracketsCreated = 0;

    //TODO: Fix the issue with %n% not being replaced.
    $("#TournamentEdit #BracketType").on("change", BracketInfoChange);
    $("#TournamentEdit .icon-plus").on("click", function () {
        var newBracket = bracketInfo.replace(/%n%/g, $("#TournamentEdit .brackets").length);
        $("#TournamentEdit .bracketSection").append("<ul class='brackets'>" + newBracket + "</ul>");

        bracketsCreated++;
    });

    function BracketInfoChange() {
        $("#TournamentEdit .bracketInfo").addClass("hide");
        $("." + BracketTypesDictionary[$("#TournamentEdit #BracketType").val()]).removeClass("hide");
    }

    (function ($) {
        if ($("#TournamentEdit").length == 1) {
            BracketInfoChange();
            // Remove the bracket selection and save it.
            bracketInfo = $("#TournamentEdit .bracketOrig").removeClass("hide").html();
            $("#TournamentEdit .bracketOrig").remove();
        }
    })($);
});