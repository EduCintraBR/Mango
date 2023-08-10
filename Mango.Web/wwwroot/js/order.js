var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": { url: "/order/getAll" },
        "columns": [
            { data: 'orderHeaderId', "width": "5%" },
            { data: 'email', "width": "22%" },
            { data: 'name', "width": "22%" },
            { data: 'phone', "width": "10%" },
            { data: 'status', "width": "11%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'orderHeaderId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/order/orderDetails?orderId=${data}" class="btn btn-primary mx-4">
                            <i class="bi bi-pencil-square"></i>
                        </a>
                    </div>`
                },
                "width": "10%"
            }
        ]
    })
}