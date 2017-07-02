jQuery(document).ready(function () {
    $("#faq .question label").on("click", function () {
        var icon = $(this).find(".icon");
        
        // Action needs to show the answer
        if (icon.hasClass("icon-plus")) {
            icon.removeClass("icon-plus").addClass("icon-cross");
            $(this).siblings(".answer").addClass("selected");
        }
        // Action needs to hide the answer
        else {
            icon.removeClass("icon-cross").addClass("icon-plus");
            $(this).siblings(".answer").removeClass("selected");
        }
        
    });
});