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
    
    // Update the Date selections
    $("#RegistrationStartDate").on("change", function () {
        $("#RegistrationEndDate").datepicker("option", "minDate", new Date($(this).val()));
        $("#TournamentStartDate").datepicker("option", "minDate", new Date($(this).val()));
        $("#TournamentEndDate").datepicker("option", "minDate", new Date($(this).val()));
    });

    $("#RegistrationEndDate").on("change", function () {
        $("#TournamentStartDate").datepicker("option", "minDate", new Date($(this).val()));
        $("#TournamentEndDate").datepicker("option", "minDate", new Date($(this).val()));
    });

    $("#TournamentStartDate").on("change", function () {
        $("#TournamentEndDate").datepicker("option", "minDate", new Date($(this).val()));
    });
});