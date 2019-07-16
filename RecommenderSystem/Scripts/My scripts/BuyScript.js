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
    link.setAttribute("class", "dropdown-item");
    link.setAttribute("href", "#");
    link.setAttribute("data-toggle", "modal");
    link.setAttribute("data-target", "#notificationModal");

    if (!read) {
        link.style.backgroundColor = "#e6f3ff";
    }

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
                    addProduct(data.prod[i]["Id"], data.prod[i]["Name"], data.prod[i]["Price"]);
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
                var total = data.total;
                $("#numNots").text(num);
                for (var i = 0; i < total; i++) {
                    addNotification(data.alerts[i]["Id"], data.alerts[i]["Title"], data.alerts[i]["Date"], data.alerts[i]["Read"]);
                }
            },
            error: function () {
                alert("fail");
            }
        });
    }

    $("#notificationModal").on("show.bs.modal", function (e) {
        var id = e.relatedTarget.id;

        $.ajax({
            type: "POST",
            url: '/Notifications/ReadNotification',
            data: { "notId": id },
            success: function (data) {
                var date = data.date;
                var content = data.content;
                var tag = data.tag;

                $("#notificationDate").text(date);
                $("#notificationContent").text(content);

                if (tag == "lose_ocene") {
                    var par = document.getElementById("discountActivation");
                    var link = document.createElement("a");
                    link.setAttribute("href", "/Notifications/ActivateDiscount");
                    link.innerHTML = "Aktivirajte popust od 10%";

                    par.appendChild(link);
                }
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#notificationModal").on("hidden.bs.modal", function (e) {
        location.reload();
    });
})