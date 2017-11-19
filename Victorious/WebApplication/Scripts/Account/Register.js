jQuery(document).ready(function ($) {

    $("#AccountRegister #SocialID").val('');
    $("#AccountRegister #AccessToken").val('');
    $("#AccountRegister #ProviderID").val(0);

    $("#AccountRegister .social-media .facebook-social-button").on("click", function () {
        FacebookAuth({ "status": "not_authorized" });
    });


    function FacebookAuth(json) {
        switch (json.status) {
            case "connected":
                if (json.authResponse.grantedScopes.indexOf("email") > -1) {
                    $("#AccountRegister #SocialID").val(json.authResponse.userID);
                    $("#AccountRegister #AccessToken").val(json.authResponse.accessToken);
                    $("#AccountRegister #ProviderID").val($("#AccountRegister #Providers option:contains('Facebook')").val());
                    $("#AccountRegister form").submit();
                }
                else {
                    FB.login(FacebookAuth, { "scope": "public_profile, email", "return_scopes": true, "auth_type": "rerequest" });
                }
                break;
            case "not_authorized":
                FB.login(FacebookAuth, { "scope": "public_profile, email", "return_scopes": true });
                break;
            case "unknown":
                console.log("User is not logged in.");
                break;
            default:
                break;
        }
    }
});