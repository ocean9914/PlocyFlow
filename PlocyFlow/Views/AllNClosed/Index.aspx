<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="PlocyFlow.DAL.CommonUtility" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%=Url.Content("~/Content/My97DatePicker/WdatePicker.js") %>" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts/jquery.tablesorter.min.js") %>" type="text/javascript"></script>
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
        function disorder(t) {
            var orderid = $(t).find(".orderId").val();
            var url = '<%=Url.Content("~/Order/Index")%>?orderId=' + orderid;
            OpenNewWin(url, 980, 800);
        }
        $(document).ready(function () {
            $(".table_blue").tablesorter();
        });
    </script>
    <center>
        <div class="percent_100 t_l ml_3">
            <div class="percent_100 lf" style="width: 760px;">
                <%using (Html.BeginForm("seek", "AllNClosed", FormMethod.Post, new { style = "display:inline-block;", autocomplete = "off" }))
                  { %>
                <span class="micblack  ml_5">部门:</span>
                <%=Html.DropDownList("deptList", ViewData["dList"] as SelectList, new { @class = "bd micblack ml_5",onchange="setDrop();"  })%>
                <span class="micblack ml_5">产品:</span>
                <%=Html.DropDownList("productList", ViewData["proList"] as SelectList, new { @class = "bd micblack ml_5",style="width:90px;", onchange = "setProduct();" })%>
                <span class="micblack">处理日期:</span>
                <input type="text" id="startDateTxt" name="startDateTxt" class=" bd txtd ml_5 micblack"
                    readonly="readonly" onclick="WdatePicker({})" style="width: 80px;" value="<%=ViewData["startTxt"]%>"/>
                <span class="micblack">至</span>
                <input type="text" id="endDateTxt" name="endDateTxt" class=" bd txtd micblack" readonly="readonly"
                    onclick="WdatePicker({})" style="width: 80px;"  value="<%=ViewData["endTxt"]%>"/>
                <%=Html.Hidden("opType")%>
                <input type="submit" class="btn ml_5" value="查询" />
                 <input type="submit" class="btn ml_5" value="导出" onclick="javascript:$('#opType').val(1);" />
                <% } %>
            </div>
            <div class="percent_100 lf">
                <table cellpadding="0" cellspacing="0" class="table_blue mt_10" style="width: 760px;">
                    <thead>
                        <tr title="点击标题栏对应列可排序">
                            <th style="width: 50px;">
                                编号
                            </th>
                            <th style="width: 150px;">
                                类型
                            </th>
                            <th style="width: 50px;">
                                状态
                            </th>
                            <th style="width: 280px;">
                                产品名称
                            </th>
                            <th style="width: 120px;">
                                部门
                            </th>
                            <th style="width: 100px;">
                                生成日期
                            </th>
                            <th style="display:none">
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <% int rowIndex = 0;
                           var data = ViewData["dataSouce"] as List<Worked>;
                           if (data != null && data.Count > 0)
                           {
                               foreach (var tr in data)
                               { %>
                         <tr class="trtd"  title="单击鼠标左键可查看详情"  onclick="disorder(this);" 
                           onmousemove="javascript:$(this).css('color','#FF6600');" 
                           onmouseout="javascript:$(this).css('color','#333333');">
                            <td class="ct">
                                <%=++rowIndex %>
                            </td>
                            <td>
                                <%=tr.WorkedType %>
                            </td>
                            <td>
                                <%=tr.Staute %>
                            </td>
                            <td>
                                <%=tr.ProductName%>
                            </td>
                            <td>
                                <%=tr.DeptName%>
                            </td>
                            <td>
                                <%=tr.OperDate%>
                            </td>
                            
                            <td style="display:none;">
                                <input type="hidden" class="orderId" value="<%=tr.OrderId %>" />
                            </td>
                        </tr>
                        <% }
                           } %>
                    </tbody>
                </table>
            </div>
         
        </div>
        <%using (Html.BeginForm("setView", "AllNClosed", FormMethod.Post, new { style = "display:none;" }))
          {  %>
        <%=Html.TextBox("deptTxt",ViewData["deptTxt"])%>
        <%=Html.TextBox("productTxt", ViewData["productTxt"])%>
        <input type="submit" id="upBut" />
        <% } %>
    </center>
</asp:Content>
