jQuery(document).ready(function () {
    $(".group-data .groupName").on("click", function () {
        $(this).closest("ul").find(".groupName").removeClass("selected");
        $(this)
            .addClass("selected")
            .closest(".list-table-body")
            .find(".round[data-groupnum='" + $(this).data("groupnum") + "']").removeClass("hide")
            .siblings().addClass("hide");
    });

    // Finalize Tournament 
    $(".bracketFinalizeButton").on("click", function () {
        var bracket = $(this).closest(".bracket");

        var jsonData = {
            "tournamentId": $("#Tournament").data("id"),
            "bracketId": bracket.data("id"),
            "roundData": {}
        };

        $.each(bracket.find(".header-rounds"), function (i, e) {
            var roundNum = 1;

            $.each($(e).find("li"), function (ii, ee) {
                var type = $(ee).data("type");
                var element = $(ee).find(".bestOfMatches");

                if (element.length) {
                    if (!jsonData.roundData.hasOwnProperty(type)) {
                        jsonData.roundData[type] = {};
                    }
                    jsonData.roundData[type][roundNum] = element.val();
                    roundNum++;
                }
            });
        });

        $.ajax({
            "url": "/Ajax/Tournament/Finalize",
            "type": "POST",
            "data": jsonData,
            "dataType": "json",
            "success": function (json) {
                location.reload();
                console.log(json);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });

    $(".bracketLockButton").on("click", function () {
        var bracket = $(this).closest(".bracket");

        var jsonData = {
            "tournamentId": $("#Tournament").data("id"),
            "bracketId": bracket.data("id")
        };

        $.ajax({
            "url": "/Ajax/Bracket/Lockout",
            "type": "POST",
            "data": jsonData,
            "dataType": "json",
            "success": function (json) {
                location.reload();
                console.log(json);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });
});