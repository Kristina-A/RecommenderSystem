
$(document).ready(function () {
    $("#btnSearch").on("click", function () {
        var val = $("#searchBar").val();

        if (val == "")
            window.location.reload();
        else
            window.location.href = '/Product/SearchProduct?name=' + val;
    });
});