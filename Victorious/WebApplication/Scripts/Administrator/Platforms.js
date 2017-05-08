jQuery(document).ready(function () {
    var $ = jQuery;
    var $platform = $(".#AdministratorPlatforms");
    
    $platform.find(".options .platform").on("keydown", function () {
        if (e.keyCode == 13) { // Enter
            AddPlatform();
        }
    });
    $platform.find(".options .AddPlatform").on("click", AddPlatform());
    $platform.find(".list-table-body .platforms .removePlatform").on("click",  RemovePlatform(this));

    function AddPlatform() {
        var jsonData = {
            "Platform": $platform.find(".options .platform"),
            "action": "add"
        };

        PlatformChange(jsonData);
    }

    function RemovePlatform(elem) {
        var jsonData = {
            "PlatformId": $(elem).closest("ul").data("id"),
            "action": "delete"
        };

        PlatformChange(jsonData);
    }

    function PlatformChange(jsonData) {
        $.ajax({
            "url": "/Ajax/Administrator/Platforms",
            "type": "post",
            "data": jsonData,
            "dataType": "json",
            "beforeSend": function () {
                $platform.find(".options .AddPlatform").attr("disabled", true);
                $platform.find(".options .platform").attr("disabled", true);
            },
            "success": function (json) {
                json = JSON.parse(json);
                console.log(json);
            },
            "error": function (json) {
                console.log(json);
            },
            "complete": function () {
                $platform.find(".options .AddPlatform").attr("disabled", false);
                $platform.find(".options .platform").attr("disabled", false);
            }
        })
    }
});