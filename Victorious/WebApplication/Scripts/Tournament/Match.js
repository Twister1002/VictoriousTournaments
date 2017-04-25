jQuery(document).ready(function () {
    var $ = jQuery;

    var PlayerSlot = {
        "-1": "unspecified",
        "0": "defender",
        "1": "challenger",
        "unspecified": -1,
        "defender": 0,
        "challenger": 1,
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
        var gameList = $(this).closest(".TournamentGames");
        var maxGames = gameList.data("max");
        var gamesListed = gameList.find(".list-table-body ul").length;

        jsonData = {
            "gameNum": gamesListed + 1,
            "scores": {
                0: "",
                1: "",
            }
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
                    // Remove all games
                    matchElem.find(".TournamentGames .list-table-body").empty();

                    // Add the currently fetched games
                    $.each(json.data.matchData, function (i, e) {
                        AddGameToDetails(e, matchElem.find(".TournamentGames"));
                    });

                    if (CanAddGames(matchElem.find(".TournamentGames")) && !json.data.finished) {
                        matchElem.find(".options .add-game").removeClass("hide");
                    }
                    else {
                        matchElem.find(".options .add-game").addClass("hide");
                    }
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
            "data": { "jsonIds": JSON.stringify(jsonData), "games": gameData },
            "dataType": "json",
            "beforeSend": function () {
                
            },
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    console.log(json.message);

                    if (json.data.currentMatch) {
                        match = $(".TournamentMatch[data-id='" + json.data.currentMatch.matchId + "']");
                        MatchUpdate(json.data.currentMatch, match);
                    }
                    if (json.data.winnerMatch) {
                        match = $(".TournamentMatch[data-id='" + json.data.winnerMatch.matchId + "']");
                        MatchUpdate(json.data.winnerMatch, match);
                    }
                    if (json.data.loserMatch) {
                        match = $(".TournamentMatch[data-id='" + json.data.loserMatch.matchId + "']");
                        MatchUpdate(json.data.loserMatch, match);
                    }

                    UpdateStandings(jsonData.tournamentId, jsonData.bracketNum);
                }
                else {
                    console.log("Error in updating");
                }

                console.log(json);
            },
            "error": function (json) {
                console.log(json);
            },
            "complete": function () {

            }
        });
    });

    function MatchUpdate(json, $match) {
        overview = $match.find(".overview");
        games = $match.find(".TournamentGames");

        // Update the Match data
        overview.find(".defender .name").text(json.defender.name);
        overview.find(".defender .score").text(json.defender.score);
        overview.find(".challenger .name").text(json.challenger.name);
        overview.find(".challenger .score").text(json.challenger.score);

        // Update the Game data 
        games.find(".defender-name").text(json.defender.name);
        games.find(".challenger-name").text(json.challenger.name);

        // Verify if the match is ready
        if (json.ready) {
            // Show the details button
            $match.find(".details").removeClass("hide");
        }
        else {
            $match.find(".details").addClass("hide");
        }

        // Verify if the users can add more games
        if (CanAddGames(games)) {
            games.find(".options .add-game").removeClass("hide");
        }
        else {
            games.find(".options .add-game").addClass("hide");
        }

        // Verify the match is finished
        if (json.finished) {
            games.find(".update-games").addClass("hide");
        }
        else {
            games.find(".update-games").removeClass("hide");
        }
    }

    // Helper method to add games to details
    function AddGameToDetails(data, $games) {
        html = "<ul data-columns='3'>";
        html += "<li class='game-number'>Game " + data.gameNum + "</li>";
        html += "<li class='score'><input type='text' class='defender-score' name='defender-score' maxlength='3' value='" + data.scores[PlayerSlot["defender"]] + "' /></li>";
        html += "<li class='score'><input type='text' class='challenger-score' name='challenger-score' maxlength='3' value='" + data.scores[PlayerSlot["challenger"]] + "' /></li>";
        html += "</ul>";

        if (CanAddGames($games)) {
            $games.find(".list-table-body").append(html);
            if (!CanAddGames($games)) {
                $games.find(".options .add-game").addClass("hide");
            }
        }
        else {
            // Remove the add button
            $games.find(".options .add-game").addClass("hide");
        }
    }

    function CanAddGames($games) {
        if ($games.find(".list-table-body ul").length >= $games.find(".list-table-body").data("max")) {
            return false;
        }
        else {
            return true;
        }
    }

    function UpdateStandings(tournyId, bracket) {
        jsonData = {
            "tournamentId": tournyId,
            "bracketNum": bracket
        };

        $.ajax({
            "url": "/Tournament/Ajax/Standings",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function () {

            },
            "success": function (json) {
                json = JSON.parse(json);
                var standings = $(".TournamentStandings .standings .list-table-body");
                if (json.status) {
                    standings.empty();

                    $.each(json.data.ranks, function (i, e) {
                        html = "<ul class='position' data-columns='3'>";
                        html += "<li class='rank'>" + e.Rank + "</li>";
                        html += "<li class='name'>" + e.Name + "</li>";
                        if (e.score > -1) html += "<li class='score'>" + e.Score + "</li>";
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