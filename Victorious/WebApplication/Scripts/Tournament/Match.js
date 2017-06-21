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
        var match = $(this).closest(".TournamentMatch");
        var gameList = match.find(".TournamentGames");
        var maxGames = gameList.data("max");
        var gamesListed = gameList.find(".list-table-body ul").length;
        var nextGameNumber = $(gameList.find(".games ul")[gamesListed - 1]).data("gamenum") + 1;

        jsonData = {
            "id": -1,
            "gameNum": (isNaN(nextGameNumber) ? 1 : nextGameNumber),
            "challenger": { "score": "" },
            "defender": { "score": "" }
        };

        AddGame(jsonData, match);
        UpdateMatchOptions(false, match);
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
                    $games = matchElem.find(".TournamentGames");

                    UpdateMatch(json.data, matchElem);
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
                    var matchElement = null;

                    // Check if there is a refresh
                    if (json.data.refresh) {
                        window.location.reload();
                        return;
                    }
                    
                    UpdateMatch(json.data, match);
                    UpdateStandings(jsonData.tournamentId, jsonData.bracketId);
                }

                console.log(json.message);
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
        if (!confirm("Are you sure you want to reset this match? It could affect future matches.")) {
            return;
        }

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
                    UpdateMatch(json.data, match);
                    UpdateStandings($("#Tournament").data("id"), match.closest(".bracket").data("id"));
                }

                console.log(json.message);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });

    function RemoveGame() {
        if (!confirm("Are you sure you want to delete this game? It could affect future matches.")) {
            return;
        }

        var match = $(this).closest(".TournamentMatch");
        var $game = $(this).closest("ul");

        var jsonData = {
            "tournamentId": $("#Tournament").data("id"),
            "bracketId": $game.closest(".bracket").data("id"),
            //"matchId": $game.closest(".TournamentMatch").data("id"),
            "matchNum": $game.closest(".TournamentMatch").data("matchnum"),
            //"gameId": $game.data("gameid"),
            "gameNum": $game.data("gamenum")
        };

        $.ajax({
            "url": "/Ajax/Match/RemoveGame",
            "type": "post",
            "data": jsonData,
            "dataType": "json",
            "success": function (json) {
                if (json.status) {
                    UpdateMatch(json.data, match.closest(".TournamentMatch"));
                    UpdateStandings($("#Tournament").data("id"), $game.closest(".bracket").data("id"));
                }

                console.log(json.message);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    }

    // Helper functions
    function UpdateMatch(matchData, $match) {
        console.log(matchData);

        UpdateTournamentOptions(matchData, $match);

        // Update the overview of the match
        $.each(matchData.matches, function (i, data) {
            // Allow users to click no details if the match is ready
            if (data.ready) {
                $match.find(".details").removeClass("hide");
            }
            else {
                $match.find(".details").addClass("hide");
            }

            $match.find(".defender .name").text(data.defender.name);
            $match.find(".defender .score").text(data.defender.score);
            $match.find(".defender").attr("data-id", data.defender.id).data("id", data.defender.id);

            $match.find(".challenger .name").text(data.challenger.name);
            $match.find(".challenger .score").text(data.challenger.score);
            $match.find(".challenger").attr("data-id", data.challenger.id).data("id", data.challenger.id);

            // Update the Game data 
            $match.find(".TournamentGames .defender.name").text(data.defender.name);
            $match.find(".TournamentGames .challenger.name").text(data.challenger.name);

            // Add the games to the match
            UpdateGamesFromMatch(data, $match);
            UpdateMatchOptions(matchData, $match);
        });

        // Register new events
        $(".TournamentMatch .defender, .TournamentMatch .challenger").off("mouseover").on("mouseover", MouseOverEvents);
        $(".TournamentMatch .defender, .TournamentMatch .challenger").off("mouseleave").on("mouseleave", MouseLeaveEvents);
        $(".TournamentMatch .removeGame").off("click").on("click", RemoveGame);
    }

    function UpdateGamesFromMatch(matchInfo, $match) {
        // Remove all the games
        $(".TournamentGames .list-table .games").empty();

        $.each(matchInfo.games, function (i, e) {
            // Add the game
            html = "<ul class='form gameDetail' data-columns='4' data-gameid='" + e.id + "' data-gamenum='" + e.gameNum + "'> ";
            html += "<li class='column game-number'>" + e.gameNum + "</li> ";
            html += "<li class='column defender score' data-id='" + e.defender.id + "'><input type='text' class='defender-score' name='defender-score' maxlength='3' value='" + e.defender.score + "' /></li> ";
            html += "<li class='column challenger score' data-id='" + e.challenger.id + "'><input type='text' class='challenger-score' name='challenger-score' maxlength='3' value='" + e.challenger.score + "' /></li> ";
            html += "<li class='column'><span class='icon icon-bin removeGame'></span></li> ";
            html += "</ul> ";

            $(".TournamentGames .list-table .games").append(html);
        });
    }

    function AddGame(data, $match) {
        html = "<ul class='form gameDetail' data-columns='4' data-gameid='" + data.id + "' data-gamenum='" + data.gameNum + "'> ";
        html += "<li class='column game-number'>" + data.gameNum + "</li> ";
        html += "<li class='column defender score' data-id='" + data.defender.id + "'><input type='text' class='defender-score' name='defender-score' maxlength='3' value='" + data.defender.score + "' /></li> ";
        html += "<li class='column challenger score' data-id='" + data.challenger.id + "'><input type='text' class='challenger-score' name='challenger-score' maxlength='3' value='" + data.challenger.score + "' /></li> ";
        html += "<li class='column'><span class='icon icon-bin removeGame'></span></li> ";
        html += "</ul> ";

        $(".TournamentGames .list-table .games").append(html);
    }

    function UpdateMatchOptions(matchData, $match) {
        // Can the user add more games?
        if ($match.find(".games ul").length >= $match.find(".games").data("max")) {
            $match.find(".TournamentGames .options .add-game").addClass("hide");
        }
        else {
            $match.find(".TournamentGames .options .add-game").removeClass("hide");
        }

        if (!matchData.isLocked) {
            $match.find(".update-games").removeClass("hide");
        }
        else {
            // Remvoe the update button and the game deletion button
            $match.find(".update-games, .TournamentGames .games .removeGame").remove();
        }
    }

    function UpdateTournamentOptions(jsonData, $inBracket) {
        if (jsonData.bracketFinished) {
            $inBracket.closest(".bracket").find(".options .BracketLock").removeClass("hide");
        }
        else {
            $inBracket.closest(".bracket").find(".options .BracketLock").addClass("hide");
        }
    }
});