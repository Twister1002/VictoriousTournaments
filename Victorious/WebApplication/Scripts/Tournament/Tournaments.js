﻿jQuery(document).ready(function () {
    var $ = jQuery;
    var tournamentChanged = false;

    // Redirect to update
    $(".tournament-update").on("click", function () {
        window.location  = "/Tournament/Update/" + $(this).closest("#Tournament").data("id");
    });

    // Tournament Deletion
    $(".tournament-delete").on("click", function () {

        if (confirm("Are you sure you want to delete this tournament? This can not be reverted.")) {
            $.ajax({
                "url": "/Ajax/Tournament/Delete",
                "type": "POST",
                "data": { "tournamentId": $(this).closest("#Tournament").data("id") },
                "dataType": "json",
                "success": function (json) {
                    if (json.status) {
                        window.location = json.data.redirect;
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

    $("#Tournament .tournament-buttons.options .checkIn").on("click", function () {
        $.ajax({
            "url": "/Ajax/Tournament/CheckIn",
            "type": "post",
            "data": { tournamentId: $("#Tournament").data("id") },
            "dataType": "json",
            "success": function (json) {
                if (json.status) {
                    $("#Tournament .tournament-buttons.options .checkIn").remove();
                }

                console.log(json.message);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });

    // Tournament Bracket Selections
    $("#Tournament .bracketNames .bracketName").on("click", function () {
        $(this).addClass("selected").siblings().removeClass("selected");
        
        ShowBracket($(this).data("bracket"));
    });

    function ShowBracket(bracketId) {
        $("#Tournament .bracket[data-id='" + bracketId + "']").addClass("selected").siblings().removeClass("selected");
    }

    // Tournament Information Updating and functions
    // Show the standings
    $("#Tournament  .tournamentData").on("click", function () {
        $(".TournamentGames").removeClass("open");
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

        if (tournamentChanged) location.reload();
    });

    // Torunament Bracket Information
    $(".TournamentInfo .bracketNum").on("click", BracketNumberSelected);
    // Tournament Infomation
    $(".TournamentInfo .selection.info").on("click", InfoSelected);
    // Reset the brackets
    $(".TournamentInfo .resetInfo .reset-bracket").on("click", ResetBracket);
    //Seeds
    $(".TournamentInfo .playerInfo .adminSettings .randomSeeds").on("click", RandomizeSeeds);
    $(".TournamentInfo .playerInfo .adminSettings .updateSeeds").on("click", SaveSeeds);

    function BracketNumberSelected() {
        var bracketId = $(this).data("bracket");
        $(".TournamentInfo .bracketData, .TournamentInfo .bracketNum").removeClass("show");

        $(this).addClass("selected").siblings().removeClass("selected");
        $(this).siblings(".TournamentInfo .bracketData[data-bracket='" + bracketId + "']").addClass("show");
    }

    function InfoSelected() {
        var bracket = $(this).closest(".bracketData");
        var info = $(this).data("show");

        $(this).addClass("selected").siblings().removeClass("selected");
        bracket.find("." + info).addClass("show").siblings().removeClass("show");
    }

    function RandomizeSeeds() {
        var users = $(this).closest(".playerInfo").find(".user input").toArray();
        var random = [];

        while (users.length != 0) {
            var randomIndex = Math.floor(Math.random() * users.length);
            random.push(users[randomIndex]);
            users.splice(randomIndex, 1);
        }

        $.each(random, function (i, e) {
            $(e).val(i + 1);
        });
    }

    function SaveSeeds() {
        var jsonData = {
            "tournamentId": $("#Tournament").data("id"),
            "bracketId": $(this).closest(".bracketData").data("id"),
            "players": {}
        };

        // Load all the players into an object
        $(this).closest(".playerInfo").find(".user").each(function (i, e) {
            if ($(e).find(".seed input").length == 1) {
                jsonData.players[$(e).data("user")] = $(e).find(".seed input").val();
            }
        });

        $.ajax({
            "url": "/Ajax/Tournament/SeedChange",
            "type": "POST",
            "data": jsonData,
            "dataType": "json",
            "success": function (json) {
                if (json.status) {
                    tournamentChanged = true;
                }
                console.log(json.message);
            },
            "error": function (json) {
                console.log("Error");
            }
        });
    }

    function ResetBracket() {
        $.ajax({
            "url": "/Ajax/Bracket/Reset",
            "type": "POST",
            "data": { "tournamentId": $("#Tournament").data("id"), "bracketId": $(this).closest(".bracketData").data("id") },
            "dataType": "json",
            "success": function (json) {
                if (json.status) {
                    window.location.replace(json.data.redirect);
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

    (function ($) {
        if ($(".TournamentInfo").length == 1) {
            if ($(".TournamentInfo .bracketNum").length > 0) {
                // Always auto select the first bracket
                $(".TournamentInfo .bracketNum")[0].click();

                // Always auto select the standings
                $(".TournamentInfo .bracketData .list-table-header .selection[data-show='standingInfo']").click();
            }
        }

        if ($("#Tournament").length == 1) {
            if ($("#Tournament .bracketName.selected").length == 0) {
                if ($("#Tournament .bracketName").length > 0) {
                    $("#Tournament .bracketName")[0].click();
                }
            }
        }
    })($);
});

function UpdateStandings(tournyId, bracketId) {
    tournyId = parseInt(tournyId, 10)
    bracketId = parseInt(bracketId, 10);

    if (isNaN(tournyId) || isNaN(bracketId)) return false;

    $.ajax({
        "url": "/Ajax/Bracket/Standings",
        "type": "POST",
        "data": { "tournamentId": tournyId, "bracketId": bracketId },
        "dataType": "json",
        "success": function (json) {
            if (json.status) {
                var standings = $(".TournamentInfo .bracketData[data-bracket='" + bracketId + "'] .standingInfo");
                standings.find(".data").remove();

                $.each(json.data.ranks, function (i, e) {
                    html = "<ul class='data' data-columns='3'> ";
                    html += "<li class='column rank'>" + e.Rank + "</li> ";
                    html += "<li class='column name'>" + e.Name + "</li> ";
                    if (json.data.usePoints) {
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