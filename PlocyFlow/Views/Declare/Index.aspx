<%@ Import Namespace="PlocyFlow.DAL.Entity" %>
<%@ Import Namespace="PlocyFlow.DAL.CommonUtility" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/main.Master" Inherits="System.Web.Mvc.ViewPage<v_bipdata>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewData["title"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    function setDrop() {
        var did = $("#deptList option:selected").val();
        $("#deptTxt").val(did);
        var pid = $("#policyList option:selected").val();
        $("#policyTxt").val(pid);
        var sid = $("#stauteList option:selected").val();
        $("#stauteTxt").val(sid);
        $("#productTxt").val("");
        $("#upBut").click();
    }
    function setProduct() {
        var did = $("#deptList option:selected").val();
        $("#deptTxt").val(did);
        var pid = $("#policyList option:selected").val();
        $("#policyTxt").val(pid);
        var sid = $("#stauteList option:selected").val();
        $("#stauteTxt").val(sid);
        var proid = $("#productList option:selected").val();
        $("#productTxt").val(proid);
        $("#upBut").click();
    }
    function checkNull() {
        var r = false;
        var proid = $("#productList option:selected").val();
        if (proid != "") {
            var attach = $("#attachFile").val();
            if (attach != "") {
                var i = attach.lastIndexOf(".");
                if (i != -1) {
                    var ext = attach.substr(i + 1).toLowerCase();
                    if (ext == "xls" || ext == "xlsx" || ext == "doc" || ext == "docx" || ext == "rar" || ext == "zip" || ext == "7z")
                        r = true;
                    else
                        alert("文件格式（Excel文档|Word文档|Rar文档|Zip文档）不符合要求.");
                }
            }
            else
                alert("请上传附件.");
        }
        else
            alert("未选中产品.");
        if (r) disbut();
        return r;
    }
    function disajax()
    {
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
             <%using (Html.BeginForm("save", "Declare", FormMethod.Post, new { style = "display:inline-block;", autocomplete = "off", enctype = "multipart/form-data", onsubmit = "return checkNull();" }))
               { %>
               <table cellspacing="0" cellpadding="0" class="table_blue  mt_10 ml_5 micblack" style="width:750px;">
                    <tbody>
                        <tr>
                          <td style="width:25%;" class="lf">部门:<%=Html.DropDownList("deptList", ViewData["dList"] as SelectList, new { @class = "micblack ml_5",onchange="setDrop();"  })%></td>
                          <td style="width:25%;" class="lf">政策类型:<%=Html.DropDownList("policyList", ViewData["pList"] as SelectList, new { @class = "micblack ml_5", onchange = "setDrop();" })%></td>
                          <td style="width:25%;" class="lf">状态:<%=Html.DropDownList("stauteList", ViewData["sList"] as SelectList, new { @class = "micblack ml_5", onchange = "setDrop();" })%></td>
                          <td style="width:25%;" class="lf">产品:<%=Html.DropDownList("productList", ViewData["proList"] as SelectList, new { @class = "micblack ml_5", onchange = "setProduct();" })%></td>
                        </tr>
                        <tr>
                         <td colspan="4" class="lf">
                          附件: <input type="file" id="attachFile" name="attachFile" accept="application/msexcel" class="micblack ml_5" style="width:70%;" />
                          <a class="comlink" href="<%=ViewData["modeFile"]%>">模板文档下载</a>
                         </td>
                        </tr>
                        <tr>
                         <td colspan="4" class="lf">
                          备注:<%=Html.TextBox("memoTxt","",new {@class="micblack ml_5",style="width:70%;"})%>
                          <%=Html.ActionLink("BIP刷新", "refresh", "Declare", null, new { @class = "sbtn" })%>
                         </td>
                        </tr>
                        <tr>
                         <td colspan="4">
                          <table cellpadding="0" cellspacing="0" border="0" class="micblack percent_100">
                           <tr>
                            <td colspan="4" class="lf" style="border-style:none; border-width:0px;">
                             <span class="title"><%=ViewData["productName"]%></span>
                            </td>
                           </tr>
                           <tr>
                            <td  class="lf" style="width:12%;border-style:none; border-width:0px;">产品名称</td>
                            <td  class="lf" style="width:38%;border-style:none; border-width:0px;"><%=Model==null ? "&nbsp" : Model.ProductName %></td>
                            <td  class="lf" style="width:10%;border-style:none; border-width:0px;">所属部门</td>
                            <td  class="lf" style="width:40%;border-style:none; border-width:0px;"><%=Model==null ? "&nbsp" : Model.DeptName %></td>
                           </tr>
                           <tr>
                            <td class="lf" style="border-style:none; border-width:0px;">游戏来源</td>
                            <td class="lf" style="border-style:none; border-width:0px;"><%=Model==null ? "&nbsp" : Model.ProductTypeName %></td>
                            <td class="lf " style="border-style:none; border-width:0px;">OB时间</td>
                            <td class="lf" style="border-style:none; border-width:0px;"><%=Model!=null && Model.OBDate!=null ?  Model.OBDate.Value.ToString("yyyy-MM-dd") :"&nbsp" %></td>
                           </tr>
                           <tr>
                             <td class="lf " style="border-style:none; border-width:0px;">游戏类型</td>
                             <td class="lf " style="border-style:none; border-width:0px;"><%=Model==null ? "&nbsp" : Model.Game_Name %></td>
                             <td class="lf" style="border-style:none; border-width:0px;">状态</td>
                             <td class="lf" style="border-style:none; border-width:0px;"><%=Model==null ? "&nbsp" : Model.StatusName %></td>
                           </tr>
                           <tr>
                             <td class="lf" style="border-style:none; border-width:0px;">产品负责人</td>
                             <td class="lf" style="border-style:none; border-width:0px;"><%=Model==null ? "&nbsp" : Model.Manager %></td>
                             <td class="lf" style="border-style:none; border-width:0px;">政策类型</td>
                             <td class="lf " style="border-style:none; border-width:0px;"><%=Model==null ? "&nbsp" : Model.PolicyName %></td>
                           </tr>
                           <tr>
                             <td class="lf" style="border-style:none; border-width:0px;">是否完成</td>
                             <td class="lf" style="border-style:none; border-width:0px;"><%=Model == null ? "&nbsp" : CommonUtility.HasBoolValue(Model.IsDone, "未完成", "已完成", "未知")%></td>
                             <td class="lf " style="border-style:none; border-width:0px;">备注</td>
                             <td class="lf" style="border-style:none; border-width:0px;"><%=Model==null ? "&nbsp" : Model.Remark %></td>
                           </tr>
                          </table>
                         </td>
                        </tr>
                        <tr>
                         <td colspan="4" class="lf">
                          <span id="lblMsg" class="micblack ml_10"><%=ViewData["msg"] %></span>
                         </td>
                        </tr>
                        <tr>
                         <td colspan="4" class="ct">
                          <input type="submit" class="btn" name="subBut" id="subBut" value="提交"   />
                           <img id="ajaxImg" src="<%=Url.Content("~/Content/images/load.gif")%>"  style="display:none; margin-left:10px;"/>
                         </td>
                        </tr>
                    </tbody>
                </table>

                <% } %>
            </div>
        </div>
         <%using (Html.BeginForm("setView", "Declare", FormMethod.Post, new { style = "display:none;" }))
          {  %>
            <%=Html.TextBox("deptTxt",ViewData["deptTxt"])%>
            <%=Html.TextBox("policyTxt", ViewData["policyTxt"])%>
            <%=Html.TextBox("stauteTxt", ViewData["stauteTxt"])%>
            <%=Html.TextBox("productTxt", ViewData["productTxt"])%>
            <input type="submit" id="upBut" />
        <% } %>
    </center>
</asp:Content>
