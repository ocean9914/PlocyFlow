<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../Scripts/jquery.autocomplete.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">

        function AutoGetUser() {
            var data = ["河北省", "河南省", "山东", "北京", "天津"];
            $("#testTxt").autocomplete(data);
        }
	</script>
    <h2>Index</h2>
    <span id="msgLab"><%=ViewData["type"]%></span>
   <%=Html.DropDownList("roleList")%>
   <input type="text" id="testTxt"  onkeyup="AutoGetUser();"/>
</asp:Content>
