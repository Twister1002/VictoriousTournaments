jQuery(document).ready(function () {
    var $ = jQuery;

    $("#TournamentEdit .list-table-body .user .user-options .remove").on("click", function () {
        $.ajax({
            "url": "/Tournament/Ajax/Demote",
            "type": "POST",
            "data": { "tournyVal": $("#TournamentEdit").data("tournament"), "userVal": $(this).closest(".user").data("user") },
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
    });

    $("#TournamentEdit .list-table-body .user .user-options .promote").on("click", function () {
        $.ajax({
            "url": "/Tournament/Ajax/Promote",
            "type": "POST",
            "data": { "tournyVal": $("#TournamentEdit").data("tournament"), "userVal": $(this).closest(".user").data("user") },
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
    });
});