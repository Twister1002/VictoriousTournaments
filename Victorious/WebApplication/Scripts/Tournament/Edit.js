jQuery(document).ready(function () {
    var $ = jQuery;
    var addedUserIndex = 0;
    var permissionDictionary = {
        100: "Creator",
        101: "Admin",
        102: "Participant",
        0: "None"
    };

    $("#TournamentEdit .list-table-body .user .actions .demote").on("click", permissionDemote);
    $("#TournamentEdit .list-table-body .user .actions .promote").on("click", permissionPromote);

    function permissionDemote() {
        PermissionAction("demote", $(this).closest(".user"));
    }

    function permissionPromote() {
        PermissionAction("promote", $(this).closest(".user"));
    }

    function PermissionAction(action, userElement) {
        jsonData = {
            "TournamentId": $("#TournamentEdit").data("id"),
            "targetUser": userElement.data("user"),
            "action": action
        }

        if (jsonData.targetUser == -1) {
            $(userElement).remove();
        }
        else {
            $.ajax({
                "url": "/Ajax/Tournament/PermissionChange",
                "type": "POST",
                "data": jsonData,
                "dataType": "json",
                "beforeSend": function () {
                    // Prevent buttons from being clicked again
                    userElement.find(".actions").find("button").attr("disabled", true);
                    userElement.find(".promote").off("click");
                    userElement.find(".demote").off("click");
                },
                "success": function (json) {
                    json = JSON.parse(json);
                    if (json.status) {
                        if (json.data.permissionChange == 0) {
                            userElement.remove();
                        }
                        else {
                            var actions = userElement.find(".actions");
                            actions.html(PermissionButtons(json.data.actions));

                            userElement.find(".permission").text(permissionDictionary[json.data.permissionChange]);
                        }
                    }

                    console.log(json);
                },
                "error": function (json) {
                    alert(json.message);
                },
                "complete": function () {
                    userElement.find(".promote").on("click", permissionPromote);
                    userElement.find(".demote").on("click", permissionDemote);
                    userElement.find(".actions").find("button").attr("disabled", false);
                }
            });
        }
    }

    $("#TournamentEdit .user-section .addUser").on("click", function () {
        html = "<ul class='user border form' data-user='-1' data-columns='4'> ";
        html += "<li class='column name'><input type='text' name='Users[" + addedUserIndex + "].Name' id='Users[" + addedUserIndex + "].Name' maxlength='50' placeholder='Email or Username'/></li> ";
        html += "<li class='column permission'>Not Saved</li> ";
        html += "<li class='column checkedIn'><span class='icon icon-checkmark'></span></li> ";
        html += "<li class='column actions'><button type='button' class='demote'>Remove</button></li> ";
        html += "</ul> ";
        
        addedUserIndex++;
        $(this).closest(".user-section").find(".users").append(html);

        // Add the event to the button
        $(".user-section .user .actions .demote").off("click");
        $(".user-section .user .actions .demote").on("click", permissionDemote);
    });

    function PermissionButtons(actions) {
        html = "";
        demoteButton = "<button class='demote'>Demote</button> ";
        promoteButton = "<button class='promote'>Promote</button> ";
        removeButton = "<button class='remove'>Remove</button> ";

        if (actions.Promote) html += promoteButton;
        if (actions.Demote)  html += demoteButton;
        if (actions.Remove) html += removeButton;

        return html;
    }
});