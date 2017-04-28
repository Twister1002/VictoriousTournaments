jQuery(document).ready(function () {
    var $ = jQuery;
    var section = $("#AdministratorGames");

    $("#AdministratorGames .gameDelete").on("click", deleteEvent);
    $("#AdministratorGames .GameAddButton").on("click", function () {
        var jsonData = {
            "function": "add",
            "title": $("#AdministratorGames .form .gameTitle .field").val()
        };

        GameUpdate(jsonData);
    });

    function deleteEvent() {
        var jsonData = {
            "function": "delete",
            "title": $(this).siblings(".gameTitle").text(),
            "gameid": $(this).closest(".game").data("gameid")
        };

        GameUpdate(jsonData);
    }

    function GameUpdate(jsonData) {
        $.ajax({
            "url": "/Administrator/Ajax/Games",
            "type": "POST",
            "data": { "jsonData": JSON.stringify(jsonData) },
            "dataType": "json",
            "beforeSend": function() {
                $("#AdministratorGames .gameDelete").off("click");
                $("#AdministratorGames .GameAddButton").attr("disabled", true);
            },
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    var table = $("#AdministratorGames .list-table-body");

                    if (json.function == "add") {
                        html = "<ul class='game' data-columns='3' data-gameid='" + json.data.model.GameID + "'>";
                        html += "<li class='column gameTitle'>" + json.data.model.Title + "</li>";
                        html += "<li class='column gamePlatforms'>None</li>";
                        html += "<li class='column gameDelete'><span class='icon icon-cross'></span></li>";
                        html += "</ul>";

                        table.append(html);

                        $(".gameTitle input").val('');
                    }
                    else if (json.function == "delete") {
                        table.find(".game[data-gameid='" + json.data.model.GameID + "']").remove();
                    }
                }

                console.log(json.message);
            },
            "error": function (json) {
                json = JSON.parse(json);

                console.log("Failure");
                console.log(json);
            },
            "complete": function () {
                $("#AdministratorGames .gameDelete").on("click", deleteEvent);
                $("#AdministratorGames .GameAddButton").attr("disabled", false);
            }
        });
    }
});