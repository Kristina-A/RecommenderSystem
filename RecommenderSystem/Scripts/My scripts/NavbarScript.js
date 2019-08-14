function addSuggestion(name) {
    var par = document.getElementById("suggestions");

    var item = document.createElement("li");
    var link = document.createElement("a");
    link.innerHTML = name;
    link.setAttribute("href", "/Product/SearchProduct?name=" + name);

    item.appendChild(link);
    par.appendChild(item);
}

$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: '/Product/GetSuggestions',
        success: function (data) {
            if (data.prods.length > 0) {
                for (var i = 0; i < data.prods.length; i++)
                    addSuggestion(data.prods[i]);
            }
        },
        error: function () {
            alert("fail");
        }
    });

    $("#btnSearch").on("click", function () {
        var val = $("#searchBar").val();

        if (val == "")
            window.location.reload();
        else
            window.location.href = '/Product/SearchProduct?name=' + val;
    });
});