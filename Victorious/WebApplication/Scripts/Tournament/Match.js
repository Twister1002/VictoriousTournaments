jQuery(document).ready(function () {
    var $ = jQuery;

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
            "before": function() {
                // Clear out the match data
                json = {
                    "matchId": -1,
                    "matchNum": -1,
                    "challenger": {
                        "seed": -1,
                        "name": "N/A",
                        "score": -1,
                        "id": -1
                    },
                    "defender": {
                        "seed": -1,
                        "name": "N/A",
                        "score": -1,
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

        json = {
            "tournyId": $("#Tournament").data("id"),
            "bracketNum": $(this).closest(".bracket").data("bracketnum"),
            "matchId": matchData.data("matchid"),
            "matchNum": matchData.data("matchnum"),
            "winnerId": matchData.find(".selected-winner").data("userid"),
            "winner": matchData.find(".selected-winner").hasClass("defender") ? "Defender" : "Challenger",
            "challengerScore": -1,
            "defenderScore": -1
        };

        $.ajax({
            "url": "/Match/Ajax/Update",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(json) },
            "dataType": "json",
            "success": function (json) {
                json = JSON.parse(json);
                if (json.status) {
                    // Find the next winner match


                    // Find the next loser match


                    
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
        matchElem.data("matchid", json.matchId);
        matchElem.data("matchnum", json.matchNum);
        matchElem.find(".matchNum").html(json.matchNum);

        // Defender Data
        defender.data("userid", json.defender.id);
        defender.data("seed", json.defender.seed);
        defender.find(".name", defender).text(json.defender.name);
        defender.find(".match-score", defender).text(json.defender.seed);

        // Challenger Data
        challenger.data("userid", json.challenger.id);
        challenger.data("seed", json.challenger.seed);
        challenger.find(".name", challenger).text(json.challenger.name);
        challenger.find(".match-score", challenger).text(json.challenger.seed);
    }

    // Mouse Events
    $(".matchData .info li").on("mouseover", function () {
        //console.log("Entered: " + $(this).data("seed"));
        var seed = $(this).data("seed");
        if (seed > -1) {
            $(".matchData .info [data-seed='" + seed + "']").addClass("seedHover");
        }
    });
    $(".matchData .info li").on("mouseleave", function () {
        //console.log("Left: " + $(this).data("seed"));
        var seed = $(this).data("seed");
        if (seed > -1) {
            $(".matchData .info [data-seed='" + seed + "']").removeClass("seedHover");
        }
    });
});