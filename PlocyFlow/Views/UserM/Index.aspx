<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="PlocyFlow.DAL.Entity" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language="javascript">
        function setCk(t) {
            var roleDrop = $(t).val();
            $("#roleid").val(roleDrop);
        }
        function edit(t) {
            var td = $(t.parentNode.parentNode);
            var tr = td.parent();
            var id = td.children(".roleid").val();
            var td2 = tr.children(".roleN");
            td2.empty();
            var drop = $("#roleList").clone().val(id).removeAttr("onchange");
            td2.append(drop);
            var esp = td.children(".ed");
            var usp = td.children(".up");
            esp.hide();
            usp.show();
        }
        function update(t) {
            var td = $(t.parentNode.parentNode);
            var tr = td.parent();
            var id = td.children(".id").val();
            var rolenameTxt = td.children(".rolename");
            var td2 = tr.children(".roleN");
            var drop = td2.children().first();
            var roleid = drop.val();
            $("#uidTxt").val(id);
            $("#roleIdTxt").val(roleid);
            $("#upBut").click();
        }
        function cacel(t) {
            var td = $(t.parentNode.parentNode);
            var tr = td.parent();
            var rolename = td.children(".rolename").val();
            var td2 = tr.children(".roleN");
            td2.empty();
            td2.text(rolename);
            var esp = td.children(".ed");
            var usp = td.children(".up");
            esp.show();
            usp.hide();
        }
    </script>
    <script language="javascript" type="text/javascript">
        var ulist;
        function getAll() {
            if (ulist == undefined) {
                ulist = new Array();
                $.get("GetUserList?jsoncallback=?", function (data) {
                    var ss = data.split(",");
                    $(ss).each(function (i, n) {
                        if (n != "") {
                            ulist[i] = n;
                        }
                    });
                });
            }
        }
        getAll();
        var ct = 0;
        function loadUList(t) {
            if (!hasupdown()) {
                var ld = $(".ajax");
                ld.empty();
                if (ulist == undefined) {
                    getAll();
                }
                var s = $.trim(t.value);
                var l = s.length;
                ct = 0;
                if (ulist.length > 0 && l > 0) {
                    for (var i = 0; i < ulist.length && ct < 10; i++) {
                        var name = ulist[i];
                        if (name.length >= l && name.substr(0, l) == s) {
                            ld.append("<li class='micblack ml_3 ali' name='list' onclick='setValue(this);'>" + name + "</li>");
                            ct++
                        }
                    }
                    var curt = $(t);
                    var p = curt.position();
                    ld.offset({ top: p.top + curt.height() + 3, left: p.left + 10 });
                    ld.width($(t).width());
                    ld.show();
                }
            }
        }
        var idx = 0;
        function closeAjax() {
            var cureid = document.activeElement.id;
            if (cureid != "ajax") {
                $(".ajax").hide();
                idx = 0;
            }
        }
        function setValue(t) {
            var v = $(t).text();
            $("[id$=userTxt]").val(v);
            $(".ajax").hide();
            idx = 0;
        }
        function hasupdown() {
            var cd = event.keyCode;
            var up = 38;
            var down = 40;
            if (cd == up || cd == down) return true;
            else return false;
        }
        function selectList(t) {
            var cd = event.keyCode;
            var up = 38;
            var down = 40;
            if (cd == up || cd == down) {
                var ld = $(".ajax");
                ld.show();
                if (idx < ct) {
                    if (cd == up)
                        idx--;
                    else
                        idx++;

                }
                else
                    idx = 0;
                if (ct > 0) {
                    ld.children().removeClass("sel");
                    var curel = ld.children().eq(idx);
                    curel.addClass("sel");
                    $("[id$=userTxt]").val(curel.text());

                }
            }
        }
       
    </script>
    <center>
        <div class="percent_100 t_l ml_5 pd_10">
            <div class="percent_100 lf">
                <% using (Html.BeginForm("AddUser", "UserM", FormMethod.Post, new { style = "display:inline-block;", autocomplete = "off" }))
                   {  %>
                <span class="micblack">用户名:</span>
                <input type="text" id="userTxt" name="userTxt" class="txtd bd ml_10" onkeyup="loadUList(this);"
                    onblur="closeAjax();" onkeydown="selectList(this);" />
                <span class="micblack ml_10">角色:</span>
                <%=Html.DropDownList("roleList", ViewData["rList"] as SelectList, new { @class = "micblack bd",onchange="setCk(this);$('#lblMsg').text('');" })%>
                <input type="submit" value="新增" class="btn  ml_10 micblack" />
                &nbsp &nbsp <span id="lblMsg" class=" ml_10 msg">
                    <%=ViewData["msg"] %></span>
                <input type="hidden" id="roleid" name="roleid" value="<%=ViewData["defaulRoleID"]%>" />
                <% } %>
            </div>
            <div class="percent_100 lf">
                <table cellpadding="0" cellspacing="0" class="mt_10 percent_100">
                    <thead>
                        <tr class="micblack" style="background-color: #cbe7ee; font-weight: bold; color: #21768f;">
                            <td style="width: 80px;">
                                编号
                            </td>
                            <td style="width: 180px;">
                                登录名
                            </td>
                            <td style="width: 160px;">
                                角色
                            </td>
                            <td style="width: 80px;">
                                操作
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <% int rowIndex = 0;
                           var data = ViewData["dataSouce"] as List<v_user_role>;
                           if (data != null && data.Count > 0)
                           {
                               foreach (var tr in data)
                               { %>
                        <tr style="background-color: #F7F6F3; color: #333333; height: 20px;">
                            <td>
                                <%=++rowIndex %>
                            </td>
                            <td>
                                <%=tr.user_name %>
                            </td>
                            <td class="roleN">
                                <%=tr.name %>
                            </td>
                            <td>
                                <span class="ed">
                                  <a class="comlink" href="javascript:void(0);" onclick="javascript:edit(this);">编辑</a> 
                                  <%=Html.ActionLink("删除", "delUser", new { id = tr.id }, new {@class="comlink"})%>
                                </span>
                                <span class="up hid">
                                  <a class="comlink" href="javascript:void(0);" onclick="javascript:update(this);">更新</a>
                                  <a class="comlink" href="javascript:void(0);" onclick="javascript:cacel(this);">取消</a>
                                </span>
                                <input type="hidden" class="id" value="<%=tr.id%>" />
                                <input type="hidden" class="rolename" value="<%=tr.name%>" />
                                <input type="hidden" class="roleid" value="<%= tr.role_id%>" />
                            </td>
                        </tr>
                        <% }
                    } %>
                    </tbody>
                </table>
            </div>
           <div id="ajax" class="ajax bd hid"></div>
        </div>
        <%using (Html.BeginForm("UpdateUser", "UserM", FormMethod.Post, new { style = "display:none;" }))
          {  %>
        <%=Html.TextBox("uidTxt")%>
        <%=Html.TextBox("roleIdTxt") %>
        <input type="submit" id="upBut" />
        <% } %>
    </center>
</asp:Content>
