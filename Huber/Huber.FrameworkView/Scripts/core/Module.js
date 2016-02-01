var Module_operat = function () {
}
//添加模块
Module_operat.addModule = function () {
    $("#moduleAdd_name").val('');
    $("#moduleAdd_author").val('');
    //$("#moduleAdd_defaultController").val('');
    //$("#moduleAdd_defaultAction").val('');
    $("#moduleAdd_describe").val('');
    $("#moduleAdd_file").val('');
    art.dialog({
        title: '添加模块',
        lock: true,
        content: document.getElementById("moduleAddContainer"),
        ok: function () {
            var name = $("#moduleAdd_name").val();
            var author = $("#moduleAdd_author").val();
            //var controller = $("#moduleAdd_defaultController").val();
            //var action = $("#moduleAdd_defaultAction").val();
            var describe = $("#moduleAdd_describe").val();
            if ($("#moduleAdd_file").val().length < 1) {
                art.dialog({icon: 'error',time: 1,content: '请选择插件包文件'});
                return false;
            }
            if (name.length < 1) {
                art.dialog({ icon: 'error', time: 1, content: '请输入插件名称' });
                return false;
            }
            if (author.length < 1) {
                art.dialog({ icon: 'error', time: 1, content: '请输入负责人' });
                return false;
            }
            //if (controller.length < 1) {
            //    art.dialog({ icon: 'error', time: 1, content: '请输入Controller' });
            //    return false;
            //}
            //if (action.length < 1) {
            //    art.dialog({ icon: 'error', time: 1, content: '请输入action' });
            //    return false;

            //}
            if (describe.length < 1) {
                art.dialog({ icon: 'error', time: 1, content: '请输入描述' });
                return false;

            }
            $.ajaxFileUpload({
                url: '/Module/AddModule', //用于文件上传的服务器端请求地址
                type: 'post',
                secureuri: false, //是否需要安全协议，一般设置为false
                fileElementId: 'moduleAdd_file', //文件上传域的ID
                dataType: 'json', //返回值类型 一般设置为json
                data: { Name: name, Describe: describe, Author: author },
                success: function (data, status)  //服务器成功响应处理函数
                {
                    debugger;
                    if (data > 0) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '添加模块成功'
                        });
                        Module_operat.showList();
                    } else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '添加模块失败'
                        });
                        return false;
                    }
                    return true;
                },
                error: function (data, status, e)//服务器响应失败处理函数
                {

                    debugger;
                    art.dialog({
                        icon: 'error',
                        time: 1,
                        content: '添加模块失败'
                    });
                    return false;
                }
            });
        },
        cancel: true
    });



}

var searchNameTxt = '';
//模块列表
Module_operat.showListWithName = function (pageIndex, searchName) {
   
    searchNameTxt = searchName;
    if (!pageIndex) {
        pageIndex = 1;
    }
   var pageSize = 10;
    $.ajax({
        url: '/Module/ModuleList',
        type: 'get',
        cache: false,
        data: { pageIndex: pageIndex, pageSize: pageSize, searchName: searchNameTxt },
        success: function (data) {
            $("#moduleList").html(data);
        }
    });
}

Module_operat.showList = function (pageIndex) {

    if (!pageIndex) {
        pageIndex = 1;
    }
    var pageSize = 10;
    $.ajax({
        url: '/Module/ModuleList',
        type: 'get',
        cache: false,
        data: { pageIndex: pageIndex, pageSize: pageSize, searchName: searchNameTxt },
        success: function (data) {
            $("#moduleList").html(data);
        }
    });
}



//启用、禁用模块 
Module_operat.changeStatus = function (sender, id) {
     
    var status = $(sender).attr("status");
    var status = 0;
    if ($(sender).html() == "启用")
        status = 1;
    $.ajax({
        url: '/Module/SetStatus',
        type: 'post',
        data: { id: id, status: status },
        success: function (data) {
            if (data > 0) {
                art.dialog({
                    icon: 'succeed',
                    time: 1,
                    content: $(sender).html() + '模块成功'
                });
                $(sender).html((status == 1 ? "停用" : "启用"));

                // $("#module_" + id + "_status").html(status);
            } else {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: str + '模块失败'
                });
                return false;
            }
            return true;
        }
    });
}
//删除模块
Module_operat.delModule = function (sender, id) {
    if (!Module_operat.checkModuleStatus(id)) {
        return;
    }
    art.dialog({
        title: '删除模块',
        block: true,
        okVal: '删除',
        cancel: true,
        content: '确认删除模块吗？',
        ok: function () {
            $.ajax({
                url: '/Module/DelModule',
                type: 'post',
                data: { id: id },
                success: function (data) {
                    if (data > 0) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '删除模块成功'
                        });
                        $(sender).parents('tr').hide("slow");
                    } else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '删除模块失败'
                        });
                        return false;
                    }
                    Module_operat.showList();
                    return true;
                }
            });
        }
    });

}
//升级模块
Module_operat.uploadModule = function (id,name) {
    $("#moduleUpdate_ID").val(id);
    $("#moduleUpdate_name").val(name);
    $("#moduleUpdate_file").val('');
    art.dialog({
        title: '升级模块',
        lock: true,
        content: document.getElementById("moduleUploadContainer"),
        ok: function () {
            if ($("#moduleUpdate_file").val().length < 1) {
                art.dialog({ icon: 'error', time: 1, content: '请选择插件包文件' });
                return false;
            }
            $.ajaxFileUpload({
                url: '/Module/UpdateModule', //用于文件上传的服务器端请求地址
                type: 'post',
                secureuri: false, //是否需要安全协议，一般设置为false
                fileElementId: 'moduleUpdate_file', //文件上传域的ID
                dataType: 'json', //返回值类型 一般设置为json
                data: { pluginId: id },
                success: function (data, status)  //服务器成功响应处理函数
                {
                    if (data > 0) {
                        art.dialog({
                            icon: 'succeed',
                            time: 1,
                            content: '模块升级成功'
                        });
                        Module_operat.showList();
                    } else {
                        art.dialog({
                            icon: 'error',
                            time: 1,
                            content: '模块升级失败'
                        });
                        return false;
                    }
                    return true;
                },
                error: function (data, status, e)//服务器响应失败处理函数
                {
                    art.dialog({
                        icon: 'error',
                        time: 1,
                        content: '添加模块失败'
                    });
                    return false;
                }
            });
        },
        okVal: '升级',
        cancel: true
    });
}

//在菜单中显示、隐藏
Module_operat.showMenu = function (sender, id) {
    if (!Module_operat.checkModuleStatus(id)) {
        return;
    }
    var status = 0;
    if ($(sender).html() == "显示") {
        status = 1;
    }


    $.ajax({
        url: '/Module/SetMenuShow',
        type: 'post',
        data: { id: id, status: status },
        success: function (data) {
            if (data > 0) {
                art.dialog({
                    icon: 'succeed',
                    time: 1,
                    content: $(sender).html() + '模块成功'
                });
                $(sender).html((status == 1 ? "隐藏" : "显示"));
            } else {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: str + '模块失败'
                });
                return false;
            }
            return true;
        }
    });
}
Module_operat.checkModuleStatus = function (id) {
    var status = $("#module_" + id + "_status").html();
    if (status == 1) {
        art.dialog({
            icon: 'error',
            time: 2,
            content: "请先停止模块！"
        });
        return false;
    }
    return true;
}

Module_operat.ModifyIconId = 0;
Module_operat.changeIconShow = function (id) {
    Module_operat.ModifyIconId = id;
    art.dialog({
        title: '修改图标',
        content: document.getElementById("moduleIconContainer"),
        id: 'dialog_moduleIconContainer'
    });
}
//设置图标
Module_operat.changeIcon = function (sender) {
    var _css=$(sender).find("i").attr("class");
    $.ajax({
        url: '/Module/UpadteModuleIcon',
        type: 'post',
        data: { Id: Module_operat.ModifyIconId, Icon: _css },
        success: function (data) {
            if (data > 0) {
               
                $("#mList_icon_i_" + Module_operat.ModifyIconId).attr("class", _css);
                art.dialog.list['dialog_moduleIconContainer'].close();
            } else {
                art.dialog({
                    icon: 'error',
                    time: 1,
                    content: str + '修改失败'
                });
                return false;
            }
            return true;
        }
    });
}


$(function () {


    $("#ModuleSearch").keypress(function (e) {
        if (e.keyCode == 13) {
            Module_operat.showListWithName(1, $(this).val());
        }
    });
    $("#moduleIconContainer .span2").each(function () {
        //alert(1);
        $(this).attr("title", "点我即可设置为模块图标");
        $(this).bind("click",function(){
            Module_operat.changeIcon($(this));
        });
    });
});
