<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="PlocyFlow.DAL.Entity" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%=Url.Content("~/Content/My97DatePicker/WdatePicker.js") %>" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function setDrop() {
            var did = $("#deptList option:selected").val();
            $("#deptTxt").val(did);
            var proid = $("#productList option:selected").val();
            $("#productTxt").val(proid);
            $("#upBut").click();
        }
        function setProduct() {
            var proid = $("#productList option:selected").val();
            $("#productTxt").val(proid);
        }
        function setComp() {
            var comp = $("#compList option:selected").val();
            $("#compTxt").val(comp);
        }
        function disDetail(t) {
            var orderid = $(t).next(".orderId").val();
            var url = '<%=Url.Content("~/ApproveComplain/Index")%>?orderId=' + orderid;
            //alert(url);
            showModel(url, "", 600, 560);
        }
        function reload() {
            window.location.reload();
        }
        function disorder(t) {
            var orderid = $(t).parent().find(".orderId").val();
            var url = '<%=Url.Content("~/Order/Index")%>?orderId=' + orderid;
            OpenNewWin(url, 980, 800);
        }
    </script>
    <center>
        <div class="percent_100 t_l ml_3">
            <div class="percent_100 lf" style="width: 760px;">
                <%using (Html.BeginForm("seek", "MyCtasks", FormMethod.Post, new { style = "display:inline-block;", autocomplete = "off" }))
                  { %>
                <span class="micblack  ml_5">部门:</span>
                <%=Html.DropDownList("deptList", ViewData["dList"] as SelectList, new { @class = "bd micblack ml_5",onchange="setDrop();"  })%>
                <span class="micblack ml_5">产品:</span>
                <%=Html.DropDownList("productList", ViewData["proList"] as SelectList, new { @class = "bd micblack ml_5",style="width:90px;", onchange = "setProduct();" })%>
                <span class="micblack ml_5">投诉类型:</span>
                <%=Html.DropDownList("compList", ViewData["cList"] as SelectList, new { @class = "bd micblack ml_5",onchange = "setComp();"  })%>
                <span class="micblack">日期:</span>
                <input type="text" id="startDateTxt" name="startDateTxt" class=" bd txtd ml_5 micblack"
                    readonly="readonly" onclick="WdatePicker({})" style="width: 80px;" />
                <span class="micblack">至</span>
                <input type="text" id="endDateTxt" name="endDateTxt" class=" bd txtd micblack" readonly="readonly"
                    onclick="WdatePicker({})" style="width: 80px;" />
                <input type="submit" class="btn ml_5" value="查询" />
                <% } %>
            </div>
            <div class="percent_100 lf">
                <table cellpadding="0" cellspacing="0" class="table_blue mt_10" style="width: 760px;">
                    <thead>
                        <tr>
                            <th style="width: 40px;">
                                编号
                            </th>
                            <th style="width: 120px;">
                                产品名称
                            </th>
                            <th style="width: 80px;">
                                部门
                            </th>
                            <th style="width: 80px;">
                                投诉类型
                            </th>
                            <th style="width: 70px;">
                                投诉日期
                            </th>
                            <th style="width: 70px;">
                                投诉人
                            </th>
                            <th style="width: 160px;">
                                截止日期
                            </th>
                            <th style="width: 70px;">
                                最后处理人
                            </th>
                            <th style="width: 70px;">
                                &nbsp
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <% int rowIndex = 0;
                           var data = ViewData["dataSouce"] as List<v_complain_order>;
                           if (data != null && data.Count > 0)
                           {
                               foreach (var tr in data)
                               { %>
                         <tr class="trtd"  title="单击鼠标左键可查看详情" 
                           onmousemove="javascript:$(this).css('color','#FF6600');" 
                           onmouseout="javascript:$(this).css('color','#333333');">
                            <td class="ct" onclick="disorder(this);" >
                                <%=++rowIndex %>
                            </td>
                            <td onclick="disorder(this);" >
                                <%=tr.ProductName %>
                            </td  >
                            <td onclick="disorder(this);" >
                                <%=tr.Departmentname %>
                            </td >
                            <td onclick="disorder(this);" >
                                <%=tr.complainName%>
                            </td >
                            <td onclick="disorder(this);" >
                                <%=tr.oper_date.ToString("yyyy-MM-dd") %>
                            </td>
                            <td onclick="disorder(this);" >
                                <%=tr.oper_user %>
                            </td >
                            <td onclick="disorder(this);" >
                                <%=tr.endate.ToString("yyyy-MM-dd HH:mm") %>
                            </td>
                            <td onclick="disorder(this);" >
                                <%=tr.last_oper %>
                            </td>
                            <td>
                                <input type="button" class="sbtn" value="处理" onclick="disDetail(this);" />
                                <input type="hidden" class="orderId" value="<%=tr.order_Id %>" />
                            </td>
                        </tr>
                        <% }
                           } %>
                    </tbody>
                </table>
            </div>
         
        </div>
        <%using (Html.BeginForm("setView", "MyCtasks", FormMethod.Post, new { style = "display:none;" }))
          {  %>
        <%=Html.TextBox("deptTxt",ViewData["deptTxt"])%>
        <%=Html.TextBox("compTxt",ViewData["compTxt"])%>
        <%=Html.TextBox("productTxt", ViewData["productTxt"])%>
        <input type="submit" id="upBut" />
        <% } %>
    </center>
</asp:Content>
