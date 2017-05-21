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
});