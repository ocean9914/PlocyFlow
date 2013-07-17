using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlocyFlow.Models;
using System.IO;
using System.Text;
using PlocyFlow.DAL.App;
using PlocyFlow.DAL.Context;
using PlocyFlow.DAL.CommonUtility;
using PlocyFlow.DAL.Entity;
using Tencent.OA.Framework.Messages;
using Tencent.OA.Framework.Messages.DataContract;
namespace PlocyFlow.Controllers
{
    public class DeclareController : BaseController
    {
        //
        // GET: /Declare/
        private string deptId;
        private string productId;
        private string policyId;
        private string stauteId;
        private v_bipdata curProduct;
        public ActionResult Index(int? id)
        {
            int policy = 1;
            if (id.HasValue)
                policy = id.Value;
            switch (policy)
            {
                case 1:
                    policyId = "CR_Brand";
                    break;
                case 2:
                    policyId = "CR_Copyright";
                    break;
                case 3:
                    policyId = "CR_EditionNo";
                    break;
                case 4:
                    policyId = "CR_Record";
                    break;
                case 5:
                    policyId = "CR_Edition";
                    break;
                default:
                    policyId = "CR_Brand";
                    break;
            }
            setInitialData();
            ViewData["msg"] = "就绪";
            return View("Index", curProduct);
        }
        public ActionResult setView(string deptTxt, string policyTxt, string stauteTxt, string productTxt)
        {
            deptId = deptTxt;
            policyId = policyTxt;
            stauteId = stauteTxt;
            productId = productTxt;
            curProduct = null;
            setInitialData();
            ViewData["msg"] = "就绪";
            return View("Index", curProduct);
        }
        public ActionResult refresh()
        {
            //string filename = Server.MapPath(Url.Content("~/download/bip.xml"));
            //string content = "";
            //using (FileStream fs = new FileStream(filename, FileMode.Open))
            //{
            //    StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            //    content = sr.ReadToEnd();
            //}
            var bip = new GetBipData();
            //bip.getData(content);
            bip.getData();
            return Index(1);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult save()
        {
            deptId = Request["deptList"];
            policyId = Request["policyList"];
            stauteId = Request["stauteList"];
            productId = Request["productList"];
            string memo = Request["memoTxt"];
            var file = Request.Files;
            if (file.Count > 0)
            {
                var temp = file[0];
                if (temp.ContentLength <= 50 * 1024 * 1024)
                {
                    try
                    {
                        string filepath = Server.MapPath(Url.Content("~/Upload/" + policyId));
                        if (!Directory.Exists(filepath))
                            Directory.CreateDirectory(filepath);
                        string FileName = CommonUtility.GetFileName(temp.FileName);
                        temp.SaveAs(filepath + "\\" + FileName);
                        string fileUrl = policyId + "/" + FileName;
                        string next_oper = apr_user_listDataContext.GetFirstApproves(policyId);
                        if (next_oper != "")
                        {
                            declareorder nd = new declareorder();
                            nd.order_Id = BaseId.getDeclareId();
                            nd.policyId = policyId;
                            nd.product_Id = productId;
                            nd.oper_user = HttpContext.User.Identity.Name;
                            nd.oper_date = DateTime.Now;
                            nd.staute = false;
                            nd.attach_url = fileUrl;
                            nd.memo = memo;
                            nd.next_oper = next_oper;
                            nd.last_oper = nd.oper_user;
                            nd.last_oper_date = DateTime.Now;
                            nd.memo1 = OperList.NewSave.Oper_Key;
                            if (declareOrderDataContext.AddDeclareOrder(nd))
                            {
                                ViewData["msg"] = "申报成功,已提交至:" + next_oper + "等待审批.";
                                string sp = ",";
                                string policyName = policyDataContext.GetPolicyName(policyId);
                                string productName = bip_dataDataContext.GetProductName(productId);
                                string[] ss = next_oper.Split(sp.ToCharArray());
                                foreach (string receiver in ss)
                                {
                                    if (!string.IsNullOrEmpty(receiver))
                                    {
                                        string message = string.Format("[政策电子流－申报]{0}向您提交了关于<<{1}>>的{2}申报事件,备注内容：{3}", nd.oper_user, productName, policyName, memo);
                                        string title = string.Format("[政策电子流-申报{0}]", policyName);
                                        try
                                        {
                                            sendRtxMessage(message, receiver, title, nd.oper_user,nd.order_Id);
                                        }
                                        catch 
                                        {
                                            
                                        }
                                    }
                                }
                            }
                            else
                                ViewData["msg"] = "提交失败";
                        }
                        else
                            ViewData["msg"] = "提交失败：下一步处理人列表为空,请联系系统管理员.";
                    }
                    catch
                    {
                        ViewData["msg"] = "系统发生错误,提交失败.";
                    }
                }
                else
                    ViewData["msg"] = "附件太大(最大50MB)，提交失败.";
            }

            curProduct = null;
            setInitialData();

            return View("Index", curProduct);
        }
        private void sendRtxMessage(string message, string receiver, string title, string sender,string orderId)
        {
            string url = "http://" + Request.Url.Host + Url.Content("~/MyGtasks/Index") + "?orderId=" + orderId;
            RTXMessage rtxMessage = new RTXMessage()
            {
                MsgInfo = message+string.Format("点击处理链接:{0}",url),
                Receiver = receiver,
                Title = title,
                Priority = MessagePriority.Normal,
                Sender = sender
            };
            MessageHelper.SendRTXMessage(rtxMessage);
        }
        [NonAction]
        public void setInitialData()
        {

            ViewData["dList"] = GeneralDropData.GetDept(ref deptId);
            ViewData["pList"] = GeneralDropData.GetPolicy(ref policyId);
            ViewData["sList"] = GeneralDropData.GetStauteOrAll(ref stauteId);
            setCurrentProduct();
            ViewData["deptTxt"] = deptId;
            ViewData["policyTxt"] = policyId;
            ViewData["stauteTxt"] = stauteId;
            ViewData["productTxt"] = productId;
            setNavTitle();
        }
        [NonAction]
        private SelectList getEmptySli(string name, string value)
        {
            SelectListItem sli = new SelectListItem();
            sli.Text = name;
            sli.Value = value;
            sli.Selected = true;
            var l = new List<SelectListItem>();
            l.Add(sli);
            return new SelectList(l, "Value", "Text");
        }
        [NonAction]
        private void setNavTitle()
        {
            switch (policyId)
            {
                case "CR_Brand":
                    Initial("申报商标", "我要申请 > 商标");
                    break;
                case "CR_Copyright":
                    Initial("申报著作权", "我要申请 > 著作权");
                    break;
                case "CR_EditionNo":
                    Initial("申报版号", "我要申请 > 版号");
                    break;
                case "CR_Record":
                    Initial("申报文化部备案", "我要申请 > 文化部备案");
                    break;
                case "CR_Edition":
                    Initial("申报版权批复", "我要申请 > 版权批复");
                    break;
                default:
                    Initial("申报商标", "我要申请 > 商标");
                    break;
            }
        }
        [NonAction]
        private void setCurrentProduct()
        {
            int staute = CommonUtility.TryParseStringToInt32(stauteId);
            IEnumerable<v_bipdata> b = null;
            if(staute==0)
                b = bip_view.GetBipData(policyId, CommonUtility.TryParseStringToInt32(deptId));
            else
                b = bip_view.GetBipData(policyId, CommonUtility.TryParseStringToInt32(deptId), CommonUtility.TryParseStringToInt32(stauteId));
            if (b != null && b.Count() > 0)
            {
                if (productId != null && productId.Trim() != "")
                    curProduct = bip_view.GetBipSingle(productId, policyId);
                else
                {
                    curProduct = b.First();
                    productId = curProduct.Productid;
                }
                ViewData["proList"] = new SelectList(b, "Productid", "ProductName", productId);
            }
            else
                ViewData["proList"] = getEmptySli("无", "");
            ViewData["modeFile"] = Url.Content("~/Download/modeFile/" + policyId + ".7z");
            if (curProduct == null)
                ViewData["productName"] = "No Product";
            else
                ViewData["productName"] = curProduct.ProductName + " 详细信息";
        }

    }
}
