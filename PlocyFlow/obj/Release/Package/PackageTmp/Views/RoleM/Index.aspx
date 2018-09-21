<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="PlocyFlow.DAL.CommonUtility" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language="javascript">
        function selectAll(t) {
            $("input[name='plist']").attr("checked", t.checked);
        }
        function setCk() {
            var roleDrop = $("#roleList option:selected").val();
            $("#roleid").val(roleDrop);
            $("#allck").attr("checked", false);
            
            $("input[name='plist']").each(function () {
                var rdlist = this.attributes["rdlist"].value;
                this.checked = false;
                if (rdlist != undefined) {
                    var l = rdlist.split(",");
                    for (var i = 0; i < l.length; i++) {
                        var t = l[i];
                        if (t != "") {
                            t = parseInt(t);
                            if (t == parseInt(roleDrop))
                                this.checked = true;
                        }
                    }
                }
            });
        }
        function collectCk() {
            var pagelist = $("#pagelist");
            pagelist.val("");
            var s = "";
            $("input[name='plist']:checked").each(function () {
                var pid = this.attributes["pid"].value;
                s = s + pid + ",";
            });
            if (s != "")
                s = s.substr(0, s.length - 1);
            pagelist.val(s);
        }
        $(function () {
            setCk();
        });
    </script>
    <center>
        <div class="percent_100 t_l ml_3">
            <div class="percent_100 lf">
                <table cellspacing="0" cellpadding="0" class="table_blue percent_100 mt_10 ml_10 micblack">
                    <tbody>
                        <tr>
                            <td style="width: 20%;" class="lf">
                                角色
                            </td>
                            <td style="width: 80%;" class="lf">
                                <%=Html.DropDownList("roleList", ViewData["rList"] as SelectList, new { @class = "micblack",onchange="setCk();$('#lblMsg').text('');" })%>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 20%;" class="lf">
                                权限页面
                            </td>
                            <td style="width: 80%;" class="lf">
                                <input type="checkbox" id="allck" onclick="selectAll(this);" /><span class="">全选/反选</span>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 20%;" class="lf">
                                &nbsp
                            </td>
                            <td style="width: 80%; vertical-align: top; padding-top: 10px; padding-bottom: 10px;"
                                align="left">
                                <%  var ckList = ViewData["ckList"] as List<RoleToPage>;
                                    if (ckList != null && ckList.Count > 0)
                                    {
                                        foreach (var ck in ckList)
                                        {%>
                                <li style="width: 120px; float: left;">
                                    <input type="checkbox" name="plist" class="ml_10" pid="<%=ck.pageId %>" rdlist="<%=ck.roleList%>" />
                                    <span class="micblack">
                                        <%=ck.pageName %></span> </li>
                                <%}
                }
                                %>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <%using (Html.BeginForm("saveRole", "RoleM", FormMethod.Post, new { style = "display:inline-block;" }))
                                  { %>
                                <input type="submit" value="保存" class="btn  ml_10 micblack" onclick="collectCk();" />
                                <span id="lblMsg" class=" ml_10 msg">
                                    <%=ViewData["msg"] %></span>
                                <input type="hidden" id="roleid" name="roleid" />
                                <input type="hidden" id="pagelist" name="pagelist" />
                                <% } %>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </center>
</asp:Content>
