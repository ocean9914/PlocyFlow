<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="PlocyFlow.DAL.Entity" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <center>
        <div class=" t_l ml_5" style="width:690px;">
            <div class="lf">
                <table cellpadding="0" cellspacing="0" class="mt_10 percent_100">
                    <tbody>
                        <tr>
                            <td style="width: 30%; text-align: center;">
                                <span class="micblac ml_10">基础列表</span>
                            </td>
                            <td style="width: 70%; text-align: left;">
                                <%using (Html.BeginForm("AddPolicyName", "BaseMain", FormMethod.Post, new { style = "display:inline-block;" }))
                                 {  %>
                                 <span class="micblack ml_10">政策标识</span>
                                 <%=Html.TextBox("idTxt","",new {@class="micblack ml_5 bd",style="width:80px;"})%>&nbsp
                                <span class="micblack ml_10">政策名称</span>
                                <%=Html.TextBox("nameTxt", "", new { @class = "micblack ml_5 bd", style = "width:80px;" })%>&nbsp
                                <input type="submit" value="新增" class="btn  ml_10 micblack" />
                                &nbsp &nbsp <span id="lblMsg" class=" ml_5 msg">
                                    <%=ViewData["msg"] %></span>
                                <% } %>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%; vertical-align: top;" align="center">
                                <div class="bd mt_10 lf pg_5" style="height: 200px; width: 120px;">
                                    <%=Html.ActionLink("部门表维护","Index",null,new{style="display:inline;"}) %><br />
                                    <%=Html.ActionLink("政策类型表维护", "ViewPolicy", null, new { style = "display:inline;" })%><br />
                                    <%=Html.ActionLink("投诉表维护", "ViewComplain", null, new { style = "display:inline;" })%><br />
                                    <%=Html.ActionLink("角色表维护", "ViewRole", null, new { style = "display:inline;" })%><br />
                                    <%=Html.ActionLink("申报审批类型维护", "ViewApprove", null, new { style = "display:inline;" })%>
                                </div>
                            </td>
                            <td style="width: 70%;  vertical-align: top; padding-left: 10px;" >
                                <table id="dataTable" class="mt_10" cellpadding="0" cellspacing="0">
                                    <thead>
                                        <tr class="micblack" style="background-color: #cbe7ee; font-weight: bold; color: #21768f;">
                                            <td style="width: 80px;">
                                                编号
                                            </td>
                                            <td style="width: 120px;">
                                               政策类型标识
                                            </td>
                                            <td style="width: 250px;">
                                                政策类型名称
                                            </td>
                                            <td style="width:80px;">
                                                操作
                                            </td>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <% int rowIndex = 0;
                                           var data = ViewData["dataSouce"] as List<policy>;
                                           if (data != null && data.Count > 0)
                                           {
                                               foreach (var tr in data)
                                               { %>
                                        <tr style="background-color: #F7F6F3; color: #333333; height: 20px;">
                                            <td>
                                                <%=++rowIndex %>
                                            </td>
                                             <td>
                                                <%=tr.p_id %>
                                            </td>
                                            <td>
                                                <%=tr.p_name %>
                                            </td>
                                            <td>
                                                <%=Html.ActionLink("删除","delPolicy",new{id=tr.id}) %>
                                            </td>
                                        </tr>
                                        <% }
                                           } %>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </center>
</asp:Content>
