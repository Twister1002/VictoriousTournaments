jQuery(document).ready(function ($) {
    
    $(".accountMenu .menu").on("click", function () {
        $(this).addClass("active").siblings().removeClass("active");
        $("#Account" + $(this).data("type")).addClass("active").siblings().removeClass("active");
    });

    (function () {
        if ($(".accountMenu").length == 1) {
            $(".accountMenu .active").click();
        }
    })();

    // Load in the social accounts
    $("#AccountUpdate .social-connection button").on("click", function () {
        var button = $(this);
        // Update the Social Link
        if (button.hasClass("facebook")) {
            FB.login(function (response) {
                switch (response.status) {
                    case "connected":
                        data = {
                            "addConnection": button.val() == "add",
                            "provider": button.closest(".social-connection").data("social"),
                            "token": response.authResponse.accessToken
                        };

                        $.ajax({
                            "url": "/Ajax/Account/SocialLink/Modify",
                            "type": "post",
                            "data": data,
                            "dataType": "json",
                            "success": function (json) {
                                if (json.status) {
                                    button.closest(".social-media").prepend("<div class='message social-media-notice " + (json.status ? "success" : "error") + "'>" + json.message + "</div>");
                                    setTimeout(function () {
                                        button.closest(".social-media").find(".social-media-notice").fadeOut(1000);
                                    }, 5000);

                                    if (data.addConnection) {
                                        button.val("remove");
                                        button.find(".prefix").text("Disconnect");
                                    }
                                    else {
                                        button.val("add");
                                        button.find(".prefix").text("Link with");
                                    }
                                }
                                else {
                                    button.closest(".social-media").prepend("<div class='message social-media-notice " + (json.status ? "success" : "error") + "'>" + json.message + "</div>");
                                    setTimeout(function () {
                                        button.closest(".social-media").find(".social-media-notice").fadeOut(1000);
                                    }, 5000);
                                }
                            },
                            "error": function (json) {
                                console.log("Error");
                                console.log(json);
                            }
                        });
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
        }
    });
});