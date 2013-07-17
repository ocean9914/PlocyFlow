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
            var pid = $("#policyList option:selected").val();
            $("#policyTxt").val(pid);
            var proid = $("#productList option:selected").val();
            $("#productTxt").val(proid);
            $("#upBut").click();
        }
        function setProduct() {
            var proid = $("#productList option:selected").val();
            $("#productTxt").val(proid);
        }
        function disDetail(t) {
//            var child = $("#childWin");
//            child.attr("src", "about:blank");
//            var id = "#detailOrder";
//            var ld = $(id);
//            ld.css({
//                position: 'absolute',
//                left: ($(window).width() - $(id).outerWidth()) / 2,
//                top: ($(window).height() - $(id).outerHeight()) / 2 + $(document).scrollTop()
//            });

            //var curt = $(t.parentNode.parentNode);
            //var p = curt.position();
            //ld.offset({ top: p.top + curt.height() + document.documentElement.scrollTop, left: p.left + document.documentElement.scrollLeft + 150 });
            //ld.offset({ top: p.top+ curt.height(), left:p.left + 150 });

           // ld.show();
            // child.attr("src", "../ApproveDeclare/Index?orderId=" + $(t).next(".orderId").val());
            var orderid = $(t).next(".orderId").val();
            var url = '<%=Url.Content("~/ApproveDeclare/Index")%>?orderId=' + orderid;
            //alert(url);
            showModel(url,"", 600, 480);
        }
        function reload() {
            window.location.reload();
        }
        function closeDetail() {
            var ld = $("#detailOrder");
            ld.offset({ top: 0, left: 0 });
            ld.hide();
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
                <%using (Html.BeginForm("seek", "MyGtasks", FormMethod.Post, new { style = "display:inline-block;", autocomplete = "off" }))
                  { %>
                <span class="micblack  ml_5">部门:</span>
                <%=Html.DropDownList("deptList", ViewData["dList"] as SelectList, new { @class = "bd micblack ml_5",onchange="setDrop();"  })%>
                <span class="micblack ml_5">政策类型:</span>
                <%=Html.DropDownList("policyList", ViewData["pList"] as SelectList, new { @class = "bd micblack ml_5", onchange = "setDrop();" })%>
                <span class="micblack ml_5">产品:</span>
                <%=Html.DropDownList("productList", ViewData["proList"] as SelectList, new { @class = "bd micblack ml_5",style="width:90px;", onchange = "setProduct();" })%>
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
                                政策类型
                            </th>
                            <th style="width: 70px;">
                                申报日期
                            </th>
                            <th style="width: 70px;">
                                申报人
                            </th>
                            <th style="width: 160px;">
                                最后处理意见
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
                           var data = ViewData["dataSouce"] as List<v_declare_order>;
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
                            </td>
                            <td onclick="disorder(this);" >
                                <%=tr.dept_name %>
                            </td>
                            <td onclick="disorder(this);" >
                                <%=tr.policyName %>
                            </td>
                            <td onclick="disorder(this);" >
                                <%=tr.oper_date.ToString("yyyy-MM-dd") %>
                            </td>
                            <td onclick="disorder(this);" >
                                <%=tr.oper_user %>
                            </td>
                            <td onclick="disorder(this);" >
                                <span title="<%=tr.last_memo %>">
                                    <%=tr.last_memo.Length>10 ? tr.last_memo.Substring(0,10):tr.last_memo%>
                                </span>
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
           <%-- <div id="detailOrder" style="display: none; position: absolute; width: 600px; border-style: ridge;
                border-color: Gray; border-width: 1.5px; background-color: White;">
                <iframe id="childWin" width="100%" height="480px" scrolling="auto" class="noborder">
                </iframe>
            </div>--%>
        </div>
        <%using (Html.BeginForm("setView", "MyGtasks", FormMethod.Post, new { style = "display:none;" }))
          {  %>
        <%=Html.TextBox("deptTxt",ViewData["deptTxt"])%>
        <%=Html.TextBox("policyTxt", ViewData["policyTxt"])%>
        <%=Html.TextBox("productTxt", ViewData["productTxt"])%>
        <input type="submit" id="upBut" />
        <% } %>
    </center>
</asp:Content>
