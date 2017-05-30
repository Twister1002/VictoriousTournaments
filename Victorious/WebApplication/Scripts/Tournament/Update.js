jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        3: "rr",
        6: "swiss"
    };

    $("#TournamentEdit #BracketType").on("change", BracketInfoChange);

    function BracketInfoChange() {
        $("#TournamentEdit .bracketInfo").addClass("hide");
        $("." + BracketTypesDictionary[$("#TournamentEdit #BracketType").val()]).removeClass("hide");
    }

    (function ($) {
        if ($("#TournamentEdit").length == 1) {
            BracketInfoChange();
        }
    })($);
});