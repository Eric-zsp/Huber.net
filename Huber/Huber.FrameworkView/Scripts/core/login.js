function login() {
    if ($("#uid").val() == '') {
        art.dialog({
            lock:true,
            icon: 'error',
            background: '#000', // 背景色
            opacity: 0.01,	// 透明度
            time: 10,
            content: '请输入用户名'
        });
        return;
    }
    if ($("#pwd").val() == '') {
        art.dialog({
            lock: true,
            icon: 'error',
            time: 1,
            content: '请输入密码'
        });
        return;
    }
    var pwdMd5Str = $.md5($("#uid").val() + "\f" + $("#pwd").val());
    $.ajax({
        url: '/User/login',
        type: 'post',
        data: { Uid: $("#uid").val(), pwd: pwdMd5Str, remember: $("#forget_pw").val() == "on" },
        cache: false,
        success: function (txt) {
            if (txt == 1) {
                window.location.href = "/Home/Index";
            } else if (txt == -1) {
                art.dialog({
                    lock: true,
                    icon: 'error',
                    time: 1,
                    content: '账号不存在'
                });
            } else if (txt == -2) {
                art.dialog({
                    lock: true,
                    icon: 'error',
                    time: 1,
                    content: '用户名或密码错误'
                });
            } else if (txt == -3) {
                art.dialog({
                    lock: true,
                    icon: 'error',
                    time: 1,
                    content: '账户已被禁用'
                });
            } else {
                art.dialog({
                    lock: true,
                    icon: 'error',
                    time: 1,
                    content: '登录失败'
                });
            }

        }
    });
}