﻿<%@ Import Namespace="PlocyFlow.Models" %>
<%@ Import Namespace="PlocyFlow.DAL.CommonUtility" %>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ApproveComp>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
            width: 98%;
            height: 100px;
            overflow: auto;
            display: inline;
        }
        .oper_memo
        {
            width: 98%;
            height: 60px;
            overflow: auto;
            display: inline;
        }
        .percent_98
        {
            width: 98%;
        }
    </style>
    <script type="text/javascript" language="javascript">

        function setType(m) {
            var memo = $.trim($('#memoTxt').val());
            var pm = $.trim($('#pmTxt').val());
            $('#oper_type').val(m);
            if (memo.length > 0) {
                if (m != 1) {
                    if (pm.length > 0) {
                        //                    var attach = $("#attachFile").val();
                        //                    if (attach != "") {
                        //                        var i = attach.lastIndexOf(".");
                        //                        if (i != -1) {
                        //                            var ext = attach.substr(i + 1).toLowerCase();
                        //                            if (ext == "xls" || ext == "xlsx" || ext == "rar" || ext == "zip" || ext == "7z")
                        //                                $('#oper_type').val(m);
                        //                            else {
                        //                                alert("文件格式（Excel文档|Rar文档|Zip文档）不符合要求.");
                        //                                return false;
                        //                            }
                        //                        }
                        //                    }
                        //                    else {
                        //                        alert("请上传附件.");
                        //                        return false;
                        //                    }
                        
                    }
                    else {
                        alert("RTX英文名不能为空.");
                        return false;
                    }
                }
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
    <p>
      <span class="ml_10"><%=User.Identity.Name %></span>您好!<br />
      <span class="ml_10">&nbsp &nbsp &nbsp &nbsp <%=Model.ComplainOrder.oper_user %></span>向您发起了关于<<<span><%=Model.ComplainOrder.productName %></span>>>的<span><%=Model.ComplainOrder.complainName %></span>投诉事件,该投诉内容如下：
      
    </p>
    <div class=" percent_100 t_l  micblack" style= "max-height:50px; overflow:auto;">
       &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp<%=Model.ComplainOrder.memo%>
    </div>
    <div class=" percent_100 t_r  micblack">
    <% string attchs = Model.ComplainOrder.attach_url == "" ? "#" : Url.Content("~/Upload/" + Model.ComplainOrder.attach_url);
                   string titles = Model.ComplainOrder.attach_url == "" ? "附件为空" : CommonUtility.GetFileName(Model.ComplainOrder.attach_url);
                 %>
              <a class="comlink ml_10" title="<%=titles %>"  href="<%=attchs  %>">查看附件(<%=titles %>)</a>
    </div>
        <%using (Html.BeginForm("save", "ApproveComplain", FormMethod.Post, new { style = "display:inline-block;width:98%;", enctype = "multipart/form-data", autocomplete = "off" }))%>
        <%{  %>
            <input type="hidden" id="orderId" name="orderId" value="<%=ViewData["orderId"] %>" />
            <table class="percent_100 lf mt_6">
             <tr>
              <td style="width:15%; vertical-align:top;"><span style="vertical-align: top">您的处理意见:</span></td>
              <td colspan="2"><textarea id="memoTxt" name="memoTxt" class="memo micblack mt_6"></textarea></td>
             </tr>
             <tr>
             <td>上传附件:</td>
              <td colspan="2">
               <input type="file" id="attachFile" name="attachFile" accept="application/msexcel" class="micblack" style="width: 80%; background-color:White;" />
              </td>
             </tr>
             <tr>
             <td style=" vertical-align:top;"><span style="vertical-align: top">操作说明:</span></td>
              <td colspan="2">
               您可以选择转交该事务给其它同事处理，或者直接提交您的处理意见给<%=Model.ComplainOrder.oper_user%>，
              </td>
             </tr>
             <tr>
              <td class="ct" colspan="3">
                <input type="hidden" name="oper_type" id="oper_type" />
                <input type="submit" class="btn mt_10" value="提交"  onclick="return setType(1);"/>
                <input type="submit" class="btn ml_10 mt_10" value="转交"  onclick="return setType(0);"/>
                <span class="ml_10">请输入RTX英文名:<%=Html.TextBox("pmTxt","",new {@class="micblack ml_3",style="width:70px;"})%></span>
              </td>
             </tr>
            </table>
        <% }%>
        <div class="percent_100 lf mt_10">
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
