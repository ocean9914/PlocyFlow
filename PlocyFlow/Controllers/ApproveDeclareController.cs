using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using PlocyFlow.Models;
namespace PlocyFlow.Controllers
{
    public class ApproveDeclareController : Controller
    {
        //
        // GET: /ApproveDeclare/
        private string orderId;
        private Approve curApprove;
        public ActionResult Index()
        {
            orderId = Request.QueryString["orderId"];
            if (orderId != null)
            {
                ViewData["orderId"] = orderId;
                curApprove = new Approve(User.Identity.Name, orderId);
                if (curApprove.HasAuthority)
                {
                    curApprove.makeData();
                    switch (curApprove.Staute)
                    {
                        case "初审":
                            return View("FirstApprove", curApprove);
                        case "终审":
                            return View("LastApprove", curApprove);
                        default:
                            return Content("非法授权审批.");
                    }
                }
                else
                    return Content("非法授权审批.");
            }
            else
                return Content("申报订单号为空");
        }
        public ActionResult save(string orderId, string memoTxt, string oper_type)
        {
            if (orderId != null && memoTxt != null && orderId.Trim() != "" && memoTxt.Trim() != "")
            {
                curApprove = new Approve(User.Identity.Name, orderId);
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
                                result = curApprove.approve_reject(memoTxt);
                                break;
                            case 1:
                                result = curApprove.approve_save(memoTxt);
                                break;
                            case 2:
                                result = curApprove.approve_close(memoTxt);
                                break;
                        }
                        if (result)
                        {
                            ViewData["js"] = getJs();
                            return View("executeJs");
                        }
                        else
                        {
                            return Content("操作类型失败." + curApprove.Message);
                        }
                    }
                    else
                        return Content("操作类型错误.");
                }
                else
                    return Content("非法授权审批.");

            }
            else
                return Content("非法授权审批.");
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
    }
}
