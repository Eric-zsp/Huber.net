var User_operat = function () {
}

//显示用户列表
User_operat.showList = function (index) {
    $.ajax({
        url: '/User/UserList?pageindex=' + index + '&pagesize=20&callback=User_operat.showList&userName=' + userNameText,
        type: 'get',
        cache: false,
        beforeSend: function () {
            //转圈
            // a0showBusy("contenlist", 48, "b");
        },
        success: function (txt) {
            //if (!checkAjaxResult(txt)) {
            //    return;
            //}
            if (txt) {
                $("#userlist").html(txt);
            }
        }
    });
}
var userNameText = "";

User_operat.showListWithName = function (index, userName) {
    
    userNameText = userName;
    $.ajax({
        url: '/User/UserList?pageindex=' + index + '&pagesize=10&callback=User_operat.showList&userName=' + userNameText,
        type: 'get',
        cache: false,
        beforeSend: function () {
            
        },
        success: function (txt) {
           
            if (txt) {
                $("#userlist").html(txt);
            }
        }
    });
}


//禁用用户
User_operat.disableUser = function (sender, uid) {
    $.ajax({
        url: '/User/disableUser?uid=' + uid,
        type: 'post',
        cache: false,
        success: function (data) {
            if (data > 0) {
                art.dialog({
                    icon: 'succeed',
                    time: 1,
                    content: '禁用用户成功'
                });
                $(sender).text("启用").attr("onclick", "User_operat.enableUser(this,'" + uid + "')");
            }
            else if (data == -2) {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '无法禁用超级管理员'
                });
            }
            else{
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '禁用用户失败'
                });
            }
        }
    });
}
//启用用户
User_operat.enableUser = function (sender, uid) {
    $.ajax({
        url: '/User/enableUser?uid=' + uid,
        type: 'post',
        cache: false,
        success: function (txt) {
            if (txt == 'True') {
                art.dialog({
                    icon: 'succeed',
                    time: 1,
                    content: '启用用户成功'
                });
                $(sender).text("禁用").attr("onclick", "User_operat.disableUser(this,'" + uid + "')");
            }
            else {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '启用用户失败'
                });
            }
        }
    });
}
//添加用户
User_operat.addUser = function () {
    art.dialog({
        title: '添加用户',
        content: document.getElementById("addusermedol"),
        lock: true,
        cancel: true,
        ok: function () {
            var userId = $.trim($("#User_operat_addUser_userId").val());
            var username = $.trim($("#User_operat_addUser_adduname").val());
            var status = document.getElementById("User_operat_addUser_status").checked ? 1 : 0;
            if (userId == '' || username == '') {
                art.dialog({
                    icon: 'error',
                    time: 2,
                    content: '登录名和昵称不能为空！'
                });
                return false;
            }
            var reg = /^[a-zA-Z1-9_]+$/;
            if (!reg.test(userId)) {
                art.dialog({
                    icon: 'error',
                    time: 2,
                    content: '登录名只能包含数字、英文字母、下划线'
                });
                return false;
            }
            var pwd = '123456';
            $.ajax({
                url: '/User/AddUser',
                type: 'post',
                data: { Uid: userId, Name: username, Status: status, Pwd: Huber_diyMd5(userId, pwd) },
                cache: false,
                success: function (txt) {
                    if (!checkAjaxResult(txt)) {
                        return;
                    }
                    if (txt == 1) {
                        art.dialog({
                            icon: 'succeed',
                            time: 2,
                            content: '添加用户成功,初始密码为' + pwd
                        });

                    }
                    else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '添加用户失败'
                        });
                    }

                }
            });
        }
    });
}
//修改用户
User_operat.modifyUserName = function (sender, uid) {
    var name = $(sender).text();
    var html = '<div id="modifyUsermedol">' +
       '<input style="display:block;" type="text" id="User_operat_modifyUser_modifyname" value="' + name + '" />' +
       '</div>';
    art.dialog({
        title: '修改用户',
        content: html,
        ok: function () {
            var username = $.trim($("#User_operat_modifyUser_modifyname").val());
            if (username == '') {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '请输入用户名'
                });
                return;
            }
            $.ajax({
                url: '/User/modifyUser?uid=' + uid + '&name=' + encodeURI(username),
                type: 'post',
                cache: false,
                beforeSend: function () {
                    //转圈
                    // a0showBusy("contenlist", 48, "b");
                },
                success: function (txt) {
                    //if (!checkAjaxResult(txt)) {
                    //    return;
                    //}
                    if (txt == 1) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '修改用户成功'
                        });
                        $(sender).html(username);
                    }
                    else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '修改用户失败'
                        });
                    }
                }
            });
        },
        cancel: true,
        id: 'EF893L'
    });

}

