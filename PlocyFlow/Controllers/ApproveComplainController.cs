using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Text;
using PlocyFlow.Models;
using PlocyFlow.DAL.CommonUtility;
namespace PlocyFlow.Controllers
{
    public class ApproveComplainController : Controller
    {
        //
        // GET: /ApproveComplain/
        private string orderId;
        private readonly string attachPath = "Complain";
        private ApproveComp curApprove;
        public ActionResult Index()
        {
            orderId = Request.QueryString["orderId"];
            if (orderId != null)
            {
                ViewData["orderId"] = orderId;
                curApprove = new ApproveComp(User.Identity.Name, orderId);
                if (curApprove.HasAuthority)
                {
                    curApprove.makeData();
                    switch (curApprove.Staute)
                    {
                        case "Right":
                            return View("FirstApprove", curApprove);
                        case "Close":
                            return View("LastApprove", curApprove);
                        default:
                            return Content("非法授权审批.");
                    }
                }
                else
                    return Content("非法授权审批.");
            }
            else
                return Content("投诉订单号为空");
        }
        private string getJs()
        {
            StringBuilder sbd = new StringBuilder();
            sbd.Append("var curWin = document.parentWindow;");
            sbd.Append(" var pareWin;");
            sbd.Append(" if (curWin) pareWin = curWin.parent;");
            sbd.Append(" else pareWin = window.parent;");
            sbd.Append(" pareWin = window.parent;");
            sbd.Append(" if (curWin) pareWin.reload();");
            sbd.Append(" else window.close();");
            return sbd.ToString();
        }
        public ActionResult save(string orderId, string memoTxt, string pmTxt, string oper_type)
        {
            if (orderId != null && memoTxt != null  && orderId.Trim() != "" && memoTxt.Trim() != "")
            {
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
                                return Content("附件保存发生错误,操作失败.");
                            }
                        }
                    }
                    else
                        return Content("附件太大(最大10MB)，操作失败.");
                }
                try
                {
                    curApprove = new ApproveComp(User.Identity.Name, orderId);
                    if (curApprove.HasAuthority)
                    {
                        curApprove.URL = Url;
                        int ot = -1;
                        if (oper_type != null && int.TryParse(oper_type, out ot))
                        {
                            bool result = false;
                            switch (ot)
                            {
                                case 0:
                                    result = curApprove.approve_to(memoTxt, pmTxt, fileUrl);

                                    break;
                                case 1:
                                    result = curApprove.approve_save(memoTxt, curApprove.ComplainOrder.oper_user, fileUrl);
                                    break;
                                case 2:
                                    result = curApprove.approve_reject(memoTxt, pmTxt, fileUrl);
                                    break;
                                case 3:
                                    result = curApprove.approve_close(memoTxt, "", fileUrl);
                                    break;
                            }
                            if (result)
                            {
                                ViewData["js"] = getJs();
                                return View("executeJs");
                            }
                            else
                            {
                                return Content("操作失败." + curApprove.Message);
                            }
                        }
                        else
                            return Content("操作类型错误.");
                    }
                    else
                        return Content("非法授权审批.");
                }
                catch
                {
                    return Content("系统发生错误,操作失败.");
                }

            }
            else
                return Content("非法授权审批.");
        }
    }
}
