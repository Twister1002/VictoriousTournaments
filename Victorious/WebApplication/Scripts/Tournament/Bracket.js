jQuery(document).ready(function () {

    // Show the standings
    $(".bracket .bracket-info .options .tournament-standings").on("click", function () {
        var elem = $(".TournamentStandings");

        if (elem.hasClass("open")) {
            // Close the side panel
            elem.removeClass("open");
        }
        else {
            // Open the side panel
            elem.addClass("open");
        }
    });

    $(".TournamentStandings .options .close").on("click", function () {
        $(this).closest(".TournamentStandings").removeClass("open");
    });

    // Reset the brackets
    $(".bracket-info .options .tournament-reset").on("click", function () {
        var bracketId = $(this).closest(".bracket").data("id");

        if (confirm("Are you sure you want to reset this bracket?")) {
            $.ajax({
                "url": "/Ajax/Bracket/Reset",
                "type": "POST",
                "data": { "bracketId": bracketId },
                "dataType": "json",
                "success": function (json) {
                    json = JSON.parse(json);
                    if (json.status) {
                        window.location.replace(json.redirect);
                    }
                    else {
                        alert(json.message);
                    }
                },
                "error": function (json) {
                    console.log("Error");
                }
            });
        }
        else {
            return false;
        }
    });
});