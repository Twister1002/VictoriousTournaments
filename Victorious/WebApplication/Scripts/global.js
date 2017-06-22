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

    $("#content .message .closeMessage").on("click", function () {
        $("#content .message").remove();
    });


    function Ajax(path, data, beforeSend, success, error, complete) {
        $.ajax({
            "url": path,
            "type": "POST",
            "data": data,
            "dataType": "json",
            "beforeSend": beforeSend,
            "success": success,
            "error": error,
            "complete": complete
        });
    }
});