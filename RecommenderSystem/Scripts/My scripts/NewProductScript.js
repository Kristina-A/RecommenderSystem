var numCharacteristics = 1;

function makeFormCharacteristic(numChar) {

    var formChar = document.getElementById("formCharacteristics");

    var characteristicName = document.createElement("div");
    characteristicName.setAttribute("class", "form-group row");

    var labelName = document.createElement("label");
    labelName.setAttribute("class", "col-sm-4 control-label");
    labelName.innerHTML = "Naziv:";

    var divWrap1 = document.createElement("div");
    divWrap1.setAttribute("class", "col-sm-7");

    var inputName = document.createElement("input");
    inputName.setAttribute("type", "text");
    inputName.setAttribute("class", "form-control");
    inputName.setAttribute("id", "inputCharName" + numChar);

    characteristicName.appendChild(labelName);
    divWrap1.appendChild(inputName);
    characteristicName.appendChild(divWrap1);
    formChar.appendChild(characteristicName);


    var characteristicValue = document.createElement("div");
    characteristicValue.setAttribute("class", "form-group row");

    var labelValue = document.createElement("label");
    labelValue.setAttribute("class", "col-sm-4 control-label");
    labelValue.innerHTML = "Vrednost:";

    var divWrap2 = document.createElement("div");
    divWrap2.setAttribute("class", "col-sm-7");

    var inputValue = document.createElement("input");
    inputValue.setAttribute("type", "text");
    inputValue.setAttribute("class", "form-control");
    inputValue.setAttribute("id", "inputCharValue" + numChar);

    characteristicValue.appendChild(labelValue);
    divWrap2.appendChild(inputValue);
    characteristicValue.appendChild(divWrap2);
    formChar.appendChild(characteristicValue);
    formChar.appendChild(document.createElement("hr"));

    var btnDelete = document.getElementById("deleteCharacteristic");
    btnDelete.style.display = "";
}

function deleteFormCharacteristic() {
    var formChar = document.getElementById("formCharacteristics");
    for (var i = 0; i < 3; i++) {
        formChar.removeChild(formChar.lastChild);
    }
}

$(document).ready(function () {
    $("#addNewCharacteristic").click(function () {
        numCharacteristics++;
        makeFormCharacteristic(numCharacteristics);
    });

    $("#discardAdding").on('click', function () {
        window.location.href = '/Home/Index';
    });

    $("#deleteCharacteristic").on('click', function () {
        deleteFormCharacteristic();
        numCharacteristics--;
        if (numCharacteristics === 1) {
            document.getElementById("deleteCharacteristic").style.display = "none";
        }
    });

    $("#addNewProduct").on('click', function () {
        var name = $("#inputName").val();
        var category = $("#selectCategory").val();
        var subcategory = $("#selectSubcat").val();
        var price = $("#inputPrice").val();
        var pictureBtn = document.getElementById("btnProductPicture");
        var picture = pictureBtn.files[0];
        var characteristics = [];

        var formData = new FormData();
        formData.append("name", name);
        formData.append("category", category);
        formData.append("subcategory", subcategory);
        formData.append("price", price);
        formData.append("picture", picture);

        for (var i = 1; i <= numCharacteristics; i++) {
            var subcat = $("#inputCharName" + i).val();
            var subcatVal = $("#inputCharValue" + i).val();
            characteristics.push(subcat + ":" + subcatVal);
        };
        formData.append("characteristics", JSON.stringify(characteristics));

        $.ajax({
            type: "POST",
            url: '/Product/AddNewProduct',
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function () {
                window.location.href = '/Home/Index';
            },
            error: function () {
                alert("fail");
            }
        });
    });

    $("#selectCategory").change(function () {
        var category = this.value;

        $.ajax({
            type: "POST",
            url: '/Product/ReturnSubcategories',
            data: { "option": category },
            success: function (data) {
                $("#selectSubcat").empty();
                $("#selectSubcat").append(new Option("Izaberite jednu potkategoriju", "", true));
                for (var i = 0; i < data.subcat.length; i++) {
                    $("#selectSubcat").append(new Option(data.subcat[i], data.subcat[i]));
                };
            },
            error: function () {
                alert("fail");
            }
        });
    });
})
