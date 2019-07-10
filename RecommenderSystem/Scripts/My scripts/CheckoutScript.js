$(document).ready(function () {

    $(".deleteFromChart.btn.btn-danger.col-sm-4.btn-sm").on("click", function () {
        var prodID = $(this).attr("id");

        $.ajax({
            type: "POST",
            url: '/Order/DeleteFromChart',
            data: { "id": prodID },
            success: function () {
                window.location.href = '/Order/Checkout/';
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#discardShopping").on("click", function () {
        $.ajax({
            type: "POST",
            url: '/Order/DeleteOrder',
            success: function () {
                window.location.href = '/Home/Index/';
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#shop").on("click", function () {
        var newAddr = $("#inputAddress").val();
        var addr = $("#selectAddress").val();
        var note = $("#inputNote").val();
        var paying = $("input:checked").val();
        var sendAddress;
        if (addr != "")
            sendAddress = addr;
        else
            sendAddress = newAddr;

        $.ajax({
            type: "POST",
            url: '/Order/SubmitOrder',
            data: { "address": sendAddress, "note": note, "pay": paying },
            success: function () {
                window.location.href = '/Home/Index/';
                alert("Uspesno ste porucili proizvode, stici ce vam mejl sa detaljima :)");
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#selectAddress").change(function () {
        if (this.value != "") {
            $("#inputAddress").val("");
            $("#inputAddress").prop("disabled", true);
        }
        else {
            $("#inputAddress").prop("disabled", false);
        }
    });
})