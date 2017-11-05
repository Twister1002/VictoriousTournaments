  window.fbAsyncInit = function() {
      FB.init({
          appId      : '307736699633852',
          cookie     : true,
          xfbml      : true,
          version    : 'v2.10'
      });
      
      FB.AppEvents.logPageView();   
      
  };

(function(d, s, id){
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) {return;}
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

$(document).ready(function ($) {
    $(".facebook-login-button").on("click", function () {
        FB.login(function (response) {
            switch (response.status) {
                case "connected":
                    $("#AccountLogin #SocialID").val(response.authResponse.userID);
                    $("#AccountLogin #ProviderID").val($("#AccountLogin #Providers option:contains('Facebook')").val());
                    $("#AccountLogin form").submit();
                    break;
                case "not_authorized":
                    console.log("User has not authorized this yet.");
                    break;
                case "unknown":
                    console.log("User is not logged in.");
                    break;
                default:
                    break;
            }
        }, { "scope": "public_profile, email" });
    });
});