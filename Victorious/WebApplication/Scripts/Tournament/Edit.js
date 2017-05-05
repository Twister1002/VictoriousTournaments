jQuery(document).ready(function () {
    var $ = jQuery;
    var permissionDictionary = {
        2: "Admin",
        1: "Participant",
        0: "None"
    };

    $("#TournamentEdit .list-table-body .user .demote").on("click", permissionDemote);
    $("#TournamentEdit .list-table-body .user .promote").on("click", permissionPromote);

    function permissionDemote() {
        PermissionAction("demote", $(this).closest(".user"));
    }

    function permissionPromote() {
        PermissionAction("promote", $(this).closest(".user"));
    }

    function PermissionAction(action, userElement) {
        jsonData = {
            "tournyVal": $("#TournamentEdit").data("tournament"),
            "userVal": userElement.data("user"),
            "action": action
        }

        $.ajax({
            "url": "/Tournament/Ajax/PermissionChange",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function() {
                // Prevent buttons from being clicked again
                userElement.find(".actions").find("button").attr("disabled", true);
            },
            "success": function (json) {
                json = JSON.parse(json);
                if (json.status) {
                    demoteButton = "<button class='demote'>Demote</button> ";
                    promoteButton = "<button class='promote'>Promote</button> ";
                    removeButton = "<button class='remove'>Remove</button> ";

                    if (json.permissionChange == 0) {
                        userElement.remove();
                    }
                    else {
                        var actions = userElement.find(".actions");
                        actions.html('');
                        userElement.find(".permission").text(permissionDictionary[json.permissionChange]);

                        if (json.actions.Promote) actions.append(promoteButton);
                        if (json.actions.Demote) actions.append(demoteButton);
                        if (json.actions.Remove) actions.append(removeButton);
                    }
                }

                console.log(json);
            },
            "error": function (json) {
                json = JSON.parse(json);
                alert(json.message);
            },
            "complete": function () {
                userElement.find(".promote").on("click", permissionPromote);
                userElement.find(".demote").on("click", permissionDemote);
                userElement.find(".actions").find("button").attr("disabled", false);
            }
        });
    }

    $("#TournamentEdit .user-section .addUser").on("click", function () {
        $(this).closest(".user-section").find(".addUserRow").addClass("show");
    });

    $("#TournamentEdit .addUserRow .addUserButton").on("click", function () {
        var dataRow = $(this).closest(".addUserRow");

        var jsonData = {

        };
    });
});