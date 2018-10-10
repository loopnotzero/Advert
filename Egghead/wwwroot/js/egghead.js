$(document).ready(function () {
    var form = $('.form-group--float');
    if (form[0]) {
        form.each(function () {
            var p = $(this).find('.form-control').val();
            if (p.length !== 0) {
                $(this).find('.form-control').addClass('form-control--active');
            }
        });

        $('body').on('blur', '.form-group--float .form-control', function () {
            var i = $(this).val();
            if (i.length === 0) {
                $(this).removeClass('form-control--active');
            }
            else {
                $(this).addClass('form-control--active');
            }
        });

        $(this).find('.form-control').change(function () {
            var x = $(this).val();
            if (x.length !== 0) {
                $(this).find('.form-control').addClass('form-control--active');
            }
        });
    }
});