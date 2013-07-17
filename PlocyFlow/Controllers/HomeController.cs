using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PlocyFlow.Models;
using PlocyFlow.DAL.App;
using PlocyFlow.DAL.Context;
namespace PlocyFlow.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            cacheBase.initialBase(Url);
            Initial("欢迎进入" + ConfigUtility.SystemName, "快速导航");
            ViewData["title"] = PageTitle;
            ViewData["navtitle"] = NavTitle;

            return View();
        }
        public ContentResult GetVeryfy()
        {
            string url = Url.Content("~/MyGtasks/Index");
            StringBuilder sbd = new StringBuilder();
            var myvery = declareOrderDataContext.GetMyGtasks(User.Identity.Name, "", "", "", "", "");
            if (myvery != null )
            {
                foreach (var d in myvery)
                {
                    sbd.AppendFormat("<li class=\"list_img\"><a href=\"{0}?orderId={1}\">申报{2}-{3}</a></li>",url,d.order_Id,d.policyName,d.ProductName);
                }
            }
            url = Url.Content("~/MyCtasks/Index");
            var mycvery = complianOrderDataContext.GetMyCtasks(User.Identity.Name, "", "", "", "", "");
            if (mycvery != null)
            {
                foreach (var d in mycvery)
                {
                    sbd.AppendFormat("<li class=\"list_img\"><a href=\"{0}?orderId={1}\">投诉{2}-{3}</a></li>",url,d.order_Id, d.complainName , d.ProductName);
                }
            }
            return Content(sbd.ToString(), "text/text", Encoding.UTF8);
        }
    }
}
