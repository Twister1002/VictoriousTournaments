jQuery(document).ready(function () {
    // Reset the brackets
    $(".bracket-info .options .tournament-reset").on("click", function () {
        var bracketId = $(this).closest(".bracket").data("id");

        if (confirm("Are you sure you want to reset this bracket?")) {
            $.ajax({
                "url": "/Ajax/Bracket/Reset",
                "type": "POST",
                "data": { "bracketId": bracketId },
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
                    console.log("Error");
                }
            });
        }
        else {
            return false;
        }
    });
});