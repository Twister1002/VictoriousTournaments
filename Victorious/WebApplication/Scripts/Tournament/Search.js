jQuery(document).ready(function () {
    $(".clearButton").on("click", function () {
        $("#TournamentSearch .options .title").val('');
        $("#TournamentSearch .options .gameType").val('');
        $("#TournamentSearch .options .startDate").val('');

        $("#TournamentSearch .options .searchButton").click();
    });

    $("#TournamentSearch .options .searchButton").on("click", function () {
        var jsonData = {
            "title": $("#TournamentSearch .options .title").val(),
            "gameTypeId": $("#TournamentSearch .options .gameType").val(),
            "startDate": $("#TournamentSearch .options .startDate").val()
        }
        $.ajax({
            "url": "/Ajax/Tournament/Search",
            "type": "post",
            "data": { "searchData": JSON.stringify(jsonData) },
            "dataType": "json",
            "success": function (json) {
                json = JSON.parse(json);

                var tournaments = $("#TournamentSearch .list-table-body");

                // Clear the games
                tournaments.empty();

                // Add all the elements found
                $.each(json, function (i, e) {
                    html = "<a class='column' href='" + e.link + "'>";
                    html += "<ul class='column-clickable' data-columns='4'>";
                    html += "<li class='column'>" + e.title + "</li>";
                    html += "<li class='column'>" + e.game + "</li>";
                    html += "<li class='column'>" + e.startDate + "</li>";
                    html += "<li class='column'>" + (e.isPublic ? "Public" : "Private") + "</li>";
                    html += "</ul>";
                    html += "</a>";

                    tournaments.append(html);
                });


                // Clear the list 
                console.log(json);
            },
            "error": function (json) {
                console.log(json);
            }
        });
    });

    (function ($) {
        // Reset all the fields
        $("#TournamentSearch .options").each(function (i, e) {
            $(e).find("input").val('');
            $(e).find("select").val('');
        });
    })($);
});
