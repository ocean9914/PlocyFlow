<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Error.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Filenotfound
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cContent" runat="server">
  <div style="text-align: center;">
        <div style="margin: 50px auto; width: 500px; text-align: left;">
            <img style="float: left; margin-right: 20px;" src="<%=Url.Content("~/Content/images/icon-notfound.gif")%>"
                alt="" />
            <div style="padding: 10px 0 100px;">
                <span style="font-weight: bold; color: #f00; font-size: 14px;">对不起，您当前访问的页面未找到，请联系管理员！</span><br />
                <p>
                    <a href="<%=Url.Content("~/Home/Index")%>">点击这里返回首页</a></p>
            </div>
        </div>
        <div style="clear: both;">
        </div>
    </div>
</asp:Content>
