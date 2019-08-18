var toggler = document.getElementsByClassName("caret");
var i;

$(document).ready(function () {
    for (i = 0; i < toggler.length; i++) {
        toggler[i].addEventListener("click", function () {
            this.parentElement.querySelector(".nested").classList.toggle("active");
            this.classList.toggle("caret-down");
        });
    }

    //$("#viewProduct").on("click", function () {
    //    var prodID = $(this).attr("href");
    //    var links = prodID.split("/");

    //    $.ajax({
    //        type: "POST",
    //        url: '/Product/ViewProduct',
    //        data: { "prodID": links[3] },
    //        success: function () {
    //        },
    //        error: function () {
    //            alert("fail");
    //        }
    //    });
    //});
});