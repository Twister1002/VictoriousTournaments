jQuery(document).ready(function () {
    var $ = jQuery;
    var permissionDictionary = {
        100: "Creator",
        101: "Admin",
        102: "Participant",
        0: "None"
    };

    // Redirect to update
    $(".tournament-update").on("click", function () {
        window.location.replace("/Tournament/Update/" + $(this).closest("#Tournament").data("id"));
    });

    // Tournament Deletion
    $(".tournament-delete").on("click", function () {

        if (confirm("Are you sure you want to delete this tournament? This can no be reverted.")) {
            $.ajax({
                "url": "/Ajax/Tournament/Delete",
                "type": "POST",
                "data": { "tournamentId": $(this).closest("#Tournament").data("id") },
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
                    alert(json.message);
                }
            });
        }
    });

    // Finalize Tournament 
    $(".tournamentFinalizeButton").on("click", function () {
        var roundData = {};
        var jsonData = {
            "tournyVal": $("#Tournament").data("id"),
            "bracketVal": $(".bracket").data("bracketnum"),
        };

        $.each($(".header-rounds"), function (i, e) {
            var roundNum = 1;
            
            $.each($(e).find("li"), function (ii, ee) {
                var type = $(ee).data("type");
                var element = $(ee).find(".bestOfMatches");

                if (element.length) {
                    if (!roundData.hasOwnProperty(type)) {
                        roundData[type] = {};
                    }
                    roundData[type][roundNum] = element.val();
                    roundNum++;
                }
            });
        });

        $.ajax({
            "url": "/Ajax/Tournament/Finalize",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData), "roundData": roundData },
            "dataType": "json",
            "success": function (json) {
                json = JSON.parse(json);
                location.replace(json.redirect);
                console.log(json);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });

    $("#Tournament .bracket .options .checkIn").on("click", function () {
        $.ajax({
            "url": "/Ajax/Tournament/CheckIn",
            "type": "post",
            "data": { tournamentId: $("#Tournament").data("id") },
            "dataType": "json",
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    $("#Tournament .bracket .options .checkIn").remove();
                }

                console.log(json.message);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });

    // Tournament Information Updating and functions

    // Show the standings
    $("#Tournament  .tournamentData").on("click", function () {
        var elem = $(".TournamentInfo");

        if (elem.hasClass("open")) {
            // Close the side panel
            elem.removeClass("open");
        }
        else {
            // Open the side panel
            elem.addClass("open");
        }
    });

    $(".TournamentInfo .close").on("click", function () {
        $(this).closest(".TournamentInfo").removeClass("open");
    });

    // Torunament Bracket Information
    $(".TournamentInfo .bracketNum").on("click", BracketNumberSelected);
    // Tournament Infomation
    $(".TournamentInfo .selection.info").on("click", InfoSelected);
    $(".TournamentInfo .playerInfo .checkIn").on("click", CheckUserIn);
    // Permission Buttions
    $(".TournamentInfo .playerInfo .user .promote, .TournamentInfo .playerInfo .user .demote").on("click", PermissionAction);
    $(".TournamentInfo .playerInfo .userAddData .addUserButton").on("click", AddUserToTournament);
    $(".TournamentInfo .playerInfo .userAddData .name").on("keydown", function (e) {
        if (e.keyCode == 13) {
            AddUserToTournament(this);
        }
    });

    function BracketNumberSelected() {
        var bracketId = $(this).data("bracket");
        $(".TournamentInfo .bracketData, .TournamentInfo .bracketNum").removeClass("show");

        $(this).addClass("show");
        $(this).siblings(".TournamentInfo .bracketData[data-bracket='" + bracketId + "']").addClass("show");
    }

    function InfoSelected() {
        var bracket = $(this).closest(".bracketData");
        var info = $(this).data("show");

        $(this).addClass("show").siblings().removeClass("show");
        bracket.find("." + info).addClass("show").siblings().removeClass("show");
    }

    function CheckUserIn() {
        var $this = $(this);

        $.ajax({
            "url": "/Ajax/Tournament/CheckIn",
            "type": "post",
            "data": { "tournamentId": $("#Tournament").data("id"), "tournamentUserId": $(this).closest(".data").data("user") },
            "dataType": "json",
            "beforeSend": function () {
                $this.off("click");
            },
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    $this.removeClass("green");
                    $this.removeClass("red");
                    $this.addClass(json.isCheckedIn ? "green" : "red");
                }

                console.log(json.message);
            },
            "error": function (json) {
                console.log(json);
            },
            "complete": function () {
                $this.on("click", CheckUserIn)
            }
        });
    }

    function PermissionAction(e, action) {
        userElement = $(this).closest(".user");
        jsonData = {
            "TournamentId": $("#Tournament").data("id"),
            "targetUser": userElement.data("user"),
            "action": action ? action : $(this).attr("class")
        }

        if (jsonData.targetUser == -1) {
            userElement.remove();
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
                        if (json.data.permissions.Permission == 0) {
                            userElement.remove();
                        }
                        else {
                            var actions = userElement.find(".actions");
                            actions.html(PermissionButtons(json.data.permissions));

                            userElement.find(".permission").text(permissionDictionary[json.data.permissions.Permission]);
                            userElement.find(".checkedIn").removeClass("red green").addClass(json.data.isCheckedIn ? "green" : "red");
                        }
                    }

                    console.log(json);
                },
                "error": function (json) {
                    alert(json.message);
                },
                "complete": function () {
                    userElement.find(".promote, .demote").on("click", PermissionAction);
                    userElement.find(".actions").find("button").attr("disabled", false);
                }
            });
        }
    }

    function PermissionButtons(actions) {
        html = "";
        demoteButton = " <button class='demote'>Demote</button> ";
        promoteButton = " <button class='promote'>Promote</button> ";
        removeButton = " <button class='remove'>Remove</button> ";

        if (actions.Promote) html += promoteButton;
        if (actions.Demote) html += demoteButton;
        if (actions.Remove) html += removeButton;

        return html;
    }

    function AddUserToTournament(e) {
        $this = e ? $(e) : $(this);
        var row = $this.closest(".userAddData");

        $.ajax({
            "url": "/Ajax/Tournament/Register",
            "type": "post",
            "data": { "tournamentId": $("#Tournament").data("id"), "name": row.find(".name input").val() },
            "dataType": "json",
            "beforeSend": function () {
                row.find(".addUserButton").attr("disabled", true);
            },
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    html = "<ul class='data user' data-user='" + json.data.user.TournamentUserId + "' data-columns='4'> ";
                    html += "<li class='column name'>" + json.data.user.Name + "</li> ";
                    html += "<li class='column permission'>" + permissionDictionary[json.data.user.Permission] + "</li> ";
                    html += "<li class='column'><span class='icon icon-checkmark red'></span></li> ";
                    html += "<li class='column actions'>"+PermissionButtons(json.data.actions)+"</li> ";
                    html += "</ul> ";

                    row.after(html);
                    row.find(".name input").val('')
                }

                console.log(json.message);
            },
            "error": function (json) {
                console.log(json);
            },
            "complete": function () {
                row.find(".addUserButton").attr("disabled", false);
            }
        });
    }

    (function ($) {
        if ($(".TournamentInfo").length == 1) {
            // Load everyone's permission level
            var permission = $(".TournamentInfo .playerInfo .user .permission");
            $.each(permission, function (i, e) {
                $(e).text(permissionDictionary[$(e).text()]);
            });
        }
    })($);
});

