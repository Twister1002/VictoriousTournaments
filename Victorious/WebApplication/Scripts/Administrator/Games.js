jQuery(document).ready(function () {
    var $ = jQuery;
    var section = $("#AdministratorGames");

    $("#AdministratorGames .gameDelete").on("click", deleteEvent);
    $("#AdministratorGames .GameAddButton").on("click", addEvent);
    $("#AdministratorGames .options .gameTitle").on("keydown", function (e) {
        if (e && e.keyCode == 13) { // Enter
            addEvent();
        }
    });

    function addEvent() {
        var jsonData = {
            "function": "add",
            "Title": $("#AdministratorGames .form .gameTitle .field").val()
        };

        GameUpdate(jsonData);
    }

    function deleteEvent() {
        var jsonData = {
            "function": "delete",
            "GameID": $(this).closest(".game").data("gameid")
        };

        GameUpdate(jsonData);
    }

    function GameUpdate(jsonData) {
        $.ajax({
            "url": "/Ajax/Administrator/Games",
            "type": "POST",
            "data": jsonData,
            "dataType": "json",
            "beforeSend": function() {
                $("#AdministratorGames .gameDelete").off("click");
                $("#AdministratorGames .GameAddButton").attr("disabled", true);
            },
            "success": function (json) {
                if (json.status) {
                    var table = $("#AdministratorGames .list-table-body");
                    table.empty();

                    $.each(json.data.games, function (i, e) {
                        html = "<ul class='game' data-columns='2' data-gameid='" + e.GameTypeID + "'>";
                        html += "<li class='column gameTitle'>" + e.Title + "</li>";
                        html += "<li class='column gameDelete'><span class='icon icon-cross'></span></li>";
                        html += "</ul>";

                        table.append(html);
                    });

                    $(".gameTitle input").val('');
                }

                console.log(json.message);
            },
            "error": function (json) {
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