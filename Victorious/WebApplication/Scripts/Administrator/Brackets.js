jQuery(document).ready(function () {
    var $ = jQuery;
    
    $("#AdministratorBrackets .bracket .icon").on("click", BracketTypeChange);

    function BracketTypeChange() {
        $this = $(this);

        $.ajax({
            "url": "/Ajax/Administrator/Bracket",
            "type": "post",
            "data": { "bracketTypeId": $this.closest(".bracket").data("bracketid") },
            "dataType": "json",
            "beforeSend": function () {
                $this.off("click");
            },
            "success": function (json) {
                json = JSON.parse(json);

                if (json.status) {
                    $this.removeClass();

                    if (json.isEnabled) {
                        $this.addClass("green icon icon-checkmark");
                    }
                    else {
                        $this.addClass("red icon icon-cross");
                    }
                }

                console.log(json.message);
            },
            "error": function(json) {
                console.log(json);
            },
            "complete": function() {
                $this.on("click", BracketTypeChange);
            }
        });
    }
});