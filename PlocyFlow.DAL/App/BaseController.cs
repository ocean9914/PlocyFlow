using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace PlocyFlow.DAL.App
{
    public abstract class BaseController : Controller
    {
        protected string PageTitle;
        protected string NavTitle;
        /// <summary>
        /// 初始化控制器各动作公共变量
        /// </summary>
        /// <param name="pageT">页面标题</param>
        /// <param name="naveT">导航标题</param>
        protected void Initial(string pageT,string naveT)
        {
            PageTitle = pageT;
            NavTitle = naveT;
            ViewData["title"] = PageTitle;
            ViewData["navtitle"] = NavTitle;
        }
    }
}
