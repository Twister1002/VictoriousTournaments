jQuery(document).ready(function () {
    var $ = jQuery;
    var permissionDictionary = {
        100: "Creator",
        101: "Admin",
        102: "Participant",
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
            "TournamentId": $("#TournamentEdit").data("id"),
            "targetUser": userElement.data("user"),
            "action": action
        }

        $.ajax({
            "url": "/Ajax/Tournament/PermissionChange",
            "type": "POST",
            "data": jsonData,
            "dataType": "json",
            "beforeSend": function() {
                // Prevent buttons from being clicked again
                userElement.find(".actions").find("button").attr("disabled", true);
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

    $("#TournamentEdit .user-section .addUser").on("click", function () {
        $(this).closest(".user-section").find(".addUserRow").addClass("show");
    });

    $("#TournamentEdit .addUserRow .name").on("keydown", function (e) {
        if (e.keyCode == 13) { // Enter
            AddNewUser($(this).closest(".form"));
        }
    });

    $("#TournamentEdit .addUserRow .addUserButton").on("click", function () {
        AddNewUser($(this).closest(".form"));
    });

    function AddNewUser($form) {
        var jsonData = {
            "Name": $form.find(".name").val(),
            "TournamentID": $("#TournamentEdit").data("id")
        };

        $.ajax({
            "url": "/Ajax/Tournament/Register",
            "type": "post",
            "data": jsonData,
            "dataType": "json",
            "beforeSend": function() {
                $form.find(".addUserButton").attr("disabled", true);
            },
            "success": function (json) {
                json = JSON.parse(json);
                console.log(json);

                if (json.status) {
                    var listSection = $("#TournamentEdit .user-section .users");

                    html = "<ul class='user border' data-user='" + json.data.TournamentUserID + "' data-columns='3'> ";
                    html += "<li class='column name'>" + json.data.Name + "</li> ";
                    html += "<li class='column permission'>" + permissionDictionary[json.data.PermissionLevel] + "</li> ";
                    html += "<li class='column actions'>" + PermissionButtons(json.data.actions) + "</li> ";
                    html += "<ul> ";

                    listSection.append(html);

                    $(".user-section ul.user").find(".actions .promote").on("click", permissionPromote);
                    $(".user-section ul.user").find(".actions .demote, .actions .remove").on("click", permissionDemote);
                    $form.find(".name").val('');
                }
                else {

                }
            },
            "error": function (json) {
                console.log(json);
            },
            "complete": function () {
                $form.find(".addUserButton").attr("disabled", false);
            }
        });
    }

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