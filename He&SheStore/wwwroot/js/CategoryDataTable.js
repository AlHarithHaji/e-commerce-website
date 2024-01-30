$(document).ready(function () {

    GetFaqsList();
});

function GetFaqsList() {
    $.ajax({
        url: '/Admin/CategoryIndexData',
        type: 'Get',
        dataType: 'json',
        success: OnSuccess
    })
}
function OnSuccess(responce) {

    $('#CategoryDataTable').DataTable({
        bProcessing: true,
        bLenghtChange: true,
        lengthMenu: [[10, 20, 30, -1], [10, 20, 30, "All"]],
        bfilter: true,
        bSort: true,
        bPaginate: true,
        bDestroy: true,
        data: responce,
        columns: [
            {
                data: 'CategoryName',
                render: function (data, type, row, meta) {
                    return row.categoryName
                }
            },          
            {
                data: 'Action',
                render: function (data, type, row, meta) {
                    return `<a href="javascript:void(0)" onclick="Edit('${row.id}')" class="btn btn-warning btn-sm"><i class="fa fa-edit"></i></a>  
                     `     

                }
            },
        ]
    });
}


$("#btnNew").click(function () {

    $.ajax({
        url: '/Admin/CategoryCreate',
        type: 'Get',
        success: function (data) {
            $("#jsAddParty").html(data);
            $('#modalOpen').modal({ backdrop: 'static', keyboard: false }, 'show');
        }
    })

});


function onSave() {


    $("#CategoryNameErr").text('');
  

    if ($("[name='CategoryName']").val() == "") {
        $("#CategoryNameErr").text('Category Name is required!');
    }
  
    else {
        let url = "/Admin/CategoryCreate";
        if ($("[name='Id']").val() != "") {
            url = "/Admin/CategoryEdit";
        }
        $.ajax({
            url: url,
            method: "POST",
            data: $("#form").serialize(),
            datatype: "JSON",
            success: function (r) {
                alert(r);
                $("#form")[0].reset();
                $('#modalOpen').modal("hide");
                GetFaqsList()

            },
            error: function (e) {
                alert(e);
            }
        });
    }
};

function Edit(id) {
    $.ajax({
        url: "/Admin/CategoryEdit",
        method: "GET",
        data: {
            id: id
        },
        success: function (data) {
            $("#jsAddParty").html(data);
            $("#btnSave").text('Update Changes');
            $("#btnSave").css({
                backgroundColor: "#17a2b8"
            });
            $('#PartyTitle').text('Edit Category Detail')
            $('#modalOpen').modal({ backdrop: 'static', keyboard: false }, 'show');
        },
        error: function (e) {
            alert(e);
        }
    });
}








