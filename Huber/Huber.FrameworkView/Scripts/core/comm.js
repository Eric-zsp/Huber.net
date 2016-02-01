function checkAjaxResult(txt) {
    if (txt == 'NL') { //失去登陆
        window.top.location.href = '/User/Login';
        return false;
    } else if (txt == 'notpage') { //404
        window.location.href = '/NotPageFound/_404';
        return false;
    } else if (txt == 'noright') { //没有权限
        window.location.href = '/NotPageFound/_404';
        return false;
    }
        //add by hgl 2014/1/8
    else if (txt == null || txt == '') {
        return false;
    }
    //end add
    return true;
}
