ValidateInput = function () {
    $this = $(this);
    $('#create').on('click', function (e) {
        var image = $('#uploadImage');
        if (image.val() == null || image.val() == undefined || image.val() == "") {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'It appears you forgot to add an image!'
            });
            e.preventDefault();
        }
    });

};

initProductIndex = function (dataUrl, deleteUrl) {
    var dataTable;

    var dataColumns = [
        {
            data: "name",
            render: function (data, display, full) {
                return `
                    <a href="Product/Upsert?id=${full.id}">${data}</a>`
            }
        },
        {
            data: "isbn",
        },
        {
            data: "price",
        },
        {
            data: "author",
        },
        {
            data: "category.name",
        },
        {
            data: "id",
            render: function (data) {
                return `
                    <a id="delete" class="btn btn-danger" value="${data}"><i class="bi bi-trash"></i>Delete?</a>`
            }
        }
    ]

    $(document).ready(function () {
        loadDataTable();
    });

    function loadDataTable() {
        dataTable = $('#productIndex').DataTable({
            ajax: {
                url: dataUrl
            },
            columns: dataColumns
        });
    }
    $(document).on('click', '#delete', function () {
        $this = $(this);
        var test = $this.closest('#delete');
        var id = test.attr('value');
        Delete(`${deleteUrl}/${id}`);
    });
    function Delete(url) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: url,
                    type: "DELETE",
                    success: function (data) {
                        if (data.success) {
                            toastr.success(data.message);
                            dataTable.ajax.reload();
                        }
                        else {
                            toastr.error(data.message);
                        }
                    }
                });
            }
        })
    }
};

initCompanyIndex = function (dataUrl, deleteUrl) {
    var dataTable;

    var dataColumns = [
        {
            data: "name",
            render: function (data, display, full) {
                return `
                    <a href="Company/Upsert?id=${full.id}">${data}</a>`
            }
        },
        {
            data: "streetAddress",
        },
        {
            data: "city",
        },
        {
            data: "postalCode",
        },
        {
            data: "phoneNumber",
        },
        {
            data: "id",
            render: function (data) {
                return `
                    <a id="delete" class="btn btn-danger" value="${data}"><i class="bi bi-trash"></i>Delete?</a>`
            }
        }
    ]

    $(document).ready(function () {
        loadDataTable();
    });

    function loadDataTable() {
        dataTable = $('#companyIndex').DataTable({
            ajax: {
                url: dataUrl
            },
            columns: dataColumns
        });
    }
    $(document).on('click', '#delete', function () {
        $this = $(this);
        var test = $this.closest('#delete');
        var id = test.attr('value');
        Delete(`${deleteUrl}/${id}`);
    });
    function Delete(url) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: url,
                    type: "DELETE",
                    success: function (data) {
                        if (data.success) {
                            toastr.success(data.message);
                            dataTable.ajax.reload();
                        }
                        else {
                            toastr.error(data.message);
                        }
                    }
                });
            }
        })
    }
};

initHomePage = function (detailsUrl) {
    $(document).on('click', '#productCard', function (e) {
        var $this = $(this);
        var id = $this.closest('#productCard').attr('value');
        $.ajax({
            url: `${detailsUrl}/${id}`,
            type: "GET"
        })
    });
};

initRegister = function () {
    $('#Input_Role').on('change', function (e) {
        var $this = $(this);
        var thisRole = $this[0].selectedOptions[0].text;
        if (thisRole != "Company") {
            $('.companyId').addClass('visually-hidden');
        }
        if (thisRole == "Company") {
            $('.companyId').removeClass('visually-hidden');
        }
    });

}