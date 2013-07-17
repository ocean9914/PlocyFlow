using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using System.Data.Entity;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.Context;

namespace PlocyFlow.Models
{
    public static class cacheBase
    {
        public static string menulist;
        public static string shortcut;
        private static UrlHelper URL;
        public static void addCache(string cacheName, object value)
        {
            var cache = HttpContext.Current.Cache[cacheName];
            if (cache == null)
                HttpContext.Current.Cache.Insert(cacheName, value);
        }
        public static void removeCache(string cacheName)
        {
            var cache = HttpContext.Current.Cache[cacheName];
            if (cache != null)
                HttpContext.Current.Cache.Remove(cacheName);
        }
        public static object GetCache(string cacheName)
        {
            var cache = HttpContext.Current.Cache[cacheName];
            if (cache != null)
                return cache;
            else
                return null;
        }
        public static void initialBase(UrlHelper u)
        {
            URL = u;
            string uname = HttpContext.Current.User.Identity.Name;
            var cache = HttpContext.Current.Cache["menulist"];
            if (cache == null)
            {
                createMenu(uname);
                addCache("menulist", menulist);
            }
        }
        public static void EmptyCache()
        {
            removeCache("menulist");
            removeCache("UserList");
        }
        private static void createMenu(string uid)
        {
            menulist = "";
            shortcut = "";
            //TreeView1.Nodes.Clear();

            T_Menu menu = new T_Menu(uid);
            if (menu.HasMenu)
            {
                if (menu.MenuTree.IsRoot)
                {
                    StringBuilder sbd = new StringBuilder();
                    StringBuilder shortsbd = new StringBuilder();
                    shortsbd.AppendFormat("<li> <a href='{0}'>{1}</a></li>", URL.Action("Index", "Home"),"首页");
                    foreach (Logical_Menu_Tree lmt in menu.MenuTree.Childs)
                    {
                        string imgsrc = getImgSrc(lmt.NodeName);
                        if (imgsrc != "")
                        {
                            sbd.Append("<dl class=\"mt_10 tree_dl\">");
                            if (lmt.IsLeaf)
                            {
                                sbd.AppendFormat("<a href=\"{0}\"><img src=\"{1}\" /></a>",URL.Content("~/"+ lmt.PageUrl), imgsrc);
                                if (lmt.CurrentPage.IsShortCut)
                                    shortsbd.AppendFormat("<li> <a href='{0}'>{1}</a></li>",URL.Content("~/"+ lmt.PageUrl), lmt.NodeName);
                            }
                            else
                                sbd.Append(string.Format("<dt><img src=\"{0}\"></a></dt>", imgsrc));
                            for (int j = 0; j < lmt.Childs.Count; j++)
                            {
                                Logical_Menu_Tree clmt = lmt.Childs[j];
                                sbd.Append(string.Format("<dd><a href=\"{0}\">{1}</a></dd>",URL.Content("~/"+ clmt.PageUrl), clmt.NodeName));
                                if(clmt.CurrentPage.IsShortCut)
                                    shortsbd.AppendFormat("<li> <a href='{0}'>{1}</a></li>",URL.Content("~/"+ clmt.PageUrl), clmt.NodeName);
                            }
                            sbd.Append("</dl>");
                        }
                    }
                    menulist = sbd.ToString();
                    shortcut = shortsbd.ToString();
                }
            }

        }
        private static string getImgSrc(string pageName)
        {
            string r = "";

            if (pageName.IndexOf("个人工作台") != -1)
                r = URL.Content( "~/Content/images/grgzt_bg.gif");
            if (pageName.IndexOf("我要申请") != -1)
                r = URL.Content( "~/Content/images/qbsy_bg.gif");
            if (pageName.IndexOf("系统管理") != -1)
                r = URL.Content( "~/Content/images/xtgl_bg.gif");
            return r;
        }
    }
    public class GeneralDropData
    {
        public static SelectList GetDept(ref string deptId)
        {
            var temp = deptDataContext.GetDept();
            if (temp != null && temp.Count() > 0)
            {
                if (deptId == null || deptId.Trim()=="")
                    deptId = temp.First().d_id.ToString();
                return new SelectList(temp, "d_id", "d_name", deptId);
            }
            else
            {
                return getEmptySli("无", "0");
            }
        }
        public static SelectList GetDeptOrAll(ref string deptId)
        {
            var souce = getEmptyList();
            var temp = deptDataContext.GetDept();
            if (temp != null && temp.Count() > 0)
            {
                foreach (var d in temp)
                {
                    var sli = new SelectListItem();
                    sli.Text = d.d_name;
                    sli.Value = d.d_id.ToString();
                    souce.Add(sli);
                }
            }
            if (deptId == null || deptId.Trim()=="")
                deptId = "0";
            return new SelectList(souce, "Value", "Text", deptId);
        }
        public static SelectList GetPolicy(ref string policyId)
        {
            var p = policyDataContext.GetPolicy();
            if (p != null && p.Count() > 0)
            {
                if (policyId == null || policyId.Trim()=="")
                    policyId = p.First().p_id;
                return new SelectList(p, "p_id", "p_name", policyId);
            }
            else
                return getEmptySli("无", "0");
        }
        public static SelectList GetPolicyOrAll(ref string policyId)
        {
            var souce = getEmptyList();
            var p = policyDataContext.GetPolicy();
            if (p != null && p.Count() > 0)
            {
                foreach (var cp in p)
                {
                    var sli = new SelectListItem();
                    sli.Text = cp.p_name;
                    sli.Value = cp.p_id;
                    souce.Add(sli);
                }
            }
            if (policyId == null || policyId.Trim()=="")
                policyId = "0";
            return new SelectList(souce, "Value", "Text", policyId);
        }
        public static SelectList GetStaute(ref string stauteId)
        {
            var s = game_statusDataContext.GetGame_Status();
            if (s != null && s.Count() > 0)
            {
                if (stauteId == null || stauteId.Trim()=="")
                    stauteId = s.First().s_id.ToString();
                return new SelectList(s, "s_id", "s_name", stauteId);
            }
            else
                return  getEmptySli("无", "0");
        }
        public static SelectList GetStauteOrAll(ref string stauteId)
        {
            var souce = getEmptyList();
             var s = game_statusDataContext.GetGame_Status();
             if (s != null && s.Count() > 0)
             {
                 foreach (var gs in s)
                 {
                     var sli = new SelectListItem();
                     sli.Text = gs.s_name;
                     sli.Value = gs.s_id.ToString();
                     souce.Add(sli);
                 }
             }
             if (stauteId == null || stauteId.Trim() == "")
                 stauteId = "0";
             return new SelectList(souce, "Value", "Text", stauteId);
        }
        public static SelectList GetProduct(ref string productId,string deptId,string policyId)
        {
            var dt = GetProductTab(deptId, policyId);
            if (dt != null && dt.Rows.Count > 0)
            {
                productId = dt.Rows[0][0].ToString();
                return new SelectList(dt.Rows, "productid", "productName", productId);
            }
            else
                return getEmptySli("无", "0");
        }
        public static SelectList GetProduct(ref string productId, string deptId)
        {
            var dt = GetProductTab(deptId, "");
            if (dt != null && dt.Rows.Count > 0)
            {
                productId = dt.Rows[0][0].ToString();
                return new SelectList(dt.Rows, "productid", "productName", productId);
            }
            else
                return getEmptySli("无", "0");
        }
        public static SelectList GetProductOrAll(ref string productId, string deptId, string policyId)
        {
             var souce = getEmptyList();
             var dt = GetProductTab(deptId, policyId);
             if (dt != null && dt.Rows.Count > 0)
             {
                 foreach (DataRow tr in dt.Rows)
                 {
                     var sli = new SelectListItem();
                     sli.Text = tr[1].ToString() ;
                     sli.Value = tr[0].ToString();
                     souce.Add(sli);
                 }
             }
             if (productId == null || productId.Trim() == "")
                 productId = "0";
             return new SelectList(souce, "Value", "Text", productId);
        }
        public static SelectList GetProductOrAll(ref string productId, string deptId)
        {
            var souce = getEmptyList();
            var dt = GetProductTab(deptId, "");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow tr in dt.Rows)
                {
                    var sli = new SelectListItem();
                    sli.Text = tr[1].ToString();
                    sli.Value = tr[0].ToString();
                    souce.Add(sli);
                }
            }
            if (productId == null || productId.Trim() == "")
                productId = "0";
            return new SelectList(souce, "Value", "Text", productId);
        }
        public static DataTable GetProductTab(string deptId,string policyId)
        {
            string sql = "select distinct productid,productName from bip_data ";
            bool hasWhere = false;
            int did = 0;
            int.TryParse(deptId, out did);
            if (did!=0)
            {
                hasWhere = true;
                sql += "  where departmentid=" + deptId;
            }
            if (policyId != null && policyId.Trim() != "" && policyId != "0")
            {
                if (hasWhere)
                    sql += " and Policyid='" + policyId + "' ";
                else
                    sql += " where Policyid='" + policyId + "' ";
            }
            var data = DBHelper.GetDataTable(sql);
            return data;
        }
        private static SelectList getEmptySli(string name, string value)
        {
            SelectListItem sli = new SelectListItem();
            sli.Text = name;
            sli.Value = value;
            sli.Selected = true;
            var l = new List<SelectListItem>();
            l.Add(sli);
            return new SelectList(l, "Value", "Text");
        }
        private static List<SelectListItem> getEmptyList()
        {
            List<SelectListItem> souce = new List<SelectListItem>();
            var sli = new SelectListItem();
            sli.Text = "ALL"; sli.Value = "0"; sli.Selected = true;
            souce.Add(sli);
            return souce;
        }
        public static SelectList GetComplain(ref string complianId)
        {
            var temp = complainDataContext.GetComplain();
            if (temp != null && temp.Count() > 0)
            {
                if (complianId == null || complianId.Trim() == "")
                    complianId = temp.First().id.ToString();
                return new SelectList(temp, "id", "name", complianId);
            }
            else
                return getEmptySli("无", "0");
        }
        public static SelectList GetComplainOrAll(ref string complainId)
        {
            var souce = getEmptyList();
            var temp = complainDataContext.GetComplain();
            if (temp != null && temp.Count() > 0)
            {
                foreach (var d in temp)
                {
                    var sli = new SelectListItem();
                    sli.Text = d.name;
                    sli.Value = d.id.ToString();
                    souce.Add(sli);
                }
            }
            if (complainId == null || complainId.Trim() == "")
                complainId = "0";
            return new SelectList(souce, "Value", "Text", complainId);
        }
    }
    public static class ExportGenaral
    {
        public static DataColumn getColumn(string name, Type tp, string title)
        {
            DataColumn dc = new DataColumn(name, tp);
            dc.Caption = title;
            return dc;
        }
        public static DataTable getTab()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(getColumn("OrderId", typeof(string), "订单号"));
            dt.Columns.Add(getColumn("WorkedType", typeof(string), "订单类型"));
            dt.Columns.Add(getColumn("Staute", typeof(string), "状态"));
            dt.Columns.Add(getColumn("ProductId", typeof(string), "产品ID"));
            dt.Columns.Add(getColumn("ProductName", typeof(string), "产品名称"));
            dt.Columns.Add(getColumn("DeptId", typeof(string), "部门ID"));
            dt.Columns.Add(getColumn("DeptName", typeof(string), "部门名称"));
            dt.Columns.Add(getColumn("OperDate", typeof(string), "订单生成日期"));
            return dt;
        }
    }
}