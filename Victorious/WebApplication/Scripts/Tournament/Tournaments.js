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


    // Finalize Tournament 
    $(".tournamentFinalizeButton").on("click", function () {
        var roundData = {};
        var jsonData = {
            "tournyVal": $("#Tournament").data("id"),
            "bracketVal": $(".bracket").data("bracketnum"),
        };

        $.each($(".header-rounds li"), function (i, e) {
            roundData[i + 1] = $(e).find(".bestOfMatches").val();
        });

        $.ajax({
            "url": "/Tournament/Ajax/Finalize",
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