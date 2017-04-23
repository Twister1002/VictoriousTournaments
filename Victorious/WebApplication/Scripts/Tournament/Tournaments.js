jQuery(document).ready(function () {
    var $ = jQuery;

    // Tournament Deletion
    $(".tournament-delete").on("click", function () {
        var jsonData = {
            "tourny": $(this).closest("#Tournament").data("id")
        };

        if (confirm("Are you sure you want to delete this tournament? This can no be reverted.")) {
            $.ajax({
                "url": "/Tournament/Ajax/Delete",
                "type": "POST",
                "data": { "jsonData": JSON.stringify(jsonData) },
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

    // Redirect to update
    $(".tournament-update").on("click", function () {
        window.location.replace("/Tournament/Update/" + $(this).closest("#Tournament").data("id"));
    });

    // Reset the brackets
    $(".tournament-reset").on("click", function () {
        var jsonData = {
            "tournyNum": $(this).closest("#Tournament").data("id")
        };

        if (confirm("Are you sure you want to reset non-finished brackets?")) {
            $.ajax({
                "url": "/Tournament/Ajax/Reset",
                "type": "POST",
                "data": { "jsonData": JSON.stringify(jsonData) },
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

                    console.log("Error");
                }
            });
        }
        else {
            return false;
        }
    });

    // Finalize Tournament 
    $(".tournamentFinalizeButton").on("click", function () {
        var jsonData = {
            "tournyVal": $("#Tournament").data("id")+"",
            "roundData": new Array()
        };

        $.each($(".header-rounds li"), function (i, e) {
            //data = {};
            //data[i+1] = $(e).find(".bestOfMatches").val();

            //jsonData.roundData.push(data);

            jsonData.roundData.push($(e).find(".bestOfMatches").val());
        });

        $.ajax.tr
        $.ajax({
            "url": "/Tournament/Ajax/Finalize",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "success": function (json) {
                json = JSON.parse(json);
                console.log(json);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });

    // View tournament standings
    $(".tournament-standings").on("click", function () {
        var elem = $("#TournamentStandings");

        if (elem.hasClass("open")) {
            // Close the side panel
            elem.removeClass("open");
        }
        else {
            // Open the side panel
            elem.addClass("open");
        }
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

    (function ($) {
        if ($("#RegistrationStartDate").length > 0) {
            $("#RegistrationStartDate")
                .datepicker("setDate", $(this).val())
                //.datepicker("option", "minDate", "-1m")
                .trigger("change");
        }
    })($);
});