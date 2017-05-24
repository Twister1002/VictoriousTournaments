jQuery(document).ready(function () {
    var $ = jQuery;
    var addedUserIndex = 0;

    $("#TournamentEdit .user-section .addUser").on("click", function () {
        html = "<ul class='user border form' data-user='-1' data-columns='4'> ";
        html += "<li class='column name'><input type='text' name='Users[" + addedUserIndex + "].Name' id='Users[" + addedUserIndex + "].Name' maxlength='50' placeholder='Email or Username'/></li> ";
        html += "<li class='column permission'>Not Saved</li> ";
        html += "<li class='column'><span class='icon icon-checkmark checkedIn'></span></li> ";
        html += "<li class='column actions'><button type='button' class='demote'>Remove</button></li> ";
        html += "</ul> ";
        
        addedUserIndex++;
        $(this).closest(".user-section").find(".users").append(html);

        // Add the event to the button
        $(".user-section .user .actions .demote").off("click");
        $(".user-section .user .actions .demote").on("click", permissionDemote);
    });
});