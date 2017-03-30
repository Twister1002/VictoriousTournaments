jQuery(document).ready(function () {
    var $ = jQuery;

    // Tournament Deletion
    $(".tournament-delete").on("click", function () {
        if (confirm("Are you sure you want to delete this tournament? This can no be reverted.")) {
            $.ajax({
                "url": "/Tournament/Ajax/Delete",
                "type": "POST",
                "data": { "tourny": $(this).data("id") },
                "dataType": "json",
                "success": function (json) {
                    if (json.success) {
                        window.location.replace(json.redirect);
                    }
                    else {
                        alert(json.message);
                    }
                },
                "error": function (json) {
                    alert(json.message);
                }
            });
        }
    });

    // View tournament standings
    $(".tournament-standings").on("click", function () {
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
});