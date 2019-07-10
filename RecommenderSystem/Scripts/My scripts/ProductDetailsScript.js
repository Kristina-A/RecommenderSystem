function clearModal() {
    $("#inputNameChar").val("");
    $("#inputValueChar").val("");
}

function addComment(id, name, surname, comment, role) {
    var div = document.getElementById("comments");

    var commentDiv = document.createElement("div");
    commentDiv.setAttribute("id", "comment" + id);

    var labelUser = document.createElement("label");
    labelUser.setAttribute("class", "col-sm-7");
    labelUser.style.color = "blue";
    labelUser.innerHTML = name + " " + surname;

    var labelContent = document.createElement("label");
    labelContent.setAttribute("class", "col-sm-7");
    labelContent.innerHTML = comment;

    var divReply;
    if (role == "Admin") {
        divReply = document.createElement("div");
        divReply.setAttribute("class", "form-group");
        divReply.style.marginLeft = "50px";

        var inputReply = document.createElement("textarea");
        inputReply.setAttribute("rows", "3");
        inputReply.setAttribute("class", "form-control col-sm-4");
        inputReply.setAttribute("id", id);
        inputReply.setAttribute("placeholder", "Unesite odgovor");
        divReply.appendChild(inputReply);

        var replyButton = document.createElement("button");
        replyButton.setAttribute("class", "btn btn-info");
        replyButton.setAttribute("type", "button");
        replyButton.innerHTML = "Odgovori";
        divReply.appendChild(replyButton);

        replyButton.addEventListener("click", function () {
            var mess = $("#" + id).val();
            $.ajax({
                type: "POST",
                url: '/Product/AddResponse',
                data: { "id": id, "content": mess },
                success: function () {
                    $("#" + id).val("");
                    addReply(id, mess);
                },
                error: function () {
                    alert("fail");
                }
            });
        });
    }

    commentDiv.appendChild(labelUser);
    commentDiv.appendChild(labelContent);
    if (role == "Admin") {
        commentDiv.appendChild(divReply);
    }
    div.appendChild(commentDiv);
}

function addReply(id, reply) {
    var div = document.getElementById("comment" + id);

    var lab = document.createElement("label");
    lab.setAttribute("class", "col-sm-7");
    lab.style.marginLeft = "50px";
    lab.innerHTML = reply;
    div.appendChild(lab);
}

function addReview(name, surname, grade, comment) {
    var div = document.getElementById("reviews");

    var reviewDiv = document.createElement("div");

    var labelUser = document.createElement("label");
    labelUser.setAttribute("class", "col-sm-4");
    labelUser.style.color = "blue";
    labelUser.innerHTML = name + " " + surname;

    var labelGrade = document.createElement("label");
    labelGrade.setAttribute("class", "col-sm-6");
    labelGrade.innerHTML = grade;

    var star = document.createElement("i");
    star.setAttribute("class", "fas fa-star");

    var labelComment = document.createElement("label");
    labelComment.setAttribute("class", "col-sm-6");
    labelComment.innerHTML = comment;

    reviewDiv.appendChild(labelUser);
    labelGrade.appendChild(star);
    reviewDiv.appendChild(labelGrade);
    reviewDiv.appendChild(labelComment);
    div.appendChild(reviewDiv);
}