//删除用户
User_operat.deleteUser = function (sender) {
    art.dialog({
        title: '删除用户',
        block: true,
        okVal: '删除',
        cancel: true,
        content: '确认删除用户吗？',
        ok: function () {
            var uid = $(sender).attr("data-uid");
            $.ajax({
                url: '/User/DeleteUser',
                data: { id: uid },
                type: 'post',
                success: function (data) {
                    if (data > 0) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '删除用户成功'
                        });
                        $(sender).parents('tr').hide("slow");
                    }
                    else if (data == -2) {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '无法删除超级管理员'
                        });
                    }
                    else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '删除用户失败'
                        });
                    }
                }
            });
        }
    });
}
//重置密码
User_operat.resetUserPwd = function (uid) {

    var pwd = '123456';
    $.ajax({
        url: '/User/resetPwd?uid=' + uid + '&pwd=' + Huber_diyMd5(uid, pwd),
        type: 'post',
        cache: false,
        beforeSend: function () {
            //转圈
            // a0showBusy("contenlist", 48, "b");
        },
        success: function (txt) {
            //if (!checkAjaxResult(txt)) {
            //    return;
            //}
            if (txt == 1) {
                art.dialog({
                    icon: 'succeed',
                    time: 2,
                    content: '密码已重置为：' + pwd + '，请牢记！'
                });

            }
            else {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '重置密码失败'
                });
            }
        }
    });
}
//修改密码
User_operat.modifyUserPwd = function (uid) {

    var html = '<div id="modifyUserPwdmedol">' +
      '当前密码：<input style="display:block;" type="password" id="User_operat.modifyUserPwd.opwd"  />' +
      '新密码：<input style="display:block;" type="password" id="User_operat.modifyUserPwd.pwd"  />' +
      '确认密码：<input style="display:block;" type="password" id="User_operat.modifyUserPwd.npwd"  />' +
      '<input style="display:block;" type="button" value="添加" id="User_operat.modifyUserPwd.btn" />' +
      '</div>';
    art.dialog({
        title: '修改密码',
        content: html,
        id: 'EF893L'
    });
    $("#User_operat.modifyUserPwd.btn").click(function () {
        var opwd = $("#User_operat.modifyUserPwd.opwd").val();
        var pwd = $("#User_operat.modifyUserPwd.pwd").val();
        var npwd = $("#User_operat.modifyUserPwd.npwd").val();
        if (opwd == '') {
            art.dialog({
                icon: 'error',
                time: 1,
                content: '请输入当前密码'
            });
            return;
        }
        if (pwd == '') {
            art.dialog({
                icon: 'error',
                time: 1,
                content: '请输入新密码'
            });
            return;
        }
        if (pwd != npwd) {
            art.dialog({
                icon: 'error',
                time: 1,
                content: '两次密码输入的不一致'
            });
            return;
        }
        $.ajax({
            url: '/User/modifyPwd?uid=' + uid + '&opwd=' + Huber_diyMd5(uid, opwd) + '&pwd=' + Huber_diyMd5(uid, pwd),
            type: 'post',
            cache: false,
            beforeSend: function () {
                //转圈
                // a0showBusy("contenlist", 48, "b");
            },
            success: function (txt) {
                //if (!checkAjaxResult(txt)) {
                //    return;
                //}
                if (txt == 1) {
                    art.dialog({
                        icon: 'succeed',
                        time: 1,
                        content: '修改用户成功'
                    });

                } else if (txt == 0) {
                    art.dialog({
                        icon: 'error',
                        time: 1,
                        content: '当前密码输入不正确！'
                    });
                }
                else {
                    art.dialog({
                        icon: 'error',
                        time: 1,
                        content: '修改密码失败'
                    });
                }
            }
        });
    });
}
//角色管理
User_operat.roleManage = function (uId, roleIds) {
    $.ajax({
        url: '/User/GetUserRoles',
        type: 'post',
        data: { uId: uId, roldIds: roleIds },
        success: function (data) {
            art.dialog({
                lock: true,
                content: data
            });
        }
    });
}
User_operat.RemoveUserRole = function (uId, roleId) {
    $.ajax({
        url: '/User/RemoveUserRole',
        type: 'post',
        data: { uId: uId, roleId: roleId },
        success: function (data) {
            if (data) {
                art.dialog({
                    icon: 'succeed',
                    time: 1,
                    content: '移除角色成功'
                });
            } else {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: '移除角色失败'
                });
            }
        }
    });
}
User_operat.addUserRole = function (uId) {
    $.ajax({
        url: '/User/RoleList',
        data: { uId: uId },
        success: function (data) {
            art.dialog({
                lock: true,
                content: data,
                close: function () {
                    User_operat.roleManage._pageIndex = 1;
                }
            });
        }
    });
}
User_operat.roleManage._pageIndex = 1;
User_operat.roleGetMore = function (pageSize) {
    $.ajax({
        url: '/User/GetRoles',
        data: { pageIndex: User_operat.roleManage._pageIndex, pageSize: pageSize },
        success: function (data) {
            User_operat.roleManage._pageIndex += 1;
            $("#User_operat_roleManage_roleList").append(data);
        }
    });
}
//添加角色
User_operat.roleAdd = function (uId) {
    var roldIds = [];
    $("#User_operat_roleManage_roleList input[type='checkbox']:checked").each(function () {
        roldIds.push($(this).val());
    });
    if (roldIds.length === 0) {
        art.dialog({
            icon: 'error',
            content: "请至少选择一个角色！",
            time: 2
        });
        return;
    }
    $.ajax({
        url: '/User/AddRoles',
        data: { uId: uId, roleIds: roldIds.join(",") },
        type: 'post',
        success: function (data) {
            if (data) {
                art.dialog({
                    icon: 'error',
                    content: "添加角色成功",
                    time: 1
                });
            } else {
                art.dialog({
                    icon: 'error',
                    content: "添加角色失败",
                    time: 1
                });
            }
        }
    });
}

User_operat.roleManage2 = function (sender) {
    var zTreeObj;
    var setting = {
        view: {
            showLine: true,
            showText: true,
            showIcon: false,
            selectedMulti: false
        },
        callback: {
            onClick: function (event, treeId, treeNode) {
                zTreeObj.checkNode(treeNode, !treeNode.checked, true, false);
            }
        },
        check: {
            enable: true,
            chkStyle: "checkbox"
        }
    }
    $.ajax({
        url: '/User/GetRoles2',
        type: 'get',
        cache: false,
        success: function (txt) {
            if (txt != '') {
                var roleIds = $(sender).attr("data-roles");
                var userId = $(sender).attr("data-userId");
                var nodes = eval(txt);
                if (nodes != null) {
                    FillnodeName(roleIds, nodes);
                    zTreeObj = $.fn.zTree.init($("#userRolePannel"), setting, nodes);
                }
                art.dialog({
                    lock: true,
                    content: document.getElementById("userRolePannel"),
                    ok: function () {
                        var checkedNodes = zTreeObj.getCheckedNodes(true);
                        var checkedArray = ",";
                        for (var i = 0; i < checkedNodes.length; i++) {
                            checkedArray += (checkedNodes[i].Id + ",");
                        }
                        $.ajax({
                            url: '/User/UpdateRights',
                            type: 'post',
                            data: { userId: userId, roleIds: checkedArray },
                            success: function (data) {
                                if (data) {
                                    art.dialog({
                                        icon: 'succeed',
                                        time: 1,
                                        content: '修改权限成功'
                                    });
                                    $(sender).attr("data-roles", checkedArray);
                                } else {
                                    art.dialog({
                                        icon: 'error',
                                        time: 1,
                                        content: '修改权限失败'
                                    });
                                }
                            }
                        });
                    },
                    cancel: function () {

                    }
                });
            }
        }
    });
}
function FillnodeName(roleIds, nodes) {
    for (var i = 0; i < nodes.length; i++) {
        nodes[i].name = nodes[i].Name;
        nodes[i].open = true;
        nodes[i].children = [];
        nodes[i].title = nodes[i].Name;
        if (roleIds.indexOf(',' + nodes[i].Id + ',') > -1) {
            nodes[i].checked = true;
        }
    }
}

$(function () {

    $("#UserSearch").keypress(function (e) {
        if (e.keyCode == 13) {
            User_operat.showListWithName(1, $(this).val());
}
    });
});