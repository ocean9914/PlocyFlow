using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PlocyFlow.Models;
using PlocyFlow.DAL.App;
using PlocyFlow.DAL.Context;
namespace PlocyFlow.Controllers
{
    public class BaseMainController : BaseController
    {
        #region 部门操作方法
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            string uid = User.Identity.Name;
            if (!PagePurview.verifyVisble(uid, "BaseMain/Index"))
            {
                return Redirect("~/Error/Accessdeny");
            }
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 部门表维护");
            ViewData["dataSouce"] = deptDataContext.GetDept();
            ViewData["msg"] = "";
            return View("Index");
        }
        public ActionResult AddDeptName(string idTxt,string nameTxt)
        {
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 部门表维护");
            int id = -1;
            int.TryParse(idTxt, out id);
            if (id!=-1 && nameTxt != null && nameTxt.Trim() != string.Empty)
            {
                if (deptDataContext.AddDeptName(id, nameTxt.Trim()))
                {
                    ViewData["msg"] = "添加成功";
                }
                else
                    ViewData["msg"] = "已存在部门:" + nameTxt;
            }
            else
                ViewData["msg"] = "部门名称不能为空";
            ViewData["dataSouce"] = deptDataContext.GetDept();
            
            return View("Index");
        }
        public ActionResult delDept(int id)
        {
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 部门表维护");
            if (deptDataContext.DelDept(id))
                ViewData["msg"] = "删除成功";
            else
                ViewData["msg"] = "删除失败";
            ViewData["dataSouce"] = deptDataContext.GetDept();
            return View("Index");
        }
        #endregion

        #region 政策类型操作方法
        
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ViewPolicy()
        {
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 政策类型维护");
            ViewData["dataSouce"] = policyDataContext.GetPolicy();
            ViewData["msg"] = "";
            return View("ViewPolicy");
        }
        public ActionResult AddPolicyName(string idTxt,string nameTxt)
        {
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 政策类型维护");
            if (idTxt != null && idTxt.Trim() != string.Empty)
            {
                if (policyDataContext.AddPolicy(idTxt.Trim(), nameTxt.Trim()))
                    ViewData["msg"] = "添加成功";
                else
                    ViewData["msg"] = "添加失败";
            }
            else
                ViewData["msg"] = "政策类型ID或名称不能为空";
            ViewData["dataSouce"] = policyDataContext.GetPolicy();
            return View("ViewPolicy");
        }
        public ActionResult delPolicy(int id)
        {
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 政策类型维护");
            if(policyDataContext.DelPolicy(id))
                ViewData["msg"] = "删除成功";
            else
                ViewData["msg"] = "删除失败";
            ViewData["dataSouce"] = policyDataContext.GetPolicy();
            return View("ViewPolicy");
        }
        #endregion

        #region 投诉类型操作方法
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ViewComplain()
        {
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 投诉表维护");
            ViewData["dataSouce"] = complainDataContext.GetComplain();
            ViewData["msg"] = "";
            return View("ViewComplain");
        }
        public ActionResult AddComplainName(string nameTxt)
        {
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 投诉表维护");
            if (nameTxt != null && nameTxt.Trim() != string.Empty)
            {
                if (complainDataContext.AddComplain(nameTxt.Trim()))
                {
                    ViewData["msg"] = "添加投诉类型成功";
                    nameTxt = "";
                }
                else
                    ViewData["msg"] = "添加投诉类型失败";
            }
            else
                ViewData["msg"] = "投诉名称不能为空";
            ViewData["dataSouce"] = complainDataContext.GetComplain();
            return View("ViewComplain");
        }
        public ActionResult DelComplain(int id)
        {
            Initial("基础数据维护", "系统管理 > 基础数据维护 > 投诉表维护");
            if (complainDataContext.DelComplain(id))
                ViewData["msg"] = "删除成功";
            else
                ViewData["msg"] = "删除失败";
            ViewData["dataSouce"] = complainDataContext.GetComplain();
            return View("ViewComplain");
        }
        #endregion

        #region 角色维护操作
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ViewRole()
        {
            Initial("基础数据维护", NavTitle = "系统管理 > 基础数据维护 > 角色表维护");
            ViewData["dataSouce"] = roleDataContext.GetRole();
            ViewData["msg"] = "";
            return View("ViewRole");
        }
        public ActionResult AddRoleName(string nameTxt)
        {
            Initial("基础数据维护", NavTitle = "系统管理 > 基础数据维护 > 角色表维护");
            if (nameTxt != null && nameTxt.Trim() != string.Empty)
            {
                if (roleDataContext.AddRole(nameTxt.Trim()))
                    ViewData["msg"] = "添加角色成功";
                else
                    ViewData["msg"] = "添加角色失败";
            }
            else
                ViewData["msg"] = "角色名称不能为空";
            ViewData["dataSouce"] = roleDataContext.GetRole();
            return View("ViewRole");
        }
        public ActionResult DelRole(int id)
        {
            Initial("基础数据维护", NavTitle = "系统管理 > 基础数据维护 > 角色表维护");
            if (roleDataContext.DelRole(id))
                ViewData["msg"] = "删除成功";
            else
                ViewData["msg"] = "删除失败";
            ViewData["dataSouce"] = roleDataContext.GetRole();
            return View("ViewRole");
        }
        #endregion

        #region 申报审批类别操作
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ViewApprove()
        {
            Initial("","系统管理 > 基础数据维护 > 申报审批表维护");
            ViewData["dataSouce"] = approve_typeDataContext.GetApprove_Type();
            ViewData["msg"] = "";
            return View("ViewApprove");
        }
        public ActionResult AddApproveName(string nameTxt)
        {
            Initial("", "系统管理 > 基础数据维护 > 申报审批表维护");
            if (nameTxt != null && nameTxt.Trim() != string.Empty)
            {
                if (approve_typeDataContext.AddApprove(nameTxt.Trim()))
                    ViewData["msg"] = "添加成功";
                else
                    ViewData["msg"] = "添加失败";
            }
            else
                ViewData["msg"] = "审批级别名称不能为空";
            ViewData["dataSouce"] = approve_typeDataContext.GetApprove_Type();
            return View("ViewApprove");
        }
        public ActionResult DelApprove(int id)
        {
            Initial("", "系统管理 > 基础数据维护 > 申报审批表维护");
            if (approve_typeDataContext.DelApprove(id))
                ViewData["msg"] = "删除成功";
            else
                ViewData["msg"] = "删除失败";
            ViewData["dataSouce"] = approve_typeDataContext.GetApprove_Type();
            return View("ViewApprove");
        }
        #endregion
    }
}
