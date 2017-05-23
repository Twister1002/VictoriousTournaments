jQuery(document).ready(function () {
    var $ = jQuery;

    $(".datepicker").datepicker().prop("readonly", true);
    $(".timepicker").prop("readonly", false).timepicker({
        "className": "timepicker",
        "disableTextInput": true,
        "disableTouchKeyboard": true,
        "step": 15,
        "timeFormat": "g:i a"
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