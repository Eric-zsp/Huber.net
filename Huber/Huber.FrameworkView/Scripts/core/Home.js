function logout() {
    $.cookie('Huber_Sign', '', { expires: 0 });
    window.location.href = '/user/login'
}
function modPawd(userId) {
    var html = '<table id="addusermedol">' +
        '<tr><td align="right">当前密码：</td><td><input  type="password" id="Home_modpwd_curpwd" /></td></tr>' +
        '<tr><td align="right">新密码：</td><td><input  type="password" id="Home_modpwd_newpwd" /><br/></tr>' +
        '<tr><td align="right">确认密码：</td><td><input  type="password" id="Home_modpwd_repwd" /><br/></tr>' +
        '</table>';

    art.dialog({
        title: '修改密码',
        content: html,
        lock: true,
        cancel: true,
        ok: function () {
            var curpwd = $("#Home_modpwd_curpwd").val();
            var newpwd = $("#Home_modpwd_newpwd").val();
            var repwd = $("#Home_modpwd_repwd").val();
            if (curpwd == '') {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '请输入当前密码'
                });
                return false;
            }
            if (newpwd == '') {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '请输入新密码'
                });
                return false;
            }
            if (newpwd != repwd) {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '两次密码输入的不一致'
                });
                return false;
            }
            $.ajax({
                url: '/User/modifyPwd',
                type: 'post',
                data: { opwd: Huber_diyMd5(userId, curpwd), pwd: Huber_diyMd5(userId, newpwd) },
                cache: false,
                success: function (txt) {
                    //if (!checkAjaxResult(txt)) {
                    //    return;
                    //}
                    if (txt == 1) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '密码修改成功'
                        });

                    }
                    else if(txt == 0){
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '当前密码输入不正确'
                        });
                    }
                    else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '密码修改失败'
                        });
                    }
                }
            });
        }
    });
}

var preActiveManu;
function clickMenu2Iframe(sender, murl) {
    if (sender.tagName == "DIV") {
        $("#contentPage").css("display", "");
        $("#contentDiv").css("display", "none");
        $("#contentPage").attr("src", murl);
        return;
    }
    if ($(preActiveManu) == $(sender)) {
        return;
    }
    $(preActiveManu).removeClass("active");
    $(sender).addClass("active");
    preActiveManu = $(sender);
   
    $("#contentPage").css("display", "");
    $("#contentDiv").css("display", "none");
    $("#contentPage").attr("src", murl);
}
function refreshPhoto() {
    $("img[id^='UserPhoto_refresh_']").each(function () {
        $(this).attr("src", $(this).attr("src")+"?t="+Math.random());
    });
}