function UpdateStandings(tournyId, bracketNum) {
    tournyId = parseInt(tournyId, 10)
    bracketNum = parseInt(bracketNum, 10);

    if (isNaN(tournyId) || isNaN(bracketNum)) return false;

    $.ajax({
        "url": "/Ajax/Bracket/Standings",
        "type": "POST",
        "data": { "tournamentId": tournyId, "bracketNum": bracketNum },
        "dataType": "json",
        "success": function (json) {
            json = JSON.parse(json);
            if (json.status) {
                var standings = $(".TournamentInfo .bracketData[data-bracket='" + bracketNum + "'] .standingInfo");
                standings.find(".data").remove();

                $.each(json.data.ranks, function (i, e) {
                    html = "<ul class='data' data-columns='3'> ";
                    html += "<li class='column rank'>" + e.Rank + "</li> ";
                    html += "<li class='column name'>" + e.Name + "</li> ";
                    if (e.usePoints) {
                        html += "<li class='column score'>" + e.Wins + " - " + e.Losses + " - " + e.Ties + "</li> ";
                    }
                    else {
                        html += "<li class='column score'></li> "
                    }
                    html += "</ul> ";

                    standings.append(html);
                });
            }
            console.log(json.message);
        },
        "error": function (json) {
            console.log(json);
        },
    });
}