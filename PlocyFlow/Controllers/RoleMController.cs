using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using PlocyFlow.DAL.Entity;
using PlocyFlow.Models;
using PlocyFlow.DAL.App;
using PlocyFlow.DAL.Context;
using PlocyFlow.DAL.CommonUtility;
namespace PlocyFlow.Controllers
{
    public class RoleMController : BaseController
    {
        //
        // GET: /RoleM/

        public ActionResult Index(int? id)
        {
            string uid = User.Identity.Name;
            if (!PagePurview.verifyVisble(uid, "RoleM/Index"))
            {
                return Redirect("~/Error/Accessdeny");
            }
            else
            {
                Initial("角色管理", "系统管理 > 角色管理");
                setRoleList();
                ViewData["msg"] = "";
                return View();
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveRole(string roleid, string pagelist)
        {
            string uid = User.Identity.Name;
            if (!PagePurview.verifyVisble(uid, "RoleM/Index"))
            {
                return Redirect("~/Error/Accessdeny");
            }
            else
            {
                Initial("角色管理", "系统管理 > 角色管理");
                int rid = -1;
                if (roleid != null)
                    int.TryParse(roleid, out rid);
                if (pagelist != null && pagelist.Trim() != string.Empty)
                {
                    string[] ss = pagelist.Split(new char[] { ',' });
                    List<int> plist = new List<int>();
                    foreach (string s in ss)
                    {
                        if (s.Trim() != "")
                        {
                            int pid = -1;
                            int.TryParse(s, out pid);
                            if (pid != -1)
                                plist.Add(pid);
                        }
                    }
                    if (plist.Count > 0)
                    {
                        if (RoleManager.setRole(rid, plist.ToArray()))
                            ViewData["msg"] = "保存成功";
                        else
                            ViewData["msg"] = "保存失败";
                    }
                }
                else
                    if (RoleManager.removeAllRole(rid))
                        ViewData["msg"] = "保存成功";
                    else
                        ViewData["msg"] = "保存失败";
                setRoleList(rid);
                return View("Index");
            }
        }

        [NonAction]
        private void setRoleList(int idx)
        {
            var rlist = roleDataContext.GetRole();
            ViewData["rList"] = new SelectList(rlist, "id", "name", idx);
            var rtp = new List<RoleToPage>();
            var plist = t_pageDataContext.GetT_Page();
            if (plist != null)
            {
                foreach (var p in plist)
                {
                    RoleToPage tp = new RoleToPage();
                    tp.pageId = p.id.ToString();
                    tp.pageName = p.page_name;
                    tp.roleList = "";
                    if (rlist != null)
                    {
                        StringBuilder sbd = new StringBuilder();
                        foreach (role r in rlist)
                        {
                            if (CommonUtility.Comp(Convert.ToUInt64(r.vcode, 16), Convert.ToUInt64(p.vcode, 16)) > 0)
                                sbd.Append(r.id.ToString() + ",");
                        }
                        string temp = sbd.ToString();
                        if (temp != string.Empty)
                            temp = temp.Substring(0, temp.Length - 1);
                        tp.roleList = temp;
                    }
                    rtp.Add(tp);
                }
            }
            ViewData["ckList"] = rtp;
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
            if (roleList.Count > 0)
                roleList[0].Selected = true;
            ViewData["roleList"] = roleList;
            var rtp = new List<RoleToPage>();
            var plist = t_pageDataContext.GetT_Page();
            if (plist != null)
            {
                foreach (var p in plist)
                {
                    RoleToPage tp = new RoleToPage();
                    tp.pageId = p.id.ToString();
                    tp.pageName = p.page_name;
                    tp.roleList = "";
                    if (rlist != null)
                    {
                        StringBuilder sbd = new StringBuilder();
                        foreach (role r in rlist)
                        {
                            if (CommonUtility.Comp(Convert.ToUInt64(r.vcode, 16), Convert.ToUInt64(p.vcode, 16)) > 0)
                                sbd.Append(r.id.ToString() + ",");
                        }
                        string temp = sbd.ToString();
                        if (temp != string.Empty)
                            temp = temp.Substring(0, temp.Length - 1);
                        tp.roleList = temp;
                    }
                    rtp.Add(tp);
                }
            }
            ViewData["ckList"] = rtp;
        }



    }
}
