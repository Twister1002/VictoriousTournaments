jQuery(document).ready(function () {
    var $ = jQuery;

    $("#TournamentEdit .list-table-body .user .user-options .remove").on("click", function () {
        var userElement = $(this).closest(".user");

        $.ajax({
            "url": "/Tournament/Ajax/Demote",
            "type": "POST",
            "data": { "tournyVal": $("#TournamentEdit").data("tournament"), "userVal": userElement.data("user") },
            "dataType": "json",
            "success": function (json) {
                if (json.status) {
                    // Move the current user down off the list.\
                    var userColumn = userElement.closest(".userTable").data("columntype");

                    // Move them down the rope
                    switch (userColumn) {
                        case "admin":
                            var columns = $(".userTable[data-columntype='participant']");
                            userElement.detach().appendTo(columns[columns.len]);
                            break;
                        case "participant":
                            userElement.html('&nbsp;');
                            break;
                    }
                }

                //alert(json.message);
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
                if (json.status) {
                    // Move the current user down off the list.
                    var user = $(this).closest(".user");
                    var userColumn = user.closest(".userTable").data("columntype");

                    // Move them down the rope
                    switch (userColumn) {
                        case "admin":
                            break;
                        case "participant":
                            var columns = $(".userTable[data-columntype='admin']");
                            user.detach().appendTo(columns[columns.len]);
                            break;
                    }
                }

                alert(json.message);
            },
            "error": function (json) {
                alert(json.message);
            }
        });
    });
});