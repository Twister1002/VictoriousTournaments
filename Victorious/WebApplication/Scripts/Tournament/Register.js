jQuery(document).ready(function ($) {
    $("#TournamentRegister form").on("submit", function () {
        terms = $(this).find("#Terms");
        if (terms.length == 0 || terms.is(":checked")) {
            return true;
        }
        else {
            html = "<div class='message error form-error'>You must accept the terms of the tournament to register.</div>";
            $("#TournamentRegister").prepend(html);
            $("#TournamentRegister .message.form-error").delay(5000).fadeOut("500");
            return false;
        }
    });
});