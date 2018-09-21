<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Error.Master" Inherits="System.Web.Mvc.ViewPage<OrderDetail>" %>

<%@ Import Namespace="PlocyFlow.Models" %>
<%@ Import Namespace="PlocyFlow.DAL.CommonUtility" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cContent" runat="server">
    <script src="<%=Url.Content("~/Scripts/highcharts.js") %>" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
   
    </script>
    <span class="title">
        <%=ViewData["title"]%></span>
    <table cellpadding="0" cellspacing="0" class=" table_blue mt_10" style="width: 780px;">
        <tr>
            <td class="lf" style="width: 15%;">
                产品名称
            </td>
            <td class="lf" style="width: 35%;">
                <%=Model==null ? "&nbsp" : Model.ProductName %>
            </td>
            <td class="lf" style="width: 15%;">
                操作类型
            </td>
            <td class="lf" style="width: 35%;">
                <%=Model==null ? "&nbsp" : Model.WorkedType %>
            </td>
        </tr>
        <tr>
            <td class="lf">
                部门
            </td>
            <td class="lf">
                <%=Model==null ? "&nbsp" : Model.DeptName %>
            </td>
            <td class="lf ">
                发起日期
            </td>
            <td class="lf">
                <%=Model==null ? "&nbsp" : Model.OperDate %>
            </td>
        </tr>
        <tr>
            <td class="lf ">
                状态
            </td>
            <td class="lf ">
                <% 
                    string stauteTitle = "", stauteValue = "&nbsp";
                    if (Model != null)
                    {
                        if (Model.Staute != null && Model.Staute != "已关闭")
                        {
                            if (Model.Staute.Length > 30)
                            {
                                stauteValue = "等待" + Model.Staute.Substring(0, 30) + "...处理";
                                stauteTitle = "等待" + Model.Staute + "处理";
                            }
                            else
                                stauteValue = "等待" + Model.Staute + "处理";
                        }
                        else
                            stauteValue = Model.Staute;
                    }
                %>
                <span title="<%=stauteTitle %>">
                    <%=stauteValue %></span>
            </td>
            <td class="lf " colspan="2">
                <% string attchs = Model.AttachUrl == "" ? "#" : Url.Content("~/Upload/" + Model.AttachUrl);
                   string titles = Model.AttachUrl == "" ? "附件为空" : CommonUtility.GetFileName(Model.AttachUrl);
                %>
                <a class="comlink ml_10" title="<%=titles %>" href="<%=attchs  %>">附件资料</a>
                <% if (Model != null && Model.IsAdmin && Model.Staute != "已关闭")
                   { %>
                <input type="button" class="btn ml_5" id="closeBut" name="closeBut" value="关闭" onclick="javascript:$('#upBut').click();" />
                <input type="button" class="btn ml_5" id="authoryBut" name="authoryBut" value="转交" onclick="javascript:$('#mNextOprTxt').val($('#pmTxt').val());$('#mBut').click();" />
                请输入:<input type="text"  class="micblack ml_3 impinput" style="width:70px;" id="pmTxt" name="pmTxt" />
                <% } %>
                &nbsp
            </td>
        </tr>
    </table>
    <div class="mt_10 pd_10 lf" style="width: 760px; border-style: solid; border-color: #acc8d0;
        border-width: 1px;">
        <span class="title">处理明细</span>
        <%if (Model != null && Model.DetailData != null && Model.DetailData.Count > 0)
          {
              foreach (var d in Model.DetailData)
              {%>
        <table class="lf mt_6" style="border-style: ridge; border-width: 1.3px; border-color: Gray;
            background-color: #f7f6f3; width: 760px;">
            <tr>
                <td style="width: 8%; padding-left: 5px;">
                    处理日期:
                </td>
                <td style="width: 22%;">
                    <%=d.OperDate.ToString("yyyy-MM-dd hh:mm:ss")%>
                </td>
                <td style="width: 22%;">
                    处理人: &nbsp
                    <%=d.UserName %>
                </td>
                <td style="width: 22%;">
                    耗时: &nbsp
                    <%=d.Elapsed >= 0.1 ? d.Elapsed.ToString("0.0") : "0"%>(小时)
                </td>
                <td style="width: 22%;">
                    操作类型: &nbsp
                    <%=d.OperType.Oper_Key%>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top;">
                    <span style="display: block; margin-left: 5px;">处理意见:</span><br />
                    <% string attch = d.AttachUrl == "" ? "#" : Url.Content("~/Upload/" + d.AttachUrl);
                       string title = d.AttachUrl == "" ? "附件为空" : CommonUtility.GetFileName(d.AttachUrl);
                    %>
                    <a style="display: inline; margin-top: 20px; margin-left: 5px;" title="<%=title %>"
                        href="<%=attch %>">附件资料</a>
                </td>
                <td colspan="4">
                    <textarea name="oper_memoTxt" class="oper_memo micblack " readonly="readonly"><%=d.OperMemo%></textarea>
                </td>
            </tr>
        </table>
        <%}%>
        <% } %>
    </div>
    <div id="container" class="mt_10 " style="max-width: 800px; height: 400px; margin: 0 auto;
        overflow: auto;">
    </div>
    <script language="javascript" type="text/javascript">
        $(function () {
            var chart;

            $(document).ready(function () {
                var xs = '<%=ViewData["ulist"] %>';
                var es = '<%=ViewData["elist"] %>';
                if (xs) {
                    xs = xs.split(",");
                }
                if (es) {
                    var ss = es.split(",");
                    es = new Array();
                    for (var i = 0; i < ss.length; i++) {
                        es[i] = parseFloat(ss[i]);
                    }
                }
                chart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'container',
                        type: 'column'
                    },
                    title: {
                        text: '<%=ViewData["title"]%>'
                    },
                    xAxis: {
                        categories: xs
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: '耗时(小时)'
                        }
                    },
                    legend: {
                        layout: 'vertical',
                        backgroundColor: '#FFFFFF',
                        align: 'left',
                        verticalAlign: 'top',
                        x: 100,
                        y: 70,
                        floating: true,
                        shadow: true
                    },
                    tooltip: {
                        formatter: function () {
                            return '' +
                        this.x + ': ' + this.y + ' 小时';
                        }
                    },
                    plotOptions: {
                        column: {
                            pointPadding: 0.2,
                            borderWidth: 0
                        }
                    },
                    series: [{
                        name: '人员',
                        data: es

                    }]
                });
            });

        });
    </script>
    <%using (Html.BeginForm("CloseOrder", "Order", FormMethod.Post, new { style = "display:none;" }))
      {  %>
    <%=Html.TextBox("orderIdTxt", ViewData["orderId"])%>
    <input type="submit" id="upBut" />
    <% } %>
    <%using (Html.BeginForm("moveOrder", "Order", FormMethod.Post, new { style = "display:none;" }))
      {  %>
    <%=Html.TextBox("mOrderIdTxt", ViewData["orderId"])%>
    <%=Html.Hidden("mNextOprTxt", ViewData["orderId"])%>
    <input type="submit" id="mBut" />
    <% } %>
</asp:Content>
