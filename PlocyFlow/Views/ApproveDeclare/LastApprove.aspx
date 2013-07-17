<%@ Import Namespace="PlocyFlow.Models" %>
<%@ Import Namespace="PlocyFlow.DAL.CommonUtility" %>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Approve>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="<%=Url.Content("~/Scripts/jquery-1.7.2.min.js") %>" type="text/javascript"></script>
    <link href="<%=Url.Content("~/Content/styles/default.css") %>" rel="stylesheet" type="text/css" />
    <style type="text/css">
        body
        {
            background-color: #EFEFEF;
        }
        .memo
        {
            width: 88%;
            height: 100px;
            overflow: auto;
            display: inline;
        }
        .percent_98
        {
            width: 98%;
        }
    </style>
    <script type="text/javascript" language="javascript">
        //        function closeWin() {
        //            var curWin = document.parentWindow;
        //            var pareWin;
        //            if (curWin)
        //                pareWin = curWin.parent;
        //            else
        //                pareWin = window.parent;
        //            if (pareWin) {
        //                pareWin.closeDetail();
        //            }
        //            else
        //                document.parentWindow.close();
        //        }
        function setType(m) {
            var memo = $.trim($('#memoTxt').val());
            if (memo.length > 0) {
                $('#oper_type').val(m);
            }
            else {
                alert("处理意见不能为空.");
                return false;
            }
        }
    </script>
</head>
<body>
    <div class="percent_98 t_l ml_10 micblack">
        <%using (Html.BeginForm("save", "ApproveDeclare", FormMethod.Post, new { style = "display:inline-block;width:98%;", autocomplete = "off" }))%>
        <%{  %>
        <div class="percent_100 rt mt_10">
            <input type="hidden" id="orderId" name="orderId" value="<%=ViewData["orderId"] %>" />
            <span style="vertical-align: top">处理意见:</span>
            <textarea id="memoTxt" name="memoTxt" class="memo micblack"></textarea>
            <input type="hidden" name="oper_type" id="oper_type" />
            <center>
                <input type="submit" class="btn mt_10" value="同意"  onclick="return setType(2);"/>
                <input type="submit" class="btn ml_10 mt_10" value="驳回"  onclick="return setType(0);"/>
               <%-- <input type="button" class="btn ml_10 mt_10" value="取消"  onclick="closeWin();"/>--%>
            </center>
             <% string attchs = Model.DeclareOrder.attach_url == "" ? "#" : Url.Content("~/Upload/" + Model.DeclareOrder.attach_url);
               string titles = Model.DeclareOrder.attach_url == "" ? "附件为空" : CommonUtility.GetFileName(Model.DeclareOrder.attach_url);
             %>
            <a class="comlink"  title="<%=titles %>"  href="<%=attchs%>">资料包下载</a>
        </div>
        <% }%>
        <div class="percent_100 lf mt_10">
       <%-- <span class="micblack">下一步处理人:</span>
        <span class="micblack ml_5"><%=Model.NextOper_User %> </span> &nbsp &nbsp &nbsp--%>
        <span class="micblack">上一步处理人:</span>
        <span class="micblack ml_5"><%=Model.PreOper_User  %> </span> &nbsp &nbsp &nbsp
        
          <table cellpadding="0" cellspacing="0" class="table_blue mt_10 mic9" style="width:98%;">
                    <thead>
                        <tr >
                            <th style="width: 15%;">
                                处理时间
                            </th>
                            <th style="width: 12%;">
                                处理人
                            </th>
                            <th style="width: 61%;">
                                处理意见(带有省略号可停留光标显示完整内容)
                            </th>
                            <th style="width: 12%;">
                                耗时(时)
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <% 
                           var data = Model.DetailData;
                           if (data != null && data.Count > 0)
                           {
                               foreach (var tr in data)
                               { %>
                        <tr style="background-color: #F7F6F3; color: #333333; height: 20px;">
                            <td>
                                <%=tr.OperDate.ToString("yyyy-MM-dd") %>
                            </td>
                            <td >
                                <%=tr.UserName %>
                            </td>
                            <% string memo = tr.OperMemo; %>
                            <td title="<%=memo %>">
                              <%=memo.Length>25 ? memo.Substring(0,25)+"...":memo%>
                            </td>
                            <td class="rt">
                               <%=tr.Elapsed>=0.1 ? tr.Elapsed.ToString("0.0"):"0" %>
                            </td>
                        </tr>
                        <% }
                    } %>
                    </tbody>
                </table>
        </div>
    </div>
</body>
</html>
