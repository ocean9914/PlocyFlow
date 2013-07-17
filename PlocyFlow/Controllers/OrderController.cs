using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlocyFlow.Models;
namespace PlocyFlow.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /Order/
        private string orderId;
        private OrderDetail curOrder;
        public ActionResult Index()
        {
             orderId = Request.QueryString["orderId"];
             ViewData["orderId"] = orderId;
             if (orderId != null)
             {
                 curOrder = new OrderDetail(orderId, User.Identity.Name);
                 if (curOrder.HasOrder)
                 {
                     ViewData["title"] = curOrder.ProductName + "--" + curOrder.WorkedType;
                     ViewData["ulist"] = curOrder.UserList;
                     ViewData["elist"] = curOrder.ElapsedList;
                     return View("Index",curOrder);
                 }
                 else
                     return Content("订单不存在.");
             }
             else
                 return Content("非法授权审批.");
            
        }
        public ActionResult CloseOrder(string orderIdTxt)
        {
            orderId = orderIdTxt;
            if (orderId != null)
            {
                curOrder = new OrderDetail(orderId, User.Identity.Name);
                if (curOrder.HasOrder && curOrder.IsAdmin)
                {
                    ViewData["title"] = curOrder.ProductName + "--" + curOrder.WorkedType;
                    ViewData["ulist"] = curOrder.UserList;
                    ViewData["elist"] = curOrder.ElapsedList;
                    curOrder.CloseOrder();
                    return View("Index", curOrder);
                }
                else
                    return Content("权限不足.");
            }
            else
                return Content("非法授权审批.");
        }
        public ActionResult moveOrder(string mOrderIdTxt, string mNextOprTxt)
        {
            orderId = mOrderIdTxt;
            if (orderId != null)
            {
                curOrder = new OrderDetail(orderId, User.Identity.Name);
                if (curOrder.HasOrder && curOrder.IsAdmin)
                {
                    ViewData["title"] = curOrder.ProductName + "--" + curOrder.WorkedType;
                    ViewData["ulist"] = curOrder.UserList;
                    ViewData["elist"] = curOrder.ElapsedList;
                    curOrder.updateNextOpr(mNextOprTxt);
                    return View("Index", curOrder);
                }
                else
                    return Content("权限不足.");
            }
            else
                return Content("非法授权审批.");
        }
    }
}
