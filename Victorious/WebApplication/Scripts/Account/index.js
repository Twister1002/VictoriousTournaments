jQuery(document).ready(function ($) {
    
    $(".accountMenu .menu").on("click", function () {
        $(this).addClass("active").siblings().removeClass("active");
        $("#Account" + $(this).data("type")).addClass("active").siblings().removeClass("active");
    });

});