using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using PlocyFlow.DAL.CommonUtility;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.Context;
namespace PlocyFlow.Models
{
    public static class MyWorkedList
    {
        public static List<Worked> GetAllNClosed(string deptId, string productId, string startD, string endD)
        {
            List<Worked> r = new List<Worked>();
            var temp = DeclareWorked.GetAllNClosed(deptId, productId, startD, endD);
            if (temp != null)
            {
                foreach (Worked w in temp)
                {
                    r.Add(w);
                }
            }
            temp = ComplainWorked.GetAllNClosed(deptId, productId, startD, endD);
            if (temp != null)
            {
                foreach (Worked w in temp)
                {
                    r.Add(w);
                }
            }
            return r;
        }
        public static List<Worked> GetAllWorked(string deptId, string productId, string startD, string endD)
        {
            return GetMyWorked(deptId, productId, startD, endD, "");
        }
        public static List<Worked> GetMyWorked(string deptId, string productId, string startD, string endD, string user_name)
        {
            List<Worked> r = new List<Worked>();
            var temp = DeclareWorked.GetMyWorked(deptId, productId, startD, endD, user_name);
            if (temp != null)
            {
                foreach (Worked w in temp)
                {
                    r.Add(w);
                }
            }
            temp = ComplainWorked.GetMyWorked(deptId, productId, startD, endD, user_name);
            if (temp != null)
            {
                foreach (Worked w in temp)
                {
                    r.Add(w);
                }
            }
            return r;
        }
    }
    public static class DeclareWorked
    {
        public static List<Worked>GetAllNClosed(string deptId, string productId, string startD, string endD)
        {
            List<Worked> r = null;
            DataTable dt = getSouceTab(deptId, productId, startD, endD,"", 0);
            if (dt != null && dt.Rows.Count > 0)
            {
                r = new List<Worked>();
                foreach (DataRow tr in dt.Rows)
                {
                    Worked nw = new Worked();
                    nw.OrderId = tr[0].ToString();
                    nw.WorkedType = "申报" + tr[4].ToString();
                    nw.Staute = CommonUtility.TryParseStringToInt32(tr[5].ToString()) > 0 ? "已关闭" : "未关闭";
                    nw.ProductId = tr[1].ToString();
                    nw.ProductName = tr[2].ToString();
                    nw.DeptId = tr[6].ToString();
                    nw.DeptName = tr[7].ToString();
                    nw.OperDate = Convert.ToDateTime(tr[8]).ToString("yyyy-MM-dd");
                    nw.NextOper = tr[9].ToString();
                    nw.OperUser = tr[10].ToString();
                    r.Add(nw);
                }
            }
            return r;
        }
        public static List<Worked> GetAllWorked(string deptId, string productId, string startD, string endD)
        {
            return GetMyWorked(deptId, productId, startD, endD, "");
        }
        public static List<Worked> GetMyWorked(string deptId, string productId, string startD, string endD, string user_name)
        {
            List<Worked> r = null;
            DataTable dt = getSouceTab(deptId, productId, startD, endD, user_name,-1);
            if (dt != null && dt.Rows.Count > 0)
            {
                r = new List<Worked>();
                foreach (DataRow tr in dt.Rows)
                {
                    Worked nw = new Worked();
                    nw.OrderId = tr[0].ToString();
                    nw.WorkedType = "申报" + tr[4].ToString();
                    nw.Staute = CommonUtility.TryParseStringToInt32(tr[5].ToString()) > 0 ? "已关闭" : "未关闭";
                    nw.ProductId = tr[1].ToString();
                    nw.ProductName = tr[2].ToString();
                    nw.DeptId = tr[6].ToString();
                    nw.DeptName = tr[7].ToString();
                    nw.OperDate = Convert.ToDateTime(tr[8]).ToString("yyyy-MM-dd");
                    nw.NextOper = tr[9].ToString();
                    r.Add(nw);
                }
            }
            return r;
        }
        /// <summary>
        /// 取得申报源始数据
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="productId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="user_name"></param>
        /// <param name="stauteFlag">-1所有状态；0未关闭；1已关闭</param>
        /// <returns></returns>
        private static DataTable getSouceTab(string deptId, string productId, string startDate, string endDate, string user_name,int stauteFlag)
        {
            DataTable dt = null;
            string sql = "select o.order_Id,o.product_Id,b.ProductName, o.policyId,p.p_name as policyName,o.staute,b.Departmentid,d.d_name,o.oper_date,o.next_oper,o.oper_user "
                      + " from declareorder o,bip_data b, dept d,policy p "
                      + " where  o.product_Id=b.Productid and o.policyId=b.Policyid "
                      + " and b.Departmentid=d.d_id and b.Policyid=p.p_id ";
            if (user_name != null && user_name.Trim() != "")
                sql += " and o.order_Id in (select distinct order_Id from declareflow where oper_user='" + user_name + "') ";
            if (deptId != null && deptId.Trim() != "" && deptId != "0")
                sql += " and b.Departmentid=" + deptId;
            if (productId != null && productId.Trim() != "" && productId != "0")
                sql += " and o.product_Id='" + productId + "' ";
            switch(stauteFlag)
            {
                case 1:
                    sql += " and o.staute=1 ";
                    break;
                case 0:
                    sql += " and o.staute=0 ";
                    break;
            }
            DateTime startD, endD;
            if (DateTime.TryParse(startDate, out startD) && DateTime.TryParse(endDate, out endD))
            {
                sql += " and date_format(o.oper_date,'%Y-%m-%d') between  '" + startD.ToString("yyyy-MM-dd") + "' and '" + endD.ToString("yyyy-MM-dd") + "' ";
            }
            dt = DBHelper.GetDataTable(sql);
            return dt;
        }
    }
    public static class ComplainWorked
    {
        public static List<Worked> GetAllNClosed(string deptId, string productId, string startD, string endD)
        {
            List<Worked> r = null;
            DataTable dt = getSouceTab(deptId, productId, startD, endD, "", 0);
            if (dt != null && dt.Rows.Count > 0)
            {
                r = new List<Worked>();
                foreach (DataRow tr in dt.Rows)
                {
                    Worked nw = new Worked();
                    nw.OrderId = tr[0].ToString();
                    nw.WorkedType = "申报" + tr[4].ToString();
                    nw.Staute = CommonUtility.TryParseStringToInt32(tr[5].ToString()) > 0 ? "已关闭" : "未关闭";
                    nw.ProductId = tr[1].ToString();
                    nw.ProductName = tr[2].ToString();
                    nw.DeptId = tr[6].ToString();
                    nw.DeptName = tr[7].ToString();
                    nw.OperDate = Convert.ToDateTime(tr[8]).ToString("yyyy-MM-dd");
                    nw.NextOper = tr[9].ToString();
                    nw.OperUser = tr[10].ToString();
                    r.Add(nw);
                }
            }
            return r;
        }
        public static List<Worked> GetAllWorked(string deptId, string productId, string startD, string endD)
        {
            return GetMyWorked(deptId, productId, startD, endD, "");
        }
        public static List<Worked> GetMyWorked(string deptId, string productId, string startD, string endD, string user_name)
        {
            List<Worked> r = null;
            DataTable dt = getSouceTab(deptId, productId, startD, endD, user_name,-1);
            if (dt != null && dt.Rows.Count > 0)
            {
                r = new List<Worked>();
                foreach (DataRow tr in dt.Rows)
                {
                    Worked nw = new Worked();
                    nw.OrderId = tr[0].ToString();
                    nw.WorkedType = "投诉" + tr[4].ToString();
                    nw.Staute = CommonUtility.TryParseStringToInt32(tr[5].ToString()) > 0 ? "已关闭" : "未关闭";
                    nw.ProductId = tr[1].ToString();
                    nw.ProductName = tr[2].ToString();
                    nw.DeptId = tr[6].ToString();
                    nw.DeptName = tr[7].ToString();
                    nw.OperDate = Convert.ToDateTime(tr[8]).ToString("yyyy-MM-dd");
                    nw.NextOper = tr[9].ToString();
                    r.Add(nw);
                }
            }
            return r;
        }
        private static DataTable getSouceTab(string deptId, string productId, string startDate, string endDate, string user_name, int stauteFlag)
        {
            DataTable dt = null;
            string sql = "select distinct o.order_Id,o.productId,o.productName,o.complainId,o.complainName ,o.staute,b.Departmentid,d.d_name,o.oper_date,o.next_oper,o.oper_user "
                      + " from complainorder o,bip_data b, dept d"
                      + " where  o.productId=b.Productid and b.Departmentid=d.d_id ";
            if (user_name != null && user_name.Trim() != "")
                sql += " and o.order_Id in (select distinct order_Id from complainflow where oper_user='" + user_name + "') ";
            if (deptId != null && deptId.Trim() != "" && deptId != "0")
                sql += " and b.Departmentid=" + deptId;
            if (productId != null && productId.Trim() != "" && productId != "0")
                sql += " and o.product_Id='" + productId + "' ";
            switch (stauteFlag)
            {
                case 1:
                    sql += " and o.staute=1 ";
                    break;
                case 0:
                    sql += " and o.staute=0 ";
                    break;
            }
            DateTime startD, endD;
            if (DateTime.TryParse(startDate, out startD) && DateTime.TryParse(endDate, out endD))
            {
                sql += " and date_format(o.oper_date,'%Y-%m-%d') between  '" + startD.ToString("yyyy-MM-dd") + "' and '" + endD.ToString("yyyy-MM-dd") + "' ";
            }
            dt = DBHelper.GetDataTable(sql);
            return dt;
        }
    }
}