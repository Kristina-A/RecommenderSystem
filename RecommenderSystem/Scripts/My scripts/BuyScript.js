function addProduct(id, name, price) {
    var par = document.getElementById("items");

    var link = document.createElement("a");
    link.innerHTML = name;
    link.setAttribute("href", "/Product/ProductDetails/" + id);
    link.setAttribute("id", id);
    link.setAttribute("value", price);
    link.setAttribute("class", "dropdown-item");

    par.appendChild(link);
}

function addNotification(id, title, date, read) {
    var par = document.getElementById("alerts");

    var link = document.createElement("a");
    link.innerHTML = title + "            " + date;
    link.setAttribute("id", id);

    if (!read)
        link.setAttribute("class", "dropdown-item btn btn-info");
    else
        link.setAttribute("class", "dropdown-item");

    par.appendChild(link);
}


$(document).ready(function () {

    if ($("#numProducts").text() != "") {
        $.ajax({
            type: "POST",
            url: '/Order/UpdateChart',
            success: function (data) {
                var num = data.number;
                $("#numProducts").text(num);
                var sum = 0;
                for (var i = 0; i < num; i++) {
                    addNotification(data.prod[i]["Id"], data.prod[i]["Name"], data.prod[i]["Price"]);
                    sum += parseInt(data.prod[i]["Price"]);
                }
                var txt = $("#total").text();
                $("#total").text(txt + sum.toString() + " din");
            },
            error: function () {
                alert("fail");
            }
        });
    }

    if ($("#numNots").text() != "") {
        $.ajax({
            type: "POST",
            url: '/Notifications/GetNotifications',
            success: function (data) {
                var num = data.number;
                $("#numNots").text(num);
                for (var i = 0; i < num; i++) {
                    addProduct(data.alerts[i]["Id"], data.alerts[i]["Title"], data.alerts[i]["Date"], data.alerts[i]["Read"]);
                }
            },
            error: function () {
                alert("fail");
            }
        });
    }
})