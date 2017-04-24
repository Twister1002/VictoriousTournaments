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

    $(".match-edit-module .options .close").on("click", function () {
        $(this).closest(".match-edit-module").removeClass("open");
    });

    // When the tournament is finalized
    $(".TournamentMatch .options .edit").on("click", function () {
        var matchElem = $(this).closest(".TournamentMatch");
        var jsonData = {
            "matchId": $(matchElem).data("id"),
            "matchNum": $(matchElem).data("matchnum"),
            "bracketNum": $(matchElem).closest(".bracket").data("bracketnum"),
            "tournamentId": $("#Tournament").data("id")
        };
        
        $.ajax({
            "url": "/Match/Ajax/Match",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function() {
                // Clear out the match data
                json = {
                    "matchId": -1,
                    "matchNum": -1,
                    "challenger": {
                        "seed": -1,
                        "name": "N/A",
                        "score": 0,
                        "id": -1
                    },
                    "defender": {
                        "seed": -1,
                        "name": "N/A",
                        "score": 0,
                        "id": -1
                    }
                }

                SetMatchData(json);
            },
            "success": function (json) {
                json = JSON.parse(json);
                $(".match-edit-module").addClass("open");
                if (json.status) {
                    SetMatchData(json.data);
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

    

    $(".match-edit-module .match-submit button").on("click", function () {
        var matchData = $(".match-edit-module .module-content .match")

        jsonData = {
            "tournyId": $("#Tournament").data("id"),
            "bracketNum": $(this).closest(".bracket").data("bracketnum"),
            "matchId": matchData.data("matchid"),
            "matchNum": matchData.data("matchnum"),
            "winnerId": matchData.find(".selected-winner").data("userid"),
            "winner": matchData.find(".selected-winner").hasClass("defender") ? "Defender" : "Challenger",
            "challengerScore": matchData.find(".challenger .score-edit").val(),
            "defenderScore": matchData.find(".defender .score-edit").val()
        };

        $.ajax({
            "url": "/Match/Ajax/Update",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function () {
                // Disable the button
                $(".match-edit-module .match-submit button").attr("disabled", true);
            },
            "success": function (json) {
                json = JSON.parse(json);
                if (json.status) {

                    if (json.data.currentMatch) {
                        var matchData = json.data.currentMatch;
                        var match = $(".list-table-body .match[data-matchnum='" + matchData.matchNum + "']");

                        $(match).find(".challenger .match-score").text(matchData.challenger.score);
                        $(match).find(".defender .match-score").text(matchData.defender.score);

                        if (matchData.isFinished) {
                            match.find(".edit").addClass("hide");
                        }
                        else {
                            match.find(".edit").removeClass("hide");
                        }
                    }
                    if (json.data.nextWinnerMatch) {
                        var matchData = json.data.nextWinnerMatch;
                        var match = $(".list-table-body .match[data-matchnum='" + matchData.matchNum + "']");

                        // Set the names
                        $(match)
                            .find(".challenger")
                            .attr("data-userid", matchData.challenger.id)
                            .data("userid", matchData.challenger.id)
                            .find(".name").text(matchData.challenger.name);
                        
                        $(match)
                            .find(".defender")
                            .attr("data-userid", matchData.defender.id)
                            .data("userid", matchData.defender.id)
                            .find(".name").text(matchData.defender.name);

                        // Set the scores
                        $(match).find(".challenger .match-score").text(matchData.challenger.score);
                        $(match).find(".defender .match-score").text(matchData.defender.score);

                        if (matchData.isReady) {
                            match.find(".edit").removeClass("hide");
                        }
                        else {
                            match.find(".edit").addClass("hide");
                        }
                    }
                    if (json.data.nextLoserMatch) {
                        var matchData = json.data.nextLoserMatch;
                        var match = $(".list-table-body .match[data-matchnum='" + matchData.matchNum + "']");

                        // Set the names
                        (match)
                            .find(".challenger")
                            .attr("data-userid", matchData.challenger.id)
                            .data("userid", matchData.challenger.id)
                            .find(".name").text(matchData.challenger.name);

                        $(match)
                            .find(".defender")
                            .attr("data-userid", matchData.defender.id)
                            .data("userid", matchData.defender.id)
                            .find(".name").text(matchData.defender.name);

                        // Set the scores
                        $(match).find(".challenger .match-score").text(matchData.challenger.score);
                        $(match).find(".defender .match-score").text(matchData.defender.score);

                        if (matchData.isReady) {
                            match.find(".edit").removeClass("hide");
                        }
                        else {
                            match.find(".edit").addClass("hide");
                        }
                    }
                }
                else {

                }
                console.log("Success");
                console.log(json);
            },
            "error": function (json) {
                console.log("error");
                console.log(json);
            },
            "complete": function () {
                // Remove the disabled button
                $(".match-edit-module .match-submit button").attr("disabled", false);

                // Close the module.
                $(".match-edit-module").removeClass("open");

                // Update the standings
                UpdateStandings(jsonData.tournyId, jsonData.bracketNum);
            }
        });
    });

    function SetMatchData(json) {
        var matchElem = $(".module-content .TournamentMatch");
        var challenger = $(matchElem).find(".challenger");
        var defender = $(matchElem).find(".defender");

        // Match Data
        matchElem.attr("data-matchid", json.matchId).data("matchid", json.matchId);
        matchElem.attr("data-matchnum", json.matchNum).data("matchnum", json.matchNum);
        matchElem.find(".matchNum").html(json.matchNum);

        // Defender Data
        defender.attr("data-userid", json.defender.id).data("userid", json.defender.id);
        defender.find(".name").text(json.defender.name);
        defender.find(".score-edit").val(json.defender.score);

        // Challenger Data
        challenger.attr("data-userid", json.challenger.id).data("userid", json.challenger.id);
        challenger.find(".name").text(json.challenger.name);
        challenger.find(".score-edit").val(json.challenger.score);
    }

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
                        html += "<li class='position-rank'>"+e.Rank+"</li>";
                        html += "<li class='position-name'>"+e.Name+"</li>";
                        if (e.score) html += "<li class='position-score'>"+e.Score+"</li>";
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