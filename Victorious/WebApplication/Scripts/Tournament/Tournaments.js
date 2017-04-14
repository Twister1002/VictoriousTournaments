jQuery(document).ready(function () {
    var $ = jQuery;

    // Tournament Deletion
    $(".tournament-delete").on("click", function () {
        if (confirm("Are you sure you want to delete this tournament? This can no be reverted.")) {
            $.ajax({
                "url": "/Tournament/Ajax/Delete",
                "type": "POST",
                "data": { "tourny": $(this).data("id") },
                "dataType": "json",
                "success": function (json) {
                    if (json.success) {
                        window.location.replace(json.redirect);
                    }
                    else {
                        alert(json.message);
                    }
                },
                "error": function (json) {
                    alert(json.message);
                }
            });
        }
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
        $("#RegistrationStartDate")
            .datepicker("setDate", $(this).val())
            //.datepicker("option", "minDate", "-1m")
            .trigger("change");
    })($);
});