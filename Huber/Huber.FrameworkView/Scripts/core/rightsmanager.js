
var zTreeObj;
var Right_URL=[];
((function () {
    var setting = {
        view: {
            selectedMulti: false,
            showLine: true,
            showText: true,
            showIcon: false,
             addHoverDom: addHoverDom,
            removeHoverDom: removeHoverDom
        }
    }
    $.ajax({
        url: '/Right/GetRightTree',
        type: 'get',
        cache: false,
        success: function (txt) {
            //if (!checkAjaxResult(txt)) {
            //    return;
            //}
            if (txt != '') {
                var nodes = eval(txt);
                if (nodes != null) {
                    FillnodeName(nodes);
                    zTreeObj = $.fn.zTree.init($("#RightTreePannel"), setting, nodes);


                }
            }

        }
    });

})());
//将后台输出的实体类添加name children title属性
function FillnodeName(nodes) {
    for (var i = 0; i < nodes.length; i++) {
        nodes[i].name = nodes[i].Name;
        nodes[i].open = true;
        nodes[i].children = nodes[i].Children;
        nodes[i].title = nodes[i].Describe;
        Right_URL.push(nodes[i].Url);
        if (nodes[i].Children.length > 0) {
            FillnodeName(nodes[i].Children);
        }
    }
}

//节点点击事件
function nodeClick(event, treeId, treeNode) {

}
//为树的节点添加鼠标悬浮效果
function addHoverDom(treeId, treeNode) {
    var sObj = $("#" + treeNode.tId + "_span");
    if (treeNode.editNameFlag || $("#diySpanStr_" + treeNode.tId).length > 0) return;
    var diySpanStr = "<span id='diySpanStr_" + treeNode.tId + "'>";
    diySpanStr += "<span class='button add' id='addBtn_" + treeNode.tId + "' title='添加子权限' onfocus='this.blur();'></span>";
    if (treeNode.Id != 0)
        diySpanStr += "<span class='button edit' id='editBtn_" + treeNode.tId + "' title='修改权限' onfocus='this.blur();'></span>";
    if (treeNode.children.length == 0) {
        diySpanStr += "<span class='button remove' id='removeBtn_" + treeNode.tId + "' title='删除权限' onfocus='this.blur();'></span>";
    }
    diySpanStr += "</span>";

    sObj.after(diySpanStr);

    //添加按钮
    var addbtn = $("#addBtn_" + treeNode.tId);
    if (addbtn) addbtn.bind("click", function () {
        $("#rightid").val("0");
        $("#rightparentid").val(treeNode.Id);
        $("#rightLvL").val(treeNode.Level + 1);
        $("#rightname").val("");
        $("#rightUrl").val("");
        $("#nodeDescribe").val("");
        $("#nodeMenu").val(0);
        $('#RightEditPannel_addbtn').unbind("click");
        $('#RightEditPannel_addbtn').css("display", "");
        $('#RightEditPannel_addbtn').attr("disabled", false);
        $('#RightEditPannel_editbtn').css("display", "none");
        //是否显示 选择模块
        if (treeNode.Id == 0) {
            $.ajax({
                url: '/Right/GetModule',
                type: 'get',
                cache: true,
                success: function (data) {
                    if (data.length <= 0) {
                        art.dialog({
                            icon: 'error',
                            lock:true,
                            time: 1,
                            content: '请先添加模块'
                        });
                        $('#RightEditPannel_addbtn').hide();
                    }
                    else {
                        $("#nodeCategory").html(data);
                        $("#nodeCategory").parents("tr").show();
                    }
                }
            });
        }
        else {
            $("#nodeCategory").val(treeNode.Category).parents("tr").hide();
        }
        $('#RightEditPannel_addbtn').bind("click", function () {
            if ($("#rightname").val() == '') {
                art.dialog({ icon: 'error', lock: true, time: 1, content: '请输入权限名' });
                return;
            }
            if ($("#rightUrl").val() == '') {
                art.dialog({ icon: 'error', time: 1, lock: true, content: '请输入权限的url路径' });
                return;
            }
            $('#RightEditPannel_addbtn').attr("disabled", "disabled");
            var tnode = { Name: encodeURI($("#rightname").val()), Url: $("#rightUrl").val(), Level: $("#rightLvL").val(), ParentId: $("#rightparentid").val(), Describe: encodeURI($("#nodeDescribe").val()), IsMenu: $("#nodeMenu").val(), Category: $("#nodeCategory").val() };

            $.ajax({
                url: '/Right/AddRight',
                data: tnode,
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

                    $('#RightEditPannel_addbtn').attr("disabled", false);
                    if (txt > 0) {

                        var insertNodes = [];
                        tnode.Id = txt;
                        tnode.Name = $("#rightname").val();
                        tnode.Describe = $("#nodeDescribe").val();
                        tnode.Children = [];
                        insertNodes.push(tnode);
                        FillnodeName(insertNodes);
                        zTreeObj.addNodes(treeNode, tnode, true);
                        Right_URL.push($("#rightUrl").val());
                        art.dialog({
                            icon: 'succeed',
                            lock: true,
                            time: 1,
                            content: '添加成功！'
                        });

                    } else if (txt == -2) {
                        art.dialog({
                            icon: 'error',
                            lock: true,
                            time: 1,
                            content: 'URl已存在，请更改！'
                        });
                    }
                    else {
                        art.dialog({
                            icon: 'error',
                            lock: true,
                            time: 1,
                            content: '添加失败！'
                        });
                    }
                }
            });
        });
        return false;
    });
    //编辑按钮
    var editbtn = $("#editBtn_" + treeNode.tId);
    if (editbtn) editbtn.bind("click", function () {
        $("#rightid").val(treeNode.Id);
        var fatherNode = treeNode.getParentNode();
        $("#rightparentid").val(treeNode.ParentId);
        $("#rightLvL").val(treeNode.Level);
        $("#rightname").val(treeNode.name);
        $("#rightUrl").val(treeNode.Url);
        $("#nodeDescribe").val(treeNode.Describe);
        $('#nodeMenu').val(treeNode.IsMenu);
        $('#RightEditPannel_editbtn').unbind("click");
        $('#RightEditPannel_editbtn').css("display", "");
        $('#RightEditPannel_editbtn').attr("disabled", false);
        $('#RightEditPannel_addbtn').css("display", "none");
        $('#RightEditPannel_editbtn').bind("click", function () {
            if ($("#rightname").val() == '') {
                art.dialog({ icon: 'error', time: 1, lock: true, content: '请输入权限名' });
                return;
            }
            if ($("#rightUrl").val() == '') {
                art.dialog({ icon: 'error', time: 1, lock: true, content: '请输入权限的url路径' });
                return;
            }
            $('#RightEditPannel_editbtn').attr("disabled", "disabled");

            $.ajax({
                url: '/Right/EditRight',
                data: { Id: $("#rightid").text(), Name: encodeURI($("#rightname").val()), Url: $("#rightUrl").val(), Level: $("#rightLvL").text(), ParentId: $("#rightparentid").text(), Describe: encodeURI($("#nodeDescribe").val()), IsMenu: $('#nodeMenu').val() },
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

                    $('#RightEditPannel_editbtn').attr("disabled", false);
                    treeNode.name = $("#rightname").val();
                    treeNode.Name = $("#rightname").val();
                    treeNode.IsMenu = $('#nodeMenu').val();
                    treeNode.Describe = $("#nodeDescribe").val();
                    treeNode.Url = $("#rightUrl").val();

                    if (txt > 0) {

                        Right_URL.push($("#rightUrl").val());
                        art.dialog({
                            icon: 'succeed',
                            lock: true,
                            time: 1,
                            content: '修改成功！'
                        });

                    } else if (txt == -2) {
                        art.dialog({
                            icon: 'error',
                            lock: true,
                            time: 1,
                            content: 'URl已存在，请更改！'
                        });
                    }
                    else {
                        art.dialog({
                            icon: 'error',
                            lock: true,
                            time: 1,
                            content: '修改失败！'
                        });
                    }
                }
            });
        });
        return false;
    });
    //删除按钮
    var removebtn = $("#removeBtn_" + treeNode.tId);
    if (removebtn) removebtn.bind("click", function () {
        var tempnode = treeNode;
        var path = '/' + treeNode.name;
        while (tempnode.getParentNode() != null) {
            tempnode = tempnode.getParentNode();
            path += '/' + tempnode.name;
        }
        art.dialog({
            icon: 'question',
            content: '是否要删除"' + path + '"？',
            lock: true,
            button: [{
                name: '同意',
                callback: function () {
                    $.ajax({
                        url: '/Right/removeRight',
                        data: { id: treeNode.Id },
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
                            if (txt == "True") {
                                art.dialog({
                                    icon: 'succeed',
                                    lock: true,
                                    time: 1,
                                    content: '删除"' + path + '成功！"'
                                });
                                zTreeObj.removeNode(treeNode);
                                for (var i in Right_URL) {
                                    if (Right_URL[i] == treeNode.Url) {
                                        Right_URL.splice(i, 1);
                                        break;
                                    }
                                }

                            }
                            else {
                                art.dialog({
                                    icon: 'error',
                                    lock: true,
                                    time: 1,
                                    content: '删除"' + path + '失败！'
                                });
                            }
                        }
                    });

                },
                focus: true
            },
               {
                   name: '不同意',
                   callback: function () {
                   }

               }]
        });
        // zTreeObj.addNodes(treeNode, { id: (100 + newCount), pId: treeNode.id, name: "new node" + (newCount++) },true);
        return false;
    });
}
$("#rightUrl").focus(function (sender) {
    $.ajax({
        url: '/Right/GetAllAction',
        type: 'get',
        cache: true,
        success: function (result) {
            if (result) {
                var elements = "[";
                $.each(result, function (index, item) {
                    if (!exsitUrl(item))
                    {
                        elements += "\'" + item + "\',";
                    }
                });
                if (elements.length>1 ) {
                    elements = elements.substr(0, elements.length - 1);
                }
                elements += "]";
                //$("#rightUrl").attr("data-source", elements);

                $("#rightUrl").completer({ 
                    autocomplete: "off",
                    source: result,
                    suggest: true
                });
               
            }
        }
    });
});
///url是否在存在树中
function exsitUrl(url) {
    for (var node in Right_URL) {
        if (Right_URL[node] == url) {
            return true;
        }
    }
    return false;
}
//为树的节点移除鼠标悬浮效果
function removeHoverDom(treeId, treeNode) {
    $("#diySpanStr_" + treeNode.tId).unbind().remove();
};
