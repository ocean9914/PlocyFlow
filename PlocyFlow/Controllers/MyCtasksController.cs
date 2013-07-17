using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlocyFlow.DAL.App;
using PlocyFlow.DAL.Context;
using PlocyFlow.DAL.CommonUtility;
using PlocyFlow.DAL.Entity;
using PlocyFlow.Models;
namespace PlocyFlow.Controllers
{
    public class MyCtasksController : BaseController
    {
        //
        // GET: /MyCtasks/
        private string deptId;
        private string productId;
        private string complainId;
        private string startD, endD;
        public ActionResult Index()
        {
            Initial("我的待办", "个人工作台 > 我的投诉待办");
            setInitialData();
            return View();
        }
        public ActionResult setView(string deptTxt, string compTxt, string productTxt)
        {
            Initial("我的待办", "个人工作台 > 我的投诉待办");
            deptId = deptTxt;
            productId = productTxt;
            complainId = compTxt;
            setInitialData();
            return View("Index");
        }
        public ActionResult seek()
        {
            Initial("我的待办", "个人工作台 > 我的申报待办");
            deptId = Request["deptList"];
            productId = Request["productList"];
            startD = Request["startDateTxt"];
            endD = Request["endDateTxt"];
            complainId = Request["compList"];
            setInitialData();
            return View("Index");
        }
        [NonAction]
        private void setInitialData()
        {
            string orderId = Request.QueryString["orderId"];
            if (orderId != null && orderId.Trim() != "")
            {
                var dsouce = complianOrderDataContext.GetMyCtasks(orderId);
                if (dsouce != null)
                {
                    var temp = dsouce as List<v_complain_order>;
                    if (temp != null && temp.Count > 0)
                    {
                        var first = temp[0];
                        if (first.Departmentid.HasValue)
                            deptId = first.Departmentid.Value.ToString();
                        complainId = first.complainId.ToString();
                        productId = first.productId;
                    }

                }
                ViewData["dList"] = GeneralDropData.GetDeptOrAll(ref deptId);
                ViewData["proList"] = GeneralDropData.GetProductOrAll(ref productId, deptId);
                ViewData["cList"] = GeneralDropData.GetComplainOrAll(ref complainId);
                ViewData["dataSouce"] = dsouce;
            }
            else
            {
                ViewData["dList"] = GeneralDropData.GetDeptOrAll(ref deptId);
                ViewData["proList"] = GeneralDropData.GetProductOrAll(ref productId, deptId);
                ViewData["cList"] = GeneralDropData.GetComplainOrAll(ref complainId);
                ViewData["dataSouce"] = complianOrderDataContext.GetMyCtasks(User.Identity.Name, deptId, productId, complainId, startD, endD);
            }
            ViewData["deptTxt"] = deptId;
            ViewData["productTxt"] = productId;
            ViewData["compTxt"] = complainId;
        }
    }
}
