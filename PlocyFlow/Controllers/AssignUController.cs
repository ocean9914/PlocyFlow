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
namespace PlocyFlow.Controllers
{
    public class AssignUController : BaseController
    {
        //
        // GET: /AssignU/

        public ActionResult Index()
        {
            string uid = User.Identity.Name;
            if (!PagePurview.verifyVisble(uid, "AssignU/Index"))
            {
                return Redirect("~/Error/Accessdeny");
            }
            Initial("角色管理", "系统管理 > 申报用户指派");
            setDropList();
            ViewData["msg"] = "";
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult saveAssign(string ulist,string pid,string aid)
        {
            string uid = User.Identity.Name;
            if (!PagePurview.verifyVisble(uid, "AssignU/Index"))
            {
                return Redirect("~/Error/Accessdeny");
            }
            Initial("角色管理", "系统管理 > 申报用户指派");
            int proid = -1;
            int.TryParse(pid, out proid);
            int aprid = -1;
            int.TryParse(aid, out aprid);
            if (ulist.Trim() != "" && pid.Trim() != "" && aid.Trim() != "")
            {
                string[] list = ulist.Split(new char[] {','});
                List<int> userList = new List<int>();
                foreach (string s in list)
                {
                    if (s != "")
                    {
                        int cu = -1;
                        int.TryParse(s, out cu);
                        if (cu != -1)
                            userList.Add(cu);
                    }
                }
                if (userList.Count > 0 && proid!=-1 && aprid!=-1
                    && aprove_userDataContext.updateApprove_User(proid,aprid,userList.ToArray()))
                {
                    ViewData["msg"] = "保存成功";    
                }
                else
                    ViewData["msg"] = "保存失败";
            }
            else
                ViewData["msg"] = "保存失败";
            if (proid != -1 && aprid != -1)
                setDropList(proid, aprid);
            else
                setDropList();
            return View("Index");
        }
        [NonAction]
        public void setDropList()
        {
            var plist = policyDataContext.GetPolicy();
            var souce = new SelectList(plist, "id", "p_name");
            if (souce != null && souce.Count() > 0)
                souce.First().Selected = true;
            ViewData["pList"] = souce;
            var alist = approve_typeDataContext.GetApprove_Type();
            souce = new SelectList(alist, "id", "name");
            if (souce != null && souce.Count() > 0)
                souce.First().Selected = true;
            ViewData["aList"] = souce;
            var cklist = t_userDataContext.GetT_User();
            ViewData["ckList"] = cklist;
            var paulist = aprove_userDataContext.GetAprove_user();
            string paul = "";
            StringBuilder sbd = new StringBuilder();
            if (paulist != null)
            {
                foreach (var pau in paulist)
                {
                    sbd.Append(pau.pid.ToString() + "#" + pau.aid.ToString() + "#" + pau.uid.ToString() + ",");
                }
                paul = sbd.ToString();
                if (paul != "")
                    paul = paul.Substring(0, paul.Length - 1);
            }
            ViewData["paul"] = paul;
        }
        [NonAction]
        public void setDropList(int pid ,int aid)
        {
            var plist = policyDataContext.GetPolicy();
            var souce = new SelectList(plist, "id", "p_name",pid);
            ViewData["pList"] = souce;
            var alist = approve_typeDataContext.GetApprove_Type();
            souce = new SelectList(alist, "id", "name",aid);
            ViewData["aList"] = souce;
            var cklist = t_userDataContext.GetT_User();
            ViewData["ckList"] = cklist;
            var paulist = aprove_userDataContext.GetAprove_user();
            string paul = "";
            StringBuilder sbd = new StringBuilder();
            if (paulist != null)
            {
                foreach (var pau in paulist)
                {
                    sbd.Append(pau.pid.ToString() + "#" + pau.aid.ToString() + "#" + pau.uid.ToString() + ",");
                }
                paul = sbd.ToString();
                if (paul != "")
                    paul = paul.Substring(0, paul.Length - 1);
            }
            ViewData["paul"] = paul;
        }
    }
}
