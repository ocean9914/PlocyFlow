<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="PlocyFlow.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <center>
        <div class="wel">
            <div class="percent_100 t_l  mt_10">
                <dl>
                    <dt class="title t_l ml_10 mt_10">我要申报</dt>
                    <dd>
                        <a class="abtn" href="<%=Url.Content("~/Declare/Index?id=1")%>">商标</a> <a class="abtn"
                            href="<%=Url.Content("~/Declare/Index?id=2")%>">著作权</a> <a class="abtn" href="<%=Url.Content("~/Declare/Index?id=3")%>">
                                版号</a> <a class="abtn" href="<%=Url.Content("~/Declare/Index?id=4")%>">文化部备案</a>
                        <a class="abtn" href="<%=Url.Content("~/Declare/Index?id=5")%>">版权批复</a>
                    </dd>
                </dl>
                <% if (PagePurview.verifyVisble(User.Identity.Name, "Complain/Index"))
                   {  %>
                <dl>
                    <dt class="title t_l ml_10">我要投诉</dt>
                    <dd>
                        <a class="abtn" href="<%=Url.Content("~/Complain/Index")%>">互娱投诉</a>
                    </dd>
                </dl>
                <% } %>
            </div>
        </div>
    </center>
</asp:Content>
