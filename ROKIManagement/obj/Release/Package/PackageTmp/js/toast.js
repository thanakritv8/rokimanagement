$(function () {

    function Toast(type, css, msg) {
        this.type = type;
        this.css = css;
        this.msg = msg;
    }

    var toasts = [
        new Toast('error', 'toast-bottom-center', 'error'),
        new Toast('warning', 'toast-bottom-center', 'warning'),
        new Toast('success', 'toast-bottom-center', 'success')
    ];

    toastr.options.extendedTimeOut = 0; //1000;
    toastr.options.timeOut = 3000;
    toastr.options.fadeOut = 250;
    toastr.options.fadeIn = 250;

    var i = 0;

    $('#btnSave').click(function () {
        showToast(2);
    });
    $('#btnEdit').click(function () {
        showToast(2);
    });
    function showToast(indexMsg) {
        var t = toasts[indexMsg];
        toastr.options.positionClass = t.css;
        toastr[t.type](t.msg);
    }
})
