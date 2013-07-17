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
    public class MyGtasksController : BaseController
    {
        //
        // GET: /MyGtasks/
        private string deptId;
        private string productId;
        private string policyId;
        private string startD, endD;
        public ActionResult Index()
        {
            Initial("我的待办", "个人工作台 > 我的申报待办");
            setInitialData();
            return View();
        }
        public ActionResult setView(string deptTxt, string policyTxt, string productTxt)
        {
            Initial("我的待办", "个人工作台 > 我的申报待办");
            deptId = deptTxt;
            policyId = policyTxt;
            productId = productTxt;
            setInitialData();
            return View("Index");
        }
        public ActionResult seek()
        {
            Initial("我的待办", "个人工作台 > 我的申报待办");
            deptId = Request["deptList"];
            policyId = Request["policyList"];
            productId = Request["productList"];
            startD = Request["startDateTxt"];
            endD = Request["endDateTxt"];
            setInitialData();
            return View("Index");
        }
        [NonAction]
        public void setInitialData()
        {
            string orderId = Request.QueryString["orderId"];
            if (orderId != null && orderId.Trim() != "")
            {
                var dsouce = declareOrderDataContext.GetMyGtasks(orderId);
                if (dsouce != null )
                {
                    var temp = dsouce as List<v_declare_order>;
                    if (temp != null && temp.Count>0)
                    {
                        var first=temp[0];
                        if (first.dept_id.HasValue)
                            deptId = first.dept_id.Value.ToString();
                        policyId = first.policyId;
                        productId = first.product_Id;
                    }
                }
                ViewData["dList"] = GeneralDropData.GetDeptOrAll(ref deptId);
                ViewData["pList"] = GeneralDropData.GetPolicyOrAll(ref policyId);
                ViewData["proList"] = GeneralDropData.GetProductOrAll(ref productId, deptId, policyId);
                ViewData["dataSouce"] = dsouce;
            }
            else
            {
                ViewData["dList"] = GeneralDropData.GetDeptOrAll(ref deptId);
                ViewData["pList"] = GeneralDropData.GetPolicyOrAll(ref policyId);
                ViewData["proList"] = GeneralDropData.GetProductOrAll(ref productId, deptId, policyId);
                ViewData["dataSouce"] = declareOrderDataContext.GetMyGtasks(User.Identity.Name, deptId, policyId, productId, startD, endD);
            }
            ViewData["deptTxt"] = deptId;
            ViewData["policyTxt"] = policyId;
            ViewData["productTxt"] = productId;
        }

    }
}
