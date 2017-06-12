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
    $(".TournamentMatch .defender, .TournamentMatch .challenger").on("mouseover", MouseOverEvents);
    $(".TournamentMatch .defender, .TournamentMatch .challenger").on("mouseleave", MouseLeaveEvents);
    $(".TournamentGames .removeGame").on("click", RemoveGame);

    function MouseOverEvents() {
        //console.log("Entered: " + $(this).data("seed"));
        var userid = $(this).data("id");
        if (userid > -1) {
            $(".TournamentMatch .defender[data-id='" + userid + "'], .TournamentMatch .challenger[data-id='" + userid + "']").addClass("teamHover");
        }
    }

    function MouseLeaveEvents() {
        //console.log("Left: " + $(this).data("seed"));
        var userid = $(this).data("id");
        if (userid > -1) {
            $(".TournamentMatch .defender[data-id='" + userid + "'], .TournamentMatch .challenger[data-id='" + userid + "']").removeClass("teamHover");
        }
    }

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
            "id": -1,
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
            "tournamentId": $("#Tournament").data("id"),
            "bracketId": matchElem.closest(".bracket").data("id"),
            "matchId": matchElem.data("id")
        };

        $.ajax({
            "url": "/Ajax/Match",
            "type": "POST",
            "data": jsonData,
            "dataType": "json",
            "beforeSend": function () {
                $(".TournamentInfo, .TournamentGames").removeClass("open");
                matchElem.find(".TournamentGames").addClass("open")
                matchElem.find(".TournamentGames .list-table-body").empty();
            },
            "success": function (json) {
                if (json.status) {
                    console.log(json);
                    $games = matchElem.find(".TournamentGames");

                    MatchUpdate(json.data.match, matchElem);
                    MatchOptionsUpdate(json.data.match, $games);
                }
                else {
                    console.log(json.message);
                }
            },
            "error": function (json) {
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

        // Get all the game data
        $.each(games, function (i, e) {
            gameData.push({
                "GameNumber": $(e).data("gamenum"),
                "DefenderScore": $(e).find(".defender-score").val(),
                "ChallengerScore": $(e).find(".challenger-score").val()
            });
        });

        var jsonData = {
            "matchId": match.data("id"),
            "matchNum": match.data("matchnum"),
            "bracketId": $(this).closest(".bracket").data("id"),
            "tournamentId": $("#Tournament").data("id"),
            "games": gameData
        }

        $.ajax({
            "url": "/Ajax/Match/Update",
            "type": "POST",
            "data": jsonData,
            "dataType": "json",
            "beforeSend": function () {
                match.find(".TournamentGames .update-games").attr("disabled", true);
            },
            "success": function (json) {
                if (json.status) {
                    console.log(json.message);
                    var matchElement = null;

                    // Check if there is a refresh
                    if (json.data.refresh) {
                        window.location.reload();
                        return;
                    }

                    $.each(json.data.matchUpdates, function (i, e) {
                        if (match.data("id") == e.matchId) {
                            if (e.finished) {
                                match.find(".TournamentGames").removeClass("open");
                            }
                        }

                        matchElement = $(".TournamentMatch[data-id='" + e.matchId + "']");
                        MatchUpdate(e, matchElement);
                        MatchOptionsUpdate(e, matchElement.find(".TournamentGames"));
                    });

                    UpdateStandings(jsonData.tournamentId, jsonData.bracketId);
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
            "data": { "tournamentId": $("#Tournament").data("id"), "bracketId": $(this).closest(".bracket").data("id"), "matchNum": match.data("matchnum") },
            "dataType": "json",
            "success": function (json) {
                if (json.status) {
                    $.each(json.data, function (i, e) {
                        MatchUpdate(e, $(".TournamentMatch[data-id='" + e.matchId + "']"));

                        if (e.matchNum == match.data("matchnum")) {
                            MatchOptionsUpdate(e, match.find(".TournamentGames"));
                        }
                    });
                    
                    match.find(".TournamentGames .games").empty();
                    UpdateStandings($("#Tournament").data("id"), match.closest(".bracket").data("id"));
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
        games.find(".defender.name").text(json.defender.name);
        games.find(".challenger.name").text(json.challenger.name);

        games.find(".games").empty();
        $.each(json.games, function (i, e) {
            AddGameToDetails(e, games);
        });

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
        html = "<ul class='form gameDetail' data-columns='4' data-gameid='" + data.id + "' data-gamenum='" + data.gameNum + "'> ";
        html += "<li class='column game-number'>" + data.gameNum + "</li> ";
        html += "<li class='column defender score' data-id='"+data.defender.id+"'><input type='text' class='defender-score' name='defender-score' maxlength='3' value='" + data.defender.score + "' /></li> ";
        html += "<li class='column challenger score' data-id='" + data.challenger.id + "'><input type='text' class='challenger-score' name='challenger-score' maxlength='3' value='" + data.challenger.score + "' /></li> ";
        html += "<li class='column'><span class='icon icon-bin removeGame'></span></li> ";
        html += "</ul> ";

        $games.find(".games").append(html);

        // Register the hover events
        $(".TournamentMatch .defender, .TournamentMatch .challenger").off("mouseover").on("mouseover", MouseOverEvents);
        $(".TournamentMatch .defender, .TournamentMatch .challenger").off("mouseleave").on("mouseleave", MouseLeaveEvents);
        $(".TournamentMatch .removeGame").off("click").on("click", RemoveGame);
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
            $games.find(".options .add-game").addClass("hide");
        }
        else {
            $games.find(".update-games").removeClass("hide");
        }
    }

    function RemoveGame() {
        var $game = $(this).closest("ul");

        var jsonData = {
            "tournamentId": $("#Tournament").data("id"),
            "bracketId": $game.closest(".bracket").data("id"),
            "matchId": $game.closest(".TournamentMatch").data("id"),
            "matchNum": $game.closest(".TournamentMatch").data("matchnum"),
            "gameId": $game.data("gameid"),
            "gameNum": $game.data("gamenum"),
        };

        $.ajax({
            "url": "/Ajax/Match/RemoveGame",
            "type": "post",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function () {

            },
            "success": function (json) {
                console.log(json);

                if (json.status) {
                    UpdateStandings($("#Tournament").data("id"), $game.closest(".bracket").data("id"));

                    $.each(json.data, function (i, e) {
                        $match = $(".TournamentMatch[data-id='" + e.matchId + "']");
                        MatchUpdate(e, $match);
                        MatchOptionsUpdate(e, $match);

                        // Update the games
                        $games = $match.find(".TournamentGames");
                        $games.find(".games").empty();
                        $.each(e.games, function (ii, ee) {
                            AddGameToDetails(ee, $games);
                        });
                    });
                }
                else {
                    console.log(json.message);
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