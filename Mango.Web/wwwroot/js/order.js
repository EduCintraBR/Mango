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
            {
                data: 'orderTime',
                "width": "10%",
                "render": function (data) {
                    return formatDateField(data);
                }
            },
            { data: 'status', "width": "11%" },
            {
                data: 'orderTotal',
                "width": "10%",
                "render": function (data) {
                    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(data);
                }
            },
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

function formatDateField (data) {
    var date = new Date(data);
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();

    day = day < 10 ? '0' + day : day;
    month = month < 10 ? '0' + month : month;

    return day + '/' + month + '/' + year;
}