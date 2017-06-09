jQuery(document).ready(function () {
    var $ = jQuery;
    var $platform = $("#AdministratorPlatforms");
    
    $("#AdministratorPlatforms .platform .removePlatform").on("click", RemovePlatform);
    $("#AdministratorPlatforms .options .AddPlatform").on("click", AddPlatform);
    $("#AdministratorPlatforms .options .platformTitle").on("keydown", function (e) {
        if (e && e.keyCode == 13) { // Enter
            AddPlatform();
        }
    });

    function AddPlatform() {
        var jsonData = {
            "Platform": $platform.find(".options .platformTitle").val(),
            "action": "add"
        };

        PlatformChange(jsonData);
    }

    function RemovePlatform() {
        var jsonData = {
            "PlatformId": $(this).closest("ul").data("id"),
            "action": "delete"
        };

        PlatformChange(jsonData);
    }

    function PlatformChange(jsonData) {
        $.ajax({
            "url": "/Ajax/Administrator/Platform",
            "type": "post",
            "data": jsonData,
            "dataType": "json",
            "beforeSend": function () {
                $platform.find(".options .AddPlatform").attr("disabled", true);
                $platform.find(".options .platformTitle").attr("disabled", true);
            },
            "success": function (json) {
                json = JSON.parse(json);
                console.log(json);

                var $table = $("#AdministratorPlatforms .list-table-body");
                $table.empty();

                $.each(json.platforms, function (i, e) {
                    html = "<ul class='platform' data-columns='2' data-id='" + e.PlatformID + "'>";
                    html += "<li class='column'>" + e.PlatformName + "</li>";
                    html += "<li class='column'><span class='icon icon-cross removePlatform'></span></li>";
                    html += "</ul>";

                    $table.append(html);
                });

                $platform.find(".options .platformTitle").val('');

                // Give everyone the event
                $table.find(".removePlatform").off("click").on("click", RemovePlatform);
            },
            "error": function (json) {
                console.log(json);
            },
            "complete": function () {
                $platform.find(".options .AddPlatform").attr("disabled", false);
                $platform.find(".options .platformTitle").attr("disabled", false);
            }
        });
    }
});