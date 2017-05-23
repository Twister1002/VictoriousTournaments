jQuery(document).ready(function () {
    var $ = jQuery;

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
            "data": { "tournamentId": $("#Tournament").data("id"), "tournamentUserId": $(this).closest(".data").data("userid") },
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
});