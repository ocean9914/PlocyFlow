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
    public class ComplainController : BaseController
    {

        private string complianId;
        private readonly string attachPath = "Complain";
        public ActionResult Index()
        {
            string uid = User.Identity.Name;
            if (PagePurview.verifyVisble(uid, "Complain/Index"))
            {
                setInitialData();
                return View("Index");
            }
            else
                return Redirect("~/Error/Accessdeny");
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult save()
        {
            string uid = User.Identity.Name;
            if (!PagePurview.verifyVisble(uid, "Complain/Index"))
            {
                return Redirect("~/Error/Accessdeny");
            }
            else
            {
                string productName = Request["productTxt"];
                string productId = Request["pId"];
                if (productId == null || productName == null)
                {
                    ViewData["msg"] = "产品不存在,请确认产品明细栏是否有显示产品详情.";
                    return Index();

                }
                string complainId = Request["complainList"];
                int cid = 0;
                if (!int.TryParse(complainId, out cid))
                {
                    ViewData["msg"] = "投诉类型不正确.";
                    return Index();
                }
                string next_oper = Request["next_opr"];
                string end = Request["endDateTxt"];
                string memo = Request["complain_memoTxt"];
                string time = Request["timeList"];
                DateTime enddate;
                if (!(end != null && end.Trim() != "") || !DateTime.TryParse(end + " " + time + ":00:00", out enddate))
                {
                    ViewData["msg"] = "截止日期不正确.";
                    return Index();
                }
                var file = Request.Files;
                string fileUrl = "";
                if (file.Count > 0)
                {
                    var temp = file[0];
                    if (temp.ContentLength <= 10 * 1024 * 1024)
                    {
                        if (temp.ContentLength > 0)
                        {
                            try
                            {
                                string filepath = Server.MapPath(Url.Content("~/Upload/" + attachPath));
                                if (!Directory.Exists(filepath))
                                    Directory.CreateDirectory(filepath);
                                string FileName = CommonUtility.GetFileName(temp.FileName);
                                temp.SaveAs(filepath + "\\" + FileName);
                                fileUrl = attachPath + "/" + FileName;
                            }
                            catch
                            {
                                ViewData["msg"] = "附件保存错误,提交失败.";
                            }
                        }
                    }
                    else
                        ViewData["msg"] = "附件太大(最大10MB)，提交失败.";
                }
                try
                {
                    if (next_oper != null && next_oper.Trim() != "")
                    {
                        complainorder nd = new complainorder();
                        nd.order_Id = BaseId.getCompOrderId();
                        nd.productId = productId;
                        nd.productName = productName;
                        nd.complainId = cid;
                        var cp = complainDataContext.GetComplain(cid);
                        nd.complainName = cp == null ? "" : cp.name;
                        nd.oper_user = HttpContext.User.Identity.Name;
                        nd.oper_date = DateTime.Now;
                        nd.next_oper = next_oper;
                        nd.attach_url = fileUrl;
                        nd.staute = false;
                        nd.memo = memo;
                        nd.endate = enddate;
                        nd.last_oper = nd.oper_user;
                        nd.last_oper_date = DateTime.Now;
                        nd.last_memo = OperList.NewSave.Oper_Key;
                        if (complianOrderDataContext.AddComplianOrder(nd))
                        {
                            try
                            {
                                ViewData["msg"] = "投诉成功,已提交至:" + next_oper + "等待处理.";
                                string url = "http://" + Request.Url.Host + Url.Content("~/MyCtasks/Index") + "?orderId=" + nd.order_Id;
                                RTXMessage rtxMessage = new RTXMessage()
                                {
                                    MsgInfo = GetRTXMessage(nd) + string.Format("点击处理链接:{0}", url),
                                    Receiver = nd.next_oper,
                                    Title = string.Format("[政策电子流-投诉{0}]", nd.complainName),
                                    Priority = MessagePriority.Normal,
                                    Sender = nd.oper_user
                                };
                                MessageHelper.SendRTXMessage(rtxMessage);
                            }
                            catch { }
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

                setInitialData();

                return View("Index");
            }
        }
        [NonAction]
        private void setInitialData()
        {
            ViewData["cList"] = GeneralDropData.GetComplain(ref complianId);
            ViewData["tList"] = getTList();
            Initial("互娱投诉", "我要申请 > 互娱投诉");
            ViewData["pName"] = "No Product";
        }

        private SelectList getTList()
        {
            List<SelectListItem> timeList = new List<SelectListItem>();
            for (int i = 8; i <= 20; i++)
            {
                SelectListItem sli = new SelectListItem();
                sli.Text = i.ToString().PadLeft(2, '0');
                sli.Value = i.ToString();
                timeList.Add(sli);
            }
            return new SelectList(timeList, "Value", "Text", 8);
        }
        private string GetRTXMessage(complainorder cnd)
        {
            return string.Format("[政策电子流－投诉]{0}向您发起了关于<<{1}>>的{2}投诉事件,投诉内容如下：{3}", cnd.oper_user, cnd.productName, cnd.complainName, cnd.memo);
        }

    }
}
