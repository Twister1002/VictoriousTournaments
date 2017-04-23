jQuery(document).ready(function () {

    // Show the standings
    $(".bracket-info .options .tournament-standings").on("click", function () {
        var elem = $("#TournamentStandings");

        if (elem.hasClass("open")) {
            // Close the side panel
            elem.removeClass("open");
        }
        else {
            // Open the side panel
            elem.addClass("open");
        }
    });

    // Reset the brackets
    $(".bracket-info .options .tournament-reset").on("click", function () {
        var jsonData = {
            "tournyNum": $(this).closest("#Tournament").data("id")
        };

        if (confirm("Are you sure you want to reset non-finished brackets?")) {
            $.ajax({
                "url": "/Tournament/Ajax/Reset",
                "type": "POST",
                "data": { "jsonData": JSON.stringify(jsonData) },
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
                    json = JSON.parse(json);

                    console.log("Error");
                }
            });
        }
        else {
            return false;
        }
    });
});