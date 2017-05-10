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
        var nextGameNumber = $(gameList.find(".games ul")[gamesListed - 1]).data("gamenum") + 1;

        jsonData = {
            "gameNum": (isNaN(nextGameNumber) ? 1 : nextGameNumber),
            "challenger": { "score": "" },
            "defender": { "score": "" }
        };

        AddGameToDetails(jsonData, gameList);

        if (CanAddGames(gameList)) {
            $(this).removeClass("hide");
        }
        else {
            $(this).addClass("hide");
        }
    });

    // Show the details of the match
    $(".TournamentMatch .details").on("click", function () {
        var matchElem = $(this).closest(".TournamentMatch");
        var jsonData = {
            "matchId": $(matchElem).data("id")
        };

        $.ajax({
            "url": "/Ajax/Match",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function () {
                matchElem.find(".TournamentGames").addClass("open");
                matchElem.find(".TournamentGames .list-table-body").empty();
            },
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    console.log(json);
                    $games = matchElem.find(".TournamentGames");

                    // Add the currently fetched games
                    $.each(json.data.games, function (i, e) {
                        AddGameToDetails(e, matchElem.find(".TournamentGames"));
                    });
                    MatchUpdate(json.data, matchElem);
                    MatchOptionsUpdate(json.data, $games);
                }
                else {
                    console.log(json.exception);
                }
            },
            "error": function (json) {
                json = JSON.parse(json);
                console.log(json);
                matchElem.find(".TournamentGames").removeClass("open");
            },
            "complete": function () {

            }
        });
    });

    $(".TournamentGames .update-games").on("click", function () {
        var match = $(this).closest(".TournamentMatch");
        var games = match.find(".games ul");
        var gameData = new Array();
        var validated = true;

        // Validate the games fields
        // For every game
        $.each(games, function (i, e) {
            // For every game's row
            defenderScore = $(e).find(".defender-score").val();
            challengerScore = $(e).find(".challenger-score").val();

            defenderScoreValid = $.isNumeric(defenderScore) || Math.floor(defenderScore) != defenderScore;
            challengerScoreValue = $.isNumeric(challengerScore) || Math.floor(challengerScore) != challengerScore;

            if (!defenderScoreValid || !challengerScoreValue) {
                $(e).find(".score").addClass("invalid");
            }
            else {
                $(e).find(".score").removeClass("invalid");
            }
        });

        if (!validated) return;

        var jsonData = {
            "matchId": match.data("id"),
            "matchNum": match.data("matchnum"),
            "bracketNum": $(this).closest(".bracket").data("bracketnum"),
            "tournamentId": $("#Tournament").data("id"),
        }

        // Get all the game data
        $.each(games, function (i, e) {
            gameData.push({
                "GameNumber": $(e).data("gamenum"),
                "DefenderScore": $(e).find(".defender-score").val(),
                "ChallengerScore": $(e).find(".challenger-score").val()
            });
        });

        $.ajax({
            "url": "/Ajax/Match/Update",
            "type": "POST",
            "data": { "jsonIds": JSON.stringify(jsonData), "games": gameData },
            "dataType": "json",
            "beforeSend": function () {
                match.find(".TournamentGames .update-games").attr("disabled", true);
            },
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    console.log(json.message);
                    var matchElement = null;

                    // Close the match details if the match is finished.
                    if (json.data.currentMatch.finished) {
                        match.find(".TournamentGames").removeClass("open");
                    }

                    if (json.data.currentMatch) {
                        matchElement = $(".TournamentMatch[data-id='" + json.data.currentMatch.matchId + "']");
                        MatchUpdate(json.data.currentMatch, matchElement);
                        MatchOptionsUpdate(json.data.currentMatch, matchElement.find(".TournamentGames"));
                    }
                    if (json.data.winnerMatch) {
                        matchElement = $(".TournamentMatch[data-id='" + json.data.winnerMatch.matchId + "']");
                        MatchUpdate(json.data.winnerMatch, matchElement);
                    }
                    if (json.data.loserMatch) {
                        matchElement = $(".TournamentMatch[data-id='" + json.data.loserMatch.matchId + "']");
                        MatchUpdate(json.data.loserMatch, matchElement);
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
                match.find(".TournamentGames .update-games").attr("disabled", false);
            }
        });
    });

    $(".TournamentGames .options .reset-match").on("click", function () {
        var match = $(this).closest(".TournamentMatch");
        var jsonData = {
            "bracketId": $(this).closest(".bracket").data("id"),
            "matchNum": match.data("matchnum")
        };

        $.ajax({
            "url": "/Ajax/Bracket/MatchReset",
            "type": "post",
            "data": { "bracketId": $(this).closest(".bracket").data("id"), "matchNum": match.data("matchnum") },
            "dataType": "json",
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    $.each(json.data, function (i, e) {
                        MatchUpdate(e, $(".TournamentMatch[data-id='" + e.matchId + "']"));

                        if (e.matchNum == match.data("matchnum")) {
                            MatchOptionsUpdate(e, match.find(".TournamentGames"));
                        }
                    });
                    
                    match.find(".TournamentGames .games").empty();
                }

                console.log(json.message);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });

    function MatchUpdate(json, $match) {
        overview = $match.find(".overview");
        games = $match.find(".TournamentGames");

        // Update the Match data
        overview.find(".defender .name").text(json.defender.name);
        overview.find(".defender .score").text(json.defender.score);
        overview.find(".defender").attr("data-id", json.defender.id).data("id", json.defender.id);

        overview.find(".challenger .name").text(json.challenger.name);
        overview.find(".challenger .score").text(json.challenger.score);
        overview.find(".challenger").attr("data-id", json.challenger.id).data("id", json.challenger.id);

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
    }

    // Helper method to add games to details
    function AddGameToDetails(data, $games) {
        html = "<ul data-columns='3' data-gameNum='"+data.gameNum+"'>";
        html += "<li class='column game-number'>Game " + data.gameNum + "</li>";
        html += "<li class='column score'><input type='text' class='defender-score' name='defender-score' maxlength='3' value='" + data.defender.score + "' /></li>";
        html += "<li class='column score'><input type='text' class='challenger-score' name='challenger-score' maxlength='3' value='" + data.challenger.score + "' /></li>";
        //html += "<li class='column'><span class='icon icon-cross removeGame'></span></li>";
        html += "</ul>";

        $games.find(".games").append(html);
    }

    function CanAddGames($games) {
        if ($games.find(".games ul").length >= $games.find(".games").data("max")) {
            return false;
        }
        else {
            return true;
        }
    }

    function MatchOptionsUpdate(json, $games) {
        // Verify if the users can add more games
        if (CanAddGames($games)) {
            $games.find(".options .add-game").removeClass("hide");
        }
        else {
            $games.find(".options .add-game").addClass("hide");
        }

        // Verify the match is finished
        if (json.finished) {
            $games.find(".update-games").addClass("hide");
        }
        else {
            $games.find(".update-games").removeClass("hide");
        }
    }

    function UpdateStandings(tournyId, bracket) {
        jsonData = {
            "tournamentId": tournyId,
            "bracketNum": bracket
        };

        $.ajax({
            "url": "/Ajax/Bracket/Standings",
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
                        html += "<li class='column rank'>" + e.Rank + "</li>";
                        html += "<li class='column name'>" + e.Name + "</li>";
                        if (e.Score != -1) html += "<li class='column score'>" + (e.Score > -1 ? e.Score : "") + "</li>";
                        html += "</ul>";

                        standings.append(html);
                    });
                }
            },
            "error": function (json) {
                console.log(json);
            },
            "complete": function () {

            }
        });
    }
});