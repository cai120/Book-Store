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

}