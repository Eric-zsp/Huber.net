var Role_operat = function () {
}
Role_operat.showList = function (pageIndex) {
    $.ajax({
        url: '/Role/RoleList',
        data: { pageIndex: pageIndex, pageSize: 10, callBack: 'Role_operat.showList', RoleName: RoleNameText },
        cache: false,
        success: function (data) {
            $("#roleList").html(data);
        }
    });
}
var RoleNameText = "";

Role_operat.showListWithName = function (pageIndex, RoleName) {
    
    RoleNameText = RoleName;
    $.ajax({
        url: '/Role/RoleList',
        data: { pageIndex: pageIndex, pageSize: 10, callBack: 'Role_operat.showList' ,RoleName:RoleNameText},
        cache: false,
        success: function (data) {
            $("#roleList").html(data);
        }
    });
}

//添加角色
Role_operat.addRole = function () {
    art.dialog({
        title: '添加角色',
        block: true,
        content: document.getElementById("addRoleDiv"),
        cancel: true,
        okVal: '添加',
        ok: function () {
            var name = $.trim($("#add_roleName").val());
            var isSuper = document.getElementById("add_isSuper").checked ? 1 : 0;
            if (name.length == 0) {
                art.dialog({
                    icon: 'error',
                    time: 2,
                    content: '角色名称不能为空！'
                });
                return false;
            }
            $.ajax({
                url: '/Role/AddRole',
                data: { name: name, isSuper: isSuper },
                type: 'post',
                success: function (data) {
                    if (data) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '添加角色成功'
                        });
                    } else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '添加角色失败'
                        });
                    }
                }
            });
        }
    });
}
//删除角色
Role_operat.deleteRole = function (sender, roleId) {
    art.dialog({
        title: '删除角色',
        block: true,
        okVal: '删除',
        cancel: true,
        content: '确认删除角色吗？',
        ok: function () {
            $.ajax({
                url: '/Role/DeleteRole',
                data: { roleId: roleId },
                type: 'post',
                success: function (data) {
                    if (data) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '删除角色成功'
                        });
                        $(sender).parents('tr').hide("slow");
                    } else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '删除角色失败'
                        });
                    }
                }
            });
        }
    });
}
//修改角色
Role_operat.updateRole = function (sender, roleId) {
    var roleName = $(sender).attr("data-rolename");
    var isSuper = $(sender).attr("data-rolesuper");
    $("#update_roleName").val(roleName);
    document.getElementById("update_isSuper").checked = (isSuper == 1 ? true : false);
    art.dialog({
        title: '修改角色',
        block: true,
        ok: function () {
            var uRoleName = $.trim($("#update_roleName").val());
            var uIsSuper = document.getElementById("update_isSuper").checked ? 1 : 0;
            if (uRoleName.length == 0) {
                art.dialog({
                    icon: 'error',
                    time: 2,
                    content: '角色名称不能为空！'
                });
                return false;
            }
            $.ajax({
                url: '/Role/UpdateRole',
                data: { Id: roleId, Name: uRoleName, IsSuper: uIsSuper },
                type: 'post',
                success: function (data) {
                    if (data) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '修改角色成功'
                        });
                        $(sender).attr("data-rolename", uRoleName);
                        $(sender).attr("data-rolesuper", uIsSuper);
                        $("#role_" + roleId + "_roleName").html(uRoleName);
                        $("#role_" + roleId + "_roleSuper").html(uIsSuper ? "是" : "否");
                    } else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '修改角色失败'
                        });
                    }
                }
            });
        },
        okVal: '修改',
        cancel: true,
        content: document.getElementById("updateRoleDiv")
    });
}
Role_operat.updateRoleSubmit = function () {

}
Role_operat.rightManage = function (sender, rightId) {
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
        url: '/Role/GetRights',
        type: 'get',
        cache: false,
        success: function (txt) {

            if (txt != '') {
                var rightids = $(sender).attr("data-rights");
                var nodes = eval(txt);
                if (nodes != null) {
                    FillnodeName(rightids, nodes);
                    zTreeObj = $.fn.zTree.init($("#RoleRightPannel"), setting, nodes);
                }
                art.dialog({
                    content: document.getElementById("RoleRightPannel"),
                    ok: function () {
                        var checkedNodes = zTreeObj.getCheckedNodes(true);
                        var checkedArray = ",";
                        for (var i = 0; i < checkedNodes.length; i++) {
                            checkedArray += (checkedNodes[i].Id + ",");
                        }
                        $.ajax({
                            url: '/Role/UpdateRight',
                            type: 'post',
                            data: { rightId: rightId, rights: checkedArray },
                            success: function (data) {
                                if (data) {
                                    art.dialog({
                                        icon: 'succeed',
                                        time: 1,
                                        content: '修改权限成功'
                                    });
                                    $(sender).attr("data-rights", checkedArray);
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
}//将后台输出的实体类添加name children title属性
function FillnodeName(rightids, nodes) {
    for (var i = 0; i < nodes.length; i++) {
        nodes[i].name = nodes[i].Name;
        nodes[i].open = true;
        nodes[i].children = nodes[i].Children;
        nodes[i].title = nodes[i].Describe;
        if (rightids.indexOf(',' + nodes[i].Id + ',') > -1) {
            nodes[i].checked = true;
        }
        if (nodes[i].Children.length > 0) {
            FillnodeName(rightids, nodes[i].Children);
        }
    }
}

$(function () {


    Role_operat.showList(1);

    $("#RoleSearch").keypress(function (e) {
        if (e.keyCode == 13) {
            Role_operat.showListWithName(1, $(this).val());

        }


    });
});