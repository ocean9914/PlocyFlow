using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.IO;
using System.Web.Mvc;
using PlocyFlow.DAL.App;
using PlocyFlow.Models;
using PlocyFlow.DAL.CommonUtility;
namespace PlocyFlow.Controllers
{
    public class MyWorkedController : BaseController
    {
        //
        // GET: /MyWorked/
        private string deptId;
        private string productId;
        private string startD, endD;
        public ActionResult Index()
        {
            Initial("我的已办", "个人工作台 > 我的已办");
            setInitialData();
            return View();
        }
        public ActionResult setView(string deptTxt, string productTxt)
        {
            Initial("我的已办", "个人工作台 > 我的已办");
            deptId = deptTxt;
            productId = productTxt;
            setInitialData();
            return View("Index");
        }
        public ActionResult seek()
        {
            Initial("我的已办", "个人工作台 > 我的已办");
            deptId = Request["deptList"];
            productId = Request["productList"];
            startD = Request["startDateTxt"];
            endD = Request["endDateTxt"];
            string opType = Request["opType"];
            if (string.IsNullOrEmpty(opType))
            {
                setInitialData();
                return View("Index");
            }
            else
            { 
                DownFile();
                return new EmptyResult();
            }
            
        }
        [NonAction]
        private void setInitialData()
        {

            ViewData["dList"] = GeneralDropData.GetDeptOrAll(ref deptId);
            ViewData["proList"] = GeneralDropData.GetProductOrAll(ref productId, deptId);
            ViewData["deptTxt"] = deptId;
            ViewData["productTxt"] = productId;
            ViewData["dataSouce"] = MyWorkedList.GetMyWorked(deptId,productId,startD,endD,User.Identity.Name);
        }
        private void DownFile()
        {
            var dsouce = MyWorkedList.GetMyWorked(deptId, productId, startD, endD, User.Identity.Name);
            if (dsouce != null)
            {
                DataTable dt = ExportGenaral.getTab();
                foreach (var d in dsouce)
                {
                    DataRow row = dt.NewRow();
                    row[0] = d.OrderId;
                    row[1] = d.WorkedType;
                    row[2] = d.Staute;
                    row[3] = d.ProductId;
                    row[4] = d.ProductName;
                    row[5] = d.DeptId;
                    row[6] = d.DeptName;
                    row[7] = d.OperDate;
                    dt.Rows.Add(row);
                }
                string sheet=DateTime.Now.ToString("yyMMddHHmmss");
                string fileName = sheet + BaseId.getRadom(3)+".xls";
                string pfile= Server.MapPath("Download/"+fileName );
                if (!string.IsNullOrEmpty(dt.CreateExcel(pfile, sheet, 0, 0)))
                {
                    ExcelHelper.DownExcel(pfile, fileName);
                }
                
            }
        }
        
    }
}
