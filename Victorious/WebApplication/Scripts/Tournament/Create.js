jQuery(document).ready(function () {
    var $ = jQuery;
    var BracketTypesDictionary = {
        6: "swiss"
    };

    $("#TournamentCreate #BracketType").on("change", function () {
        $("#TournamentCreate .bracketInfo").addClass("hide");
        $("." + BracketTypesDictionary[$(this).val()]).removeClass("hide");
    });
});