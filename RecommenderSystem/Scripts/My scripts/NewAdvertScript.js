$(document).ready(function () {
    $("#discardAd").on('click', function () {
        $("#addAd").modal("toggle");
    });

    $("#addAd").on("show.bs.modal", function (e) {
        $("#btnAdPicture").val(null);
        $('input:checkbox').prop('checked', false);
    });

    $("#saveAd").on('click', function () {
        var picture = document.getElementById("btnAdPicture").files[0];
        var categories = [];

        $.each($("input[type='checkbox']:checked"), function () {
            categories.push($(this).val());
        });

        var formData = new FormData();
        formData.append("categories", JSON.stringify(categories));
        formData.append("picture", picture);

        $.ajax({
            type: "POST",
            url: '/Advert/AddNewAdvert',
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function () {
                $("#addAd").modal("toggle");
            },
            error: function () {
                alert("fail");
            }
        });
    });
})