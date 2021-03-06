$(document).ready(function () {
    $('#EventsTable').DataTable({
        destroy: true,
        "paging": true
    });

});


completed = function (xhr) {
    var response = $.parseJSON(xhr);
    if (response.StatusCode === "200") {
        $('#modalTitle').html("Success");
        $('#modalMessage').html(response.Message);
        $('#infoModal').modal("show");
    } else {
        $('#modalTitle').html("Error: " + response.StatusCode);
        $('#modalMessage').html("Error Description: " + response.StatusDescrption + "\nError Message: " + response.Message);
        $('#infoModal').modal("show");
    } imgDisplay
};



function viewTable(url, listTable, table) {
    CurrentTable = table;
    $.ajax({
        url: url,
        dataType: "html",
        type: "GET",
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            $("#" + listTable).html(data);
            $('#' + table).DataTable({
                destroy: true,
                "paging": true
            });
        },
        error: function (xhr) {
            window.location.href = "/Error?StatusCode=444&StatusDescription=Error&StatusMessage=" + xhr.responseText;
        }
    });
}


function readURLMulti(input) {
    if (input.files) {
        for (i = 0; i < input.files.length; i++) {
            var reader = new FileReader();
            var img = '<div class="parentDiv newImg" style="width:200px; height:200px; display:inline-block;padding:10px;">' +
                '<img src="mydata" class="img-fluid" style="margin-left:auto; margin-right:auto; display:table"></div>';

            reader.onload = function (e) {
                $("#detailsGallery").append(img.replace('mydata', e.target.result));
            }
            reader.readAsDataURL(input.files[i]);
        }
    }
}

$("#multiUploadBtn").change(function () {
    readURLMulti(this);
});

function Delete(url, id, name, table, listTable) {
    $('#confirm').dialog({
        resizable: false,
        width: 400,
        height: 200,
        modal: true,
        open: function (event, ui) {
            $('#message').html("Are you sure you want to delete this " + name + "?");
        }
    }).dialog("option", "buttons", [
        {
            text: "Yes",
            width: "80",
            click: function () {
                $(this).dialog('close');
                $.ajax({
                    url: url + id,
                    dataType: "html",
                    type: "GET",
                    async: true,
                    processData: false,
                    cache: false,
                    success: function (data) {
                        $("#" + listTable).html(data);
                        $("#" + table).DataTable({
                            destroy: true,
                            "paging": true
                        });
                    },
                    error: function (xhr) {
                        window.location.href = "/Error?StatusCode=444&StatusDescription=Error&StatusMessage=" + xhr.responseText;
                    }
                });
            }
        },
        {
            text: "No",
            width: "80",
            click: function () {
                $(this).dialog('close');
            }
        }
    ]);
}

function ReadLog(fileName) {
    $.ajax({
        url: "/Admin/GetLogDetails?fileName=" + fileName,
        dataType: "text",
        type: "GET",
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            $("#currentLog").val(data);
        },
        error: function (xhr) {
            $("#currentLog").val(xhr.responseText);
        }
    });
}