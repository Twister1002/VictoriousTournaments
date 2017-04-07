jQuery(document).ready(function () {
    var $ = jQuery;

    $(".GameAddButton").on("click", function () {
        var jsonData = {
            "type": "add"
        };

        $.ajax({
            "url": "/Administrator/Ajax/Games",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "success": function (json) {
                console.log("success");
                console.log(json)
            },
            "error": function (json) {
                console.log("Failure");
                console.log(json);
            }
        });
    });
});