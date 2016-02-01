var User_operat = function () {
}
User_operat.reLoadDb = function () {
    $.ajax({
        url: '/Tool/ReloadDb',
        type: 'post',
        success: function (data) {
            if (data > 0) {
                art.dialog({
                    icon: 'succeed',
                    time: 1,
                    content: '更新数据库成功'
                });
            }
            else {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '更新数据库失败'
                });
            }

        }
    });
}