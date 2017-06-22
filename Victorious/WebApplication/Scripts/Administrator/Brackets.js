jQuery(document).ready(function () {
    var $ = jQuery;
    
    $("#AdministratorBrackets .bracket .icon").on("click", BracketTypeChange);

    function BracketTypeChange() {
        $brackets = $(this).closest(".list-table-body");

        $.ajax({
            "url": "/Ajax/Administrator/Bracket",
            "type": "post",
            "data": { "BracketID": $(this).closest(".bracket").data("id") },
            "dataType": "json",
            "beforeSend": function () {
                $brackets.find(".icon").off("click");
            },
            "success": function (json) {
                if (json.status) {
                    $.each(json.data.brackets, function (i, e) {
                        $icon = $("#AdministratorBrackets .bracket[data-id='" + e.BracketTypeID + "'] .icon");
                        $icon.removeClass();
                        if (e.IsActive) {
                            $icon.addClass("green icon icon-checkmark");
                        }
                        else {
                            $icon.addClass("red icon icon-cross");
                        }
                    });
                }

                console.log(json.message);
            },
            "error": function(json) {
                console.log(json);
            },
            "complete": function() {
                $brackets.find(".icon").on("click", BracketTypeChange);
            }
        });
    }
});