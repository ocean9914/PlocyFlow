<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="PlocyFlow.DAL.Entity" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
 <%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    function selectAll(t) {
        $("input[name='plist']").attr("checked", t.checked);
    }
    var pau = new Array();
    function setCk() {
        var pid = $("#proList option:selected").val();
        $("#pid").val(pid);
        var aid = $("#aprList option:selected").val();
        $("#aid").val(aid);
        $("#allck").attr("checked", false);
        $("input[name='plist']").each(function () {
            var uid = this.attributes["uid"].value;
            this.checked = false;
            if (uid != undefined) {
                for (var i = 0; i < pau.length; i++) {
                    var t = pau[i];
                    if (t != "") {
                        var l = t.split("#");
                        if (l.length >= 3) {
                            var cpid = l[0];
                            var caid = l[1];
                            var cuid = l[2];
                            if (pid != "" && aid != "" && uid != "" && pid == cpid && aid == caid && uid == cuid)
                                this.checked = true;
                        }
                    }
                }
            }
        });
    }
    function collectCk() {
        var ulist = $("#ulist");
        ulist.val("");
        var s = "";
        $("input[name='plist']:checked").each(function () {
            var uid = this.attributes["uid"].value;
            s = s + uid + ",";
        });
        if (s != "")
            s = s.substr(0, s.length - 1);
        ulist.val(s);
    }
    $(function () {
        var pid = $("#proList option:selected").val();
        $("#pid").val(pid);
        var aid = $("#aprList option:selected").val();
        $("#aid").val(aid);
        var paul = $("#paulist").val();
        if (paul != "")
            pau = paul.split(",");
        setCk();
    });
    </script>
   <div class="percent_100 t_l ml_3">
            <div class="percent_100 lf">
                <table cellspacing="0" cellpadding="0" class="table_blue percent_100 mt_10 ml_10 micblack">
                    <tbody>
                        <tr>
                            <td style="width: 25%;" class="lf">
                                申报政策
                            </td>
                            <td style="width: 25%;" class="lf">
                                <%=Html.DropDownList("proList", ViewData["pList"] as SelectList, new { @class = "micblack",onchange="setCk();$('#lblMsg').text('');" })%>
                            </td>
                            <td style="width: 25%;" class="lf">
                                审批类型
                            </td>
                            <td style="width: 25%;" class="lf">
                                <%=Html.DropDownList("aprList", ViewData["aList"] as SelectList, new { @class = "micblack",onchange="setCk();$('#lblMsg').text('');" })%>
                            </td>
                        </tr>
                        <tr>
                            <td class="lf">
                                用户列表
                            </td>
                            <td  colspan="3" class="lf">
                                <input type="checkbox" id="allck" onclick="selectAll(this);" /><span class="">全选/反选</span>
                            </td>
                        </tr>
                        <tr>
                            <td  class="lf">
                                &nbsp
                            </td>
                            <td style=" vertical-align: top; padding-top: 10px; padding-bottom: 10px;" colspan="3"
                                align="left">
                                <%  var ckList = ViewData["ckList"] as List<t_user>;
                                    if (ckList != null && ckList.Count > 0)
                                    {
                                        foreach (var ck in ckList)
                                        {%>
                                <li style="width: 120px; float: left;">
                                    <input type="checkbox" name="plist" class="ml_10" uid="<%=ck.id %>"  />
                                    <span class="micblack">
                                        <%=ck.user_name %></span> </li>
                                <%}
                }
                                %>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="center">
                                <%using (Html.BeginForm("saveAssign", "AssignU", FormMethod.Post, new { style = "display:inline-block;" }))
                                  { %>
                                <input type="submit" value="保存" class="btn  ml_10 micblack" onclick="collectCk();" />
                                <span id="lblMsg" class=" ml_10 msg">
                                    <%=ViewData["msg"] %></span>
                                <input type="hidden" id="ulist" name="ulist" />
                                <input type="hidden" id="pid" name="pid" />
                                <input type="hidden" id="aid" name="aid" />
                                <% } %>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
         <input type="hidden" id="paulist" name="paulist"  value="<%=ViewData["paul"] %>"/>
</asp:Content>
