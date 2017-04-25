jQuery(document).ready(function () {
    var $ = jQuery;

    var PlayerSlot = {
        "-1": "unspecified",
        "0": "defender",
        "1": "challenger"
    }

    // Mouse Events
    $(".TournamentMatch .overview .defender, .TournamentMatch .overview .challenger").on("mouseover", function () {
        //console.log("Entered: " + $(this).data("seed"));
        var userid = $(this).data("id");
        if (userid > -1) {
            $(".TournamentMatch .overview [data-id='" + userid + "']").addClass("teamHover");
        }
    });
    $(".TournamentMatch .overview .defender, .TournamentMatch .overview .challenger").on("mouseleave", function () {
        //console.log("Left: " + $(this).data("seed"));
        var userid = $(this).data("id");
        if (userid > -1) {
            $(".TournamentMatch .overview [data-id='" + userid + "']").removeClass("teamHover");
        }
    });

    $(".TournamentGames .options .close").on("click", function () {
        $(this).closest(".TournamentGames").removeClass("open");
    });

    // Add games to the match
    $(".TournamentGames .options .add-game").on("click", function () {
        // Add a new line to the game data
        var gameList = $(this).closest(".TournamentGames").find(".list-table-body");
        var maxGames = gameList.data("max");
        var gamesListed = gameList.find("ul").length;

        jsonData = {
            "gameNum": gamesListed + 1,
            "defenderScore": "",
            "challengerScore": ""
        }

        AddGameToDetails(jsonData, gameList);
    });

    // Show the details of the match
    $(".TournamentMatch .details").on("click", function () {
        var matchElem = $(this).closest(".TournamentMatch");
        var jsonData = {
            "matchId": $(matchElem).data("id")
        };

        $.ajax({
            "url": "/Match/Ajax/Match",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function () {
                matchElem.find(".TournamentGames").addClass("open");
            },
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    $.each(json.data.matchData, function (i, e) {
                        AddGameToDetails(e, matchElem.find(".TournamentGames .list-table-body"));
                    });
                }
                else {
                    alert(json.message);
                    console.log(json.exception);
                }
            },
            "error": function (json) {
                json = JSON.parse(json);
                $(".match-edit-module").removeClass("open");
                alert("There was an error in acquiring this match data: " + json.message);
            },
            "complete": function () {
                $(".match-edit-module .match .selected-winner").removeClass("selected-winner");
            }
        });
    });

    $(".TournamentGames .update-games").on("click", function () {
        var match = $(this).closest(".TournamentMatch");
        var games = match.find(".games ul");
        var gameData = new Array();

        var jsonData = {
            "matchId": match.data("id"),
            "matchNum": match.data("matchnum"),
            "bracketNum": $(this).closest(".bracket").data("bracketnum"),
            "tournamentId": $("#Tournament").data("id"),
        }

        // Get all the game data
        $.each(games, function (i, e) {
            gameData.push({
                "DefenderScore": $(e).find(".defender-score").val(),
                "ChallengerScore": $(e).find(".challenger-score").val()
            });
        });

        $.ajax({
            "url": "/Match/Ajax/Update",
            "type": "POST",
            "data": { "jsonIds": jsonData, "games": gameData },
            "dataType": "json",
            "beforeSend": function () {

            },
            "success": function (json) {
                json = JSON.parse(json);

                console.log(json);
            },
            "error": function (json) {
                console.log(json);
            },
            "complete": function () {

            }
        });
    });

    // Helper method to add games to details
    function AddGameToDetails(data, element) {
        html = "<ul data-columns='3'>";
        html += "<li class='game-number'>Game " + data.gameNum + "</li>";
        html += "<li class='score'><input type='text' class='defender-score' name='defender-score' maxlength='3' value='" + data.defenderScore + "' /></li>";
        html += "<li class='score'><input type='text' class='challenger-score' name='challenger-score' maxlength='3' value='" + data.challengerScore + "' /></li>";
        html += "</ul>";

        if (element.find("ul").length >= element.data("max")) {
            // Remove the add button
            element.closest(".TournamentGames").find(".options .add-game").addClass("hide");
        }
        else {
            element.append(html);
            if (element.find("ul").length >= element.data("max")) {
                element.closest(".TournamentGames").find(".options .add-game").addClass("hide");
            }
        }
    }


    //$(".match-edit-module .match-submit button").on("click", function () {
    //    var matchData = $(".match-edit-module .module-content .match")

    //    jsonData = {
    //        "tournyId": $("#Tournament").data("id"),
    //        "bracketNum": $(this).closest(".bracket").data("bracketnum"),
    //        "matchId": matchData.data("matchid"),
    //        "matchNum": matchData.data("matchnum"),
    //        "winnerId": matchData.find(".selected-winner").data("userid"),
    //        "winner": matchData.find(".selected-winner").hasClass("defender") ? "Defender" : "Challenger",
    //        "challengerScore": matchData.find(".challenger .score-edit").val(),
    //        "defenderScore": matchData.find(".defender .score-edit").val()
    //    };

    //    $.ajax({
    //        "url": "/Match/Ajax/Update",
    //        "type": "POST",
    //        "data": { "jsonData": JSON.stringify(jsonData) },
    //        "dataType": "json",
    //        "beforeSend": function () {
    //            // Disable the button
    //            $(".match-edit-module .match-submit button").attr("disabled", true);
    //        },
    //        "success": function (json) {
    //            json = JSON.parse(json);
    //            if (json.status) {

    //                if (json.data.currentMatch) {
    //                    var matchData = json.data.currentMatch;
    //                    var match = $(".list-table-body .match[data-matchnum='" + matchData.matchNum + "']");

    //                    $(match).find(".challenger .match-score").text(matchData.challenger.score);
    //                    $(match).find(".defender .match-score").text(matchData.defender.score);

    //                    if (matchData.isFinished) {
    //                        match.find(".edit").addClass("hide");
    //                    }
    //                    else {
    //                        match.find(".edit").removeClass("hide");
    //                    }
    //                }
    //                if (json.data.nextWinnerMatch) {
    //                    var matchData = json.data.nextWinnerMatch;
    //                    var match = $(".list-table-body .match[data-matchnum='" + matchData.matchNum + "']");

    //                    // Set the names
    //                    $(match)
    //                        .find(".challenger")
    //                        .attr("data-userid", matchData.challenger.id)
    //                        .data("userid", matchData.challenger.id)
    //                        .find(".name").text(matchData.challenger.name);

    //                    $(match)
    //                        .find(".defender")
    //                        .attr("data-userid", matchData.defender.id)
    //                        .data("userid", matchData.defender.id)
    //                        .find(".name").text(matchData.defender.name);

    //                    // Set the scores
    //                    $(match).find(".challenger .match-score").text(matchData.challenger.score);
    //                    $(match).find(".defender .match-score").text(matchData.defender.score);

    //                    if (matchData.isReady) {
    //                        match.find(".edit").removeClass("hide");
    //                    }
    //                    else {
    //                        match.find(".edit").addClass("hide");
    //                    }
    //                }
    //                if (json.data.nextLoserMatch) {
    //                    var matchData = json.data.nextLoserMatch;
    //                    var match = $(".list-table-body .match[data-matchnum='" + matchData.matchNum + "']");

    //                    // Set the names
    //                    (match)
    //                        .find(".challenger")
    //                        .attr("data-userid", matchData.challenger.id)
    //                        .data("userid", matchData.challenger.id)
    //                        .find(".name").text(matchData.challenger.name);

    //                    $(match)
    //                        .find(".defender")
    //                        .attr("data-userid", matchData.defender.id)
    //                        .data("userid", matchData.defender.id)
    //                        .find(".name").text(matchData.defender.name);

    //                    // Set the scores
    //                    $(match).find(".challenger .match-score").text(matchData.challenger.score);
    //                    $(match).find(".defender .match-score").text(matchData.defender.score);

    //                    if (matchData.isReady) {
    //                        match.find(".edit").removeClass("hide");
    //                    }
    //                    else {
    //                        match.find(".edit").addClass("hide");
    //                    }
    //                }
    //            }
    //            else {

    //            }
    //            console.log("Success");
    //            console.log(json);
    //        },
    //        "error": function (json) {
    //            console.log("error");
    //            console.log(json);
    //        },
    //        "complete": function () {
    //            // Remove the disabled button
    //            $(".match-edit-module .match-submit button").attr("disabled", false);

    //            // Close the module.
    //            $(".match-edit-module").removeClass("open");

    //            // Update the standings
    //            UpdateStandings(jsonData.tournyId, jsonData.bracketNum);
    //        }
    //    });
    //});

    function UpdateStandings(tourny, bracket) {
        jsonData = {
            "tournamentId": tourny,
            "bracketNum": bracket
        }

        $.ajax({
            "url": "/Tournament/Ajax/Standings",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function () {

            },
            "success": function (json) {
                json = JSON.parse(json);
                var standings = $("#TournamentStandings .standings .list-table-body");
                if (json.status) {
                    standings.empty();
                    $.each(json.data.ranks, function (i, e) {
                        html = "<ul class='border' data-columns='3'>";
                        html += "<li class='position-rank'>" + e.Rank + "</li>";
                        html += "<li class='position-name'>" + e.Name + "</li>";
                        if (e.score) html += "<li class='position-score'>" + e.Score + "</li>";
                        html += "</ul>";

                        standings.append(html);
                    });
                }
            },
            "error": function (json) {

            },
            "complete": function () {

            }
        });
    }
});