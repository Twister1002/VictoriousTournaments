jQuery(document).ready(function () {
    $(".group-data .groupName").on("click", function () {
        $(this).closest("ul").find(".groupName").removeClass("selected");
        $(this)
            .addClass("selected")
            .closest(".list-table-body")
            .find(".round[data-groupnum='" + $(this).data("groupnum") + "']").removeClass("hide")
            .siblings().addClass("hide");
    });
});