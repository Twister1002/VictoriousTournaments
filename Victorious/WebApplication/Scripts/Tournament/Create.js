jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        3: "rr",
        6: "swiss"
    };

    $("#TournamentCreate #BracketType").on("change", BracketInfoChange);

    function BracketInfoChange() {
        $("#TournamentCreate .bracketInfo").addClass("hide");
        $("." + BracketTypesDictionary[$("#TournamentCreate #BracketType").val()]).removeClass("hide");
    }

    (function ($) {
        if ($("#TournamentCreate").length == 1) {
            BracketInfoChange();
        }
    })($);
});