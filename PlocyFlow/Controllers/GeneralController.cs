using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlocyFlow.Models;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.Context;
using PlocyFlow.DAL.CommonUtility;
using Tencent.OA.Framework;
using Tencent.OA.Framework.Organization;
namespace PlocyFlow.Controllers
{
    public class GeneralController : Controller
    {
        //
        // GET: /General/
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult getProduct()
        {
            if (Request["q"] != null)
            {
                string q = Request["q"].Trim();
                var plist = bip_view.GetBipDataByName(q);
                List<string> product = new List<string>();
                List<Product> prolist = new List<Product>();
                bool done = true;
                Product cp = null;
                foreach (var p in plist)
                {
                    if (!product.Contains(p.Productid))
                    {
                        done = true;
                        cp = new Product();
                        cp.Productid = p.Productid;
                        cp.ProductName = p.ProductName;
                        cp.DepartId = p.Departmentid.HasValue ? p.Departmentid.Value.ToString() : " ";
                        cp.DepartName = p.DeptName;
                        cp.ProductTypeId = p.ProductTypeid;
                        cp.ProductTypeName = p.ProductTypeName;
                        cp.OBDate = p.OBDate.HasValue ? p.OBDate.Value.ToString("yyyy-MM-dd") : " ";
                        cp.Game_TypeId = p.GameTypeid;
                        cp.Game_TypeName = p.Game_Name;
                        cp.Statusid = p.Statusid.HasValue ? p.Statusid.Value.ToString() : " ";
                        cp.StatusName = p.StatusName;
                        cp.Manager = p.Manager;
                        cp.PolicyId = p.Policyid;
                        cp.PolicyName = p.PolicyName;
                        done = done & p.IsDone.HasValue ? p.IsDone.Value : false;
                        cp.IsDone = CommonUtility.HasBoolValue(p.IsDone, "未完成", "已完成", "未知");
                        cp.Remark = p.Remark;
                        prolist.Add(cp);
                        product.Add(p.Productid);
                    }
                    else
                    {
                        cp.PolicyName += "；" + p.PolicyName;
                        cp.IsDone = done ? "已完成" : "未完成";
                    }
                }
                return Json(prolist, JsonRequestBehavior.AllowGet);
            }
            else
                return new EmptyResult();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult getStaff()
        {
            if (Request["q"] != null)
            {
                string q = Request["q"].Trim();
                string str = (q ?? "").ToLowerInvariant();
                var cacheName = "MyUser";
                var ulist = cacheBase.GetCache(cacheName);
                if (ulist == null)
                {
                    StaffCollection r = Staff.GetAllStaff(true);
                    cacheBase.addCache(cacheName, r);
                    ulist = r;
                }
                StaffCollection staffs = ulist as StaffCollection;
                if (staffs != null && staffs.Count > 0)
                {
                    var resData = (from staff in staffs
                                   where staff.FullName.ToLowerInvariant().StartsWith(str) 
                                   orderby staff.FullName.ToLowerInvariant() ascending
                                   select new
                                   {
                                       ID = staff.ID,
                                       Name = staff.FullName,
                                       ChineseName = staff.ChineseName,
                                       EnglishName = staff.EnglishName,
                                       PostName = staff.PostName,
                                       PostID = staff.PostId,
                                       DepartmentId = staff.DepartmentId,
                                       DepartmentName = staff.DepartmentName,
                                       TelNO = staff.BranchPhoneNumber,
                                       StaffLevel = staff.OfficialName,
                                       StaffLevelID = staff.OfficialId,
                                       SeatNO = staff.SeatPosition,
                                       MobileNO = staff.MobilePhoneNumber
                                   }).Take(50).ToList();
                    if(resData!=null)
                        return Json(resData, JsonRequestBehavior.AllowGet);
                    else
                        return new EmptyResult();
                }
                else
                    return new EmptyResult();    
            }
            else
                return new EmptyResult();
        }
    }
}
