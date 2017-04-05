jQuery(document).ready(function () {
    var $ = jQuery;

    var PlayerSlot = {
        "-1": "unspecified",
        "0": "defender",
        "1": "challenger"
    }

    // Match Administration
    $(".match-edit-module .matchData .info li").on("click", function () {
        // Remove the class
        $(this).siblings().removeClass("selected-winner");
        $(this).addClass("selected-winner");
    });

    $(".match-edit-module .module-close .close").on("click", function () {
        $(".match-edit-module").removeClass("open");
    });

    $(".match .matchHeader .edit").on("click", function () {
        var matchElem = $(this).closest(".match");
        var bracketNum = $(matchElem).closest(".bracket").data("bracketnum");
        var matchNum = $(matchElem, ".match").data("matchnum");
        var tournamentId = $("#Tournament").data("id");
        
        $.ajax({
            "url": "/Match/Ajax/Match",
            "type": "POST",
            "data": { "match": matchNum, "bracket":bracketNum, "tourny": tournamentId },
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
        //$(this).attr("disable", true)
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
            "success": function (json) {
                json = JSON.parse(json);
                if (json.status) {
                    // Update the current match data
                    currMatch = $(".list-table-body .match[data-matchnum='" + json.data.matchNum + "']");
                    currMatch.find(".challenger .match-score").text(json.data.challenger.score);
                    currMatch.find(".defender .match-score").text(json.data.defender.score);

                    // Move the Defender
                    if (json.data.defender.nextRound != -1) {
                        nextMatch = $(".list-table-body .match[data-matchnum='" + json.data.defender.nextRound + "']");
                        slot = nextMatch.find("." + PlayerSlot[json.data.defender.slot]);
                        slot.attr("data-userid", json.data.defender.id).data("userid", json.data.defender.id);
                        slot.find(".name").text(json.data.defender.name);

                        if (nextMatch.find(".defender").data("userId") != -1 && nextMatch.find(".challenger").data("userid") != -1) {
                            nextMatch.find(".edit").removeClass("hide");
                        }
                        else {
                            nextMatch.find(".edit").addClass("hide");
                        }
                    }

                    // Find the next loser match
                    if (json.data.challenger.nextRound != -1) {
                        nextMatch = $(".list-table-body .match[data-matchnum='" + json.data.challenger.nextRound + "']");
                        slot = nextMatch.find("." + PlayerSlot[json.data.challenger.slot]);
                        slot.attr("data-userid", json.data.challenger.id).data("userid", json.data.challenger.id);
                        slot.find(".name").text(json.data.challenger.name);

                        if (nextMatch.find(".defender").data("userId") != -1 && nextMatch.find(".challenger").data("userid") != -1) {
                            nextMatch.find(".edit").removeClass("hide");
                        }
                        else {
                            nextMatch.find(".edit").addClass("hide");
                        }
                    }

                    // Remove the edit button
                    $(".list-table-body .match[data-matchnum='" + jsonData.matchNum + "'] .matchHeader .edit").addClass("hide");
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
                $(".match-edit-module").removeClass("open");
            }
        });
    });

    function SetMatchData(json) {
        var matchElem = $(".module-content .match");
        var challenger = $(matchElem).find(".matchData .info .challenger");
        var defender = $(matchElem).find(".matchData .info .defender");

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

    // Mouse Events
    $(".matchData .info li").on("mouseover", function () {
        //console.log("Entered: " + $(this).data("seed"));
        var userid = $(this).data("userid");
        if (userid > -1) {
            $(".matchData .info [data-userid='" + userid + "']").addClass("teamHover");
        }
    });
    $(".matchData .info li").on("mouseleave", function () {
        //console.log("Left: " + $(this).data("seed"));
        var userid = $(this).data("userid");
        if (userid > -1) {
            $(".matchData .info [data-userid='" + userid + "']").removeClass("teamHover");
        }
    });

    // Set edit icons
    (function($){
        matches = $(".list-table-body .match ");
        $.each(matches, function (i, e) {
            challenger = $(e).find(".challenger");
            defender = $(e).find(".defender");

            if (challenger.data("userid") > 0 && defender.data("userid") > 0) {
                // Enable the button

            }
        });
    })($)
});