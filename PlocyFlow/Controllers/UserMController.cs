using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PlocyFlow.DAL.Entity;
using PlocyFlow.Models;
using PlocyFlow.DAL.App;
using PlocyFlow.DAL.Context;
using PlocyFlow.DAL.CommonUtility;
using Tencent.OA.Framework;
using Tencent.OA.Framework.Organization;
namespace PlocyFlow.Controllers
{
    public class UserMController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(int? id)
        {
            string uid = User.Identity.Name;
            if (!PagePurview.verifyVisble(uid, "UserM/Index"))
            {
                return Redirect("~/Error/Accessdeny");
            }
            else
            {
                Initial("用户管理", "系统管理 > 用户管理");
                setRoleList();
                ViewData["msg"] = "";
                ViewData["dataSouce"] = user_roleDataContext.GetUser_Role();
                return View();
            }
        }
        public ActionResult AddUser(string userTxt, string roleid)
        {
            string uid = User.Identity.Name;
            if (!PagePurview.verifyVisble(uid, "UserM/Index"))
            {
                return Redirect("~/Error/Accessdeny");
            }
            else
            {
                Initial("用户管理", "系统管理 > 用户管理");
                if (userTxt.Trim() != "" && roleid != "")
                {
                    var uid_new = GetStaffByName(userTxt.Trim());
                    if (uid_new > 0)
                    {
                        if (t_userDataContext.AddUser(userTxt.Trim(), uid_new, Convert.ToInt32(roleid)))
                            ViewData["msg"] = "添加成功";
                        else
                            ViewData["msg"] = "添加失败";
                    }
                }
                setRoleList();
                ViewData["dataSouce"] = user_roleDataContext.GetUser_Role();
                return View("Index");
            }
        }
        public ActionResult delUser(int id)
        {
             string uid = User.Identity.Name;
             if (!PagePurview.verifyVisble(uid, "UserM/Index"))
             {
                 return Redirect("~/Error/Accessdeny");
             }
             else
             {
                 Initial("用户管理", "系统管理 > 用户管理");
                 t_userDataContext.DelUser(id);
                 setRoleList();
                 ViewData["dataSouce"] = user_roleDataContext.GetUser_Role();
                 return View("Index");
             }
        }
        public ActionResult UpdateUser(string uidTxt, string roleIdTxt)
        {
             string uid = User.Identity.Name;
             if (!PagePurview.verifyVisble(uid, "UserM/Index"))
             {
                 return Redirect("~/Error/Accessdeny");
             }
             else
             {
                 Initial("用户管理", "系统管理 > 用户管理");
                 t_userDataContext.UpdateUser(Convert.ToInt32(uidTxt), Convert.ToInt32(roleIdTxt));
                 setRoleList();
                 ViewData["dataSouce"] = user_roleDataContext.GetUser_Role();
                 return View("Index");
             }
        }
        [NonAction]
        private void setRoleList()
        {
            var rlist = roleDataContext.GetRole();
            var roleList = new List<SelectListItem>();
            if (rlist != null)
            {
                foreach (role r in rlist)
                {
                    var l = new SelectListItem();
                    l.Text = r.name;
                    l.Value = r.id.ToString();
                    roleList.Add(l);
                }
            }
            ViewData["defaulRoleID"] = 0;
            if (roleList.Count > 0)
            {
                roleList[0].Selected = true;
                ViewData["defaulRoleID"] = roleList[0].Value;
            }
            ViewData["roleList"] = roleList;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ContentResult GetUserList()
        {
            var cacheName = "UserList";
            var ulist = cacheBase.GetCache(cacheName);
            if (ulist == null)
            {
                string r = getAllStaff();
                cacheBase.addCache(cacheName, r);
                ulist = r;
            }
            return Content(ulist.ToString(), "text/plain");
        }
        [NonAction]
        private string getAllStaff()
        {
            StringBuilder sbd = new StringBuilder();
            StaffCollection staffs = Staff.GetAllStaff(true);
            if (staffs != null && staffs.Count > 0)
            {
                sbd.Append(staffs[0].FullName.Trim().ToLower());
                for (int i = 1; i < staffs.Count; i++)
                {
                    Staff sa = staffs[i];
                    if (sa.StatusId != 1000 && sa.StatusId != 2)
                    {
                        sbd.Append("," + sa.FullName.Trim().ToLower());
                    }
                }
            }
            return sbd.ToString();
        }
        [NonAction]
        private int GetStaffByName(string name)
        {
            var staffID = 0;
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    Staff staff = new Staff(name);
                    staffID = staff.ID;
                }
                catch
                {
                    staffID = 0;
                }
            }

            return staffID;
        }
    }
}
