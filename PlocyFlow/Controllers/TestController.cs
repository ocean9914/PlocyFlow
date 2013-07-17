using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using PlocyFlow.Models;
using PlocyFlow.DAL.App;
namespace PlocyFlow.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        private static int stauteId = 0;
        public ActionResult Index()
        {
            ViewData["type"] = "DropDownList test";
            List<SelectListItem> l = new List<SelectListItem>();
            SelectListItem li = new SelectListItem();
            li.Text = "First"; li.Value = "1";
            l.Add(li);
            li = new SelectListItem();
            li.Text = "Second";
            li.Value = "2";
            l.Add(li);
            ViewData["roleList"] = new SelectList(l, "Value", "Text", 2);
            return View();
        }
        public ContentResult Bip()
        {
            string filename = Server.MapPath(Url.Content("~/download/bip.xml"));
            string content = "";
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                content = sr.ReadToEnd();
            }
            return Content(content, "text/xml", Encoding.UTF8);
        }
        public ContentResult GetBipData()
        {
            string filename = Server.MapPath(Url.Content("~/download/bip.xml"));
            string content = "";
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                content = sr.ReadToEnd();
            }
            return Content(new GetBipData().getData(content).ToString(), "text/xml", Encoding.UTF8);
        }
        public ContentResult GetVeryfy()
        {
            StringBuilder sbd = new StringBuilder();
            for (int i = 0; i < 20; i++)
            {
                sbd.Append("<li>待办" + Convert.ToString(++stauteId) + "测试列表</li>");
            }
           return Content( sbd.ToString(),"text/text",Encoding.UTF8);
        }
        public ContentResult GetCurPageName()
        {
            StringBuilder sbd = new StringBuilder();
            sbd.AppendLine(Request.Url.AbsoluteUri);
            sbd.AppendLine(Request.Url.AbsolutePath);
            sbd.AppendLine(Request.Url.LocalPath);
            return Content(sbd.ToString());
        }
    }
}