$(document).ready(function () {
    var button;
    var row;
    var classes = $("#pictureBox").attr("class");//mora za ovo, treba nesto sto je svim userima i adminima dostupno
    var prodID = classes.split(" ")[0];

    $.ajax({
        type: "POST",
        url: '/Product/AverageGrade',
        data: { "id": prodID },
        success: function (data) {
            var text = $("#lblGrade").text();
            $("#lblGrade").text(text + " (" + data.number + "): " + data.grade);
        },
        error: function () {
            alert("fail");
        }
    });

    $("#deleteProduct").click(function () {

        $.ajax({
            type: "POST",
            url: '/Product/DeleteProduct',
            data: { "id": prodID },
            success: function () {
                window.location.href = '/Home/Index';
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#discardEditing").click(function () {
        $("#editModal").modal("toggle");
    });

    $("#saveEditing").click(function () {
        var name = $("#inputEditName").val();
        var price = $("#inputEditPrice").val();
        var pictureBtn = document.getElementById("btnEditPicture");
        var picture = pictureBtn.files[0];

        var formData = new FormData();
        formData.append("id", prodID);
        formData.append("name", name);
        formData.append("price", price);
        formData.append("picture", picture);

        $.ajax({
            type: "POST",
            url: '/Product/EditProduct',
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function () {
                $("#editModal").modal("toggle");
                window.location.href = '/Product/ProductDetails/' + id;
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#discardChar").on("click", function () {
        $("#characteristicsModal").modal("toggle");
    });

    $("#characteristicsModal").on("show.bs.modal", function (e) {
        button = e.relatedTarget.id;
        clearModal();
        if (button == "editChar") {
            row = e.relatedTarget.closest("tr");
            var name = $(row).find('td.name').text();
            var value = $(row).find('td.value').text();

            $("#inputNameChar").val(name);
            $("#inputValueChar").val(value);
        }
    });

    $("#saveChar").on("click", function () {
        var name = $("#inputNameChar").val();
        var value = $("#inputValueChar").val();
        var oldName = null;
        var oldVal = null;
        if (button == "editChar") {
            oldName = $(row).find('td.name').text();
            oldVal = $(row).find('td.value').text();
        }

        $.ajax({
            type: "POST",
            url: '/Product/EditCharacteristics',
            data: { "id": prodID, "charName": name, "charValue": value, "oldN": oldName, "oldV": oldVal },
            success: function () {
                $("#characteristicsModal").modal("toggle");
                if (button == "editChar") {
                    $(row).find('td.name').text(name);
                    $(row).find('td.value').text(value);
                }
                else {
                    window.location.href = '/Product/ProductDetails/' + prodID;
                }
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#showReviews").on("click", function () {
        if ($(this).attr("class") != "disabled") {
            $.ajax({
                type: "POST",
                url: '/Product/GetReviews',
                data: { "id": prodID },
                success: function (data) {
                    var num = data.number;
                    for (var i = 0; i < num; i++) {
                        addReview(data.people[i]["Name"], data.people[i]["Surname"], data.revs[i]["Grade"], data.revs[i]["Comment"]);
                    }
                    $("#showReviews").addClass("disabled");
                },
                error: function () {
                    alert("fail");
                }
            });
        }
    });

    $("#showReviews").on({
        mouseenter: function () {
            $(this).css("color", "red");
            $(this).css("cursor", "pointer");
        },
        mouseleave: function () {
            $(this).css("color", "black");
        }
    });

    $("#showComments").on({
        mouseenter: function () {
            $(this).css("color", "red");
            $(this).css("cursor", "pointer");
        },
        mouseleave: function () {
            $(this).css("color", "black");
        }
    });

    $("#showComments").on("click", function () {
        if ($(this).attr("class") != "disabled") {
            $.ajax({
                type: "POST",
                url: '/Product/GetComments',
                data: { "id": prodID },
                success: function (data) {
                    var num = data.number;
                    for (var i = 0; i < num; i++) {
                        var role = data.status;
                        addComment(data.com[i]["Id"], data.people[i]["Name"], data.people[i]["Surname"], data.com[i]["Content"], role);

                        var responses = data.com[i]["Responses"];
                        for (var j = 0; j < responses.length; j++) {
                            addReply(data.com[i]["Id"], responses[j])
                        }
                    }
                    $("#showComments").addClass("disabled");
                },
                error: function () {
                    alert("fail");
                }
            });
        }
    });

    $("#postComment").on("click", function () {
        var txt = $("#comment").val();

        $.ajax({
            type: "POST",
            url: '/Product/AddComment',
            data: { "prodId": prodID, "content": txt },
            success: function () {
                window.location.href = '/Product/ProductDetails/' + prodID;
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $(".far.fa-star").on({
        mouseenter: function () {
            for (var i = 1; i <= $(this).attr("id"); i++)
                $("#" + i).attr("class", "fas fa-star");
            $(this).css("cursor", "pointer");
        },
        mouseleave: function () {
            for (var i = 1; i <= $(this).attr("id"); i++)
                $("#" + i).attr("class", "far fa-star");
        }
    });

    $("#discardReview").on("click", function () {
        $("#reviewModal").modal("toggle");
    });

    $("#reviewModal").on("show.bs.modal", function (e) {
        var star = e.relatedTarget.id;
        $("#inputComment").val("");
        $("#lblStar").text(star);
    });

    $("#saveReview").on("click", function () {
        var star = $("#lblStar").text();
        var comm = $("#inputComment").val();

        $.ajax({
            type: "POST",
            url: '/Product/AddReview',
            data: { "id": prodID, "grade": star, "comment": comm },
            success: function () {
                $("#reviewModal").modal("toggle");
                window.location.href = '/Product/ProductDetails/' + prodID;
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#addToChart").on("click", function () {
        $.ajax({
            type: "POST",
            url: '/Order/AddToChart',
            data: { "id": prodID },
            success: function () {
                window.location.href = '/Product/ProductDetails/' + prodID;
            },
            error: function () {
                alert("fail");
            }
        });
    });
})