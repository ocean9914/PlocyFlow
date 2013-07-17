<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%=Url.Content("~/Content/My97DatePicker/WdatePicker.js") %>" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts/jquery.bgiframe.min.js") %>" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts/thickbox-compressed.js") %>" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts/jquery.autocomplete.js") %>" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(function () {
            $("#productTxt").autocomplete('../General/getProduct', {
                dataType: 'json',
                parse: function (data) {
                    return $.map(data, function (row) {
                        return {
                            data: row,
                            value: row.Productid,
                            result: row.ProductName
                        }
                    });
                },
                formatItem: function (item) {
                    return item.ProductName;
                }
            }).result(function (e, item) {
                loadData(item);
            });
            $("#next_opr").autocomplete('../General/getStaff', {
                dataType: 'json',
                parse: function (data) {
                    return $.map(data, function (row) {
                        return {
                            data: row,
                            value: row.Name,
                            result: row.Name
                        }
                    });
                },
                formatItem: function (item) {
                    return item.Name;
                }
            }).result(function (e, item) {
                formatU(item);
            });
        });
        function formatU(item) {
            $("#next_opr").val(item.EnglishName);
        }
        function loadData(item) {
            $("#pId").val(item.Productid);
            $("#ptitle").text(item.ProductName);
            $("#pname").text(item.ProductName);
            $("#dname").text(item.DepartName);
            $("#ptname").text(item.ProductTypeName);
            $("#obdate").text(item.OBDate);
            $("#gname").text(item.Game_TypeName);
            $("#sname").text(item.StatusName);
            $("#manager").text(item.Manager);
            $("#policy").text(item.PolicyName);
            $("#isdone").text(item.IsDone);
            $("#remark").text(item.Remark);
            $("#next_opr").val(item.Manager);
            
        }
        function checkNull() {
            var r = false;
            var proid = $("#pId").val();
            if (proid != "") {
                if ($.trim($("#complain_memoTxt").val()) != "") {
//                    var attach = $("#attachFile").val();
//                    if (attach != "") {
//                        var i = attach.lastIndexOf(".");
//                        if (i != -1) {
//                            var ext = attach.substr(i + 1).toLowerCase();
//                            if (ext == "xls" || ext == "xlsx" || ext == "rar" || ext == "zip" || ext == "7z")
//                                r = true;
//                            else
//                                alert("文件格式（Excel文档|Rar文档|Zip文档）不符合要求.");
//                        }
//                    }
//                    else
                    //                        alert("请上传附件.");
                    r = true;
                }
                else
                    alert("请填写投诉内容.");
            }
            else
                alert("产品不存在,请确认是否有产品明细.");
            if (r) disbut();
            return r;
        }
        function disajax() {
            $("#subBut").hide();
            $("#ajaxImg").show();
        }
        function disbut() {
            setTimeout(disajax, 10);
        }
    </script>
    <center>
        <div class="percent_100 t_l ml_3">
            <div class="percent_100 lf">
                <%using (Html.BeginForm("save", "Complain", FormMethod.Post, new { style = "display:inline-block;", enctype = "multipart/form-data", onsubmit = "return checkNull();" }))
                  { %>
                <table cellspacing="0" cellpadding="0" class="table_blue  mt_10 ml_5 micblack" style="width: 750px;">
                    <tbody>
                        <tr>
                            <td style="width: 35%;" class="lf">
                                产品:
                                <%=Html.TextBox("productTxt", "", new { @class = "micblack ml_5 bd" })%>
                                <input type="hidden" id="pId" name="pId" value="" />
                            </td>
                            <td style="width: 30%;" class="lf">
                                投诉:<%=Html.DropDownList("complainList", ViewData["cList"] as SelectList, new { @class = "micblack ml_5 bd" })%>
                            </td>
                            <td style="width: 35%;" class="lf">
                                下一步处理人:<%=Html.TextBox("next_opr", "", new { @class = "micblack ml_5 bd" })%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="lf">
                                附件:
                                <input type="file" id="attachFile" name="attachFile" accept="application/msexcel"
                                    class="micblack ml_5" style="width: 80%;" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <table cellpadding="0" cellspacing="0" border="0" class="micblack percent_100">
                                    <tr>
                                        <td colspan="4" class="lf" style="border-style: none; border-width: 0px;">
                                            <span id="ptitle" class="title">
                                                <%=ViewData["pName"]%></span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lf" style="width: 12%; border-style: none; border-width: 0px;">
                                            产品名称
                                        </td>
                                        <td class="lf" style="width: 38%; border-style: none; border-width: 0px;">
                                        <span id="pname">&nbsp</span>
                                        </td>
                                        <td class="lf" style="width: 10%; border-style: none; border-width: 0px;">
                                            所属部门
                                        </td>
                                        <td class="lf" style="width: 40%; border-style: none; border-width: 0px;">
                                            <span id="dname">&nbsp</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            游戏来源
                                        </td>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            <span id="ptname">&nbsp</span>
                                        </td>
                                        <td class="lf " style="border-style: none; border-width: 0px;">
                                            OB时间
                                        </td>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            <span id="obdate">&nbsp</span> 
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lf " style="border-style: none; border-width: 0px;">
                                            游戏类型
                                        </td>
                                        <td class="lf " style="border-style: none; border-width: 0px;">
                                            <span id="gname">&nbsp</span>
                                        </td>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            状态
                                        </td>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            <span id="sname">&nbsp</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            产品负责人
                                        </td>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            <span id="manager">&nbsp</span>
                                        </td>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            政策类型
                                        </td>
                                        <td class="lf " style="border-style: none; border-width: 0px;">
                                            <span id="policy">&nbsp</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            是否完成
                                        </td>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            <span id="isdone">&nbsp</span>
                                        </td>
                                        <td class="lf " style="border-style: none; border-width: 0px;">
                                            备注
                                        </td>
                                        <td class="lf" style="border-style: none; border-width: 0px;">
                                            <span id="remark">&nbsp</span>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="lf">
                                <span style="vertical-align: top">备注:</span>
                                <textarea id="complain_memoTxt" name="complain_memoTxt" class="micblack ml_5 bd"
                                    style="width: 80%; height: 150px;"></textarea>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="lf">
                                截止日期:
                                <input type="text" id="endDateTxt" name="endDateTxt" class=" bd txtd ml_5 micblack"
                                    readonly="readonly" onclick="WdatePicker({})" style="width: 80px;" />
                                <%=Html.DropDownList("timeList", ViewData["tList"] as SelectList, new { @class = "micblack ml_5 bd" })%>
                                <span class="micblack ml_3">点</span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="lf">
                                <span id="lblMsg" class="micblack ml_10">
                                    <%=ViewData["msg"] %></span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="ct">
                                <input type="submit" class="btn" name="subBut" id="subBut" value="提交"  />
                                <img id="ajaxImg" src="<%=Url.Content("~/Content/images/load.gif")%>" style="display: none;
                                    margin-left: 10px;" />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <% } %>
            </div>
        </div>
    </center>
</asp:Content>
