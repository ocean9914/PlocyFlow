using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.CommonUtility;
namespace PlocyFlow.DAL.Context
{
   public  class complianOrderDataContext
    {
       public static IEnumerable<complainorder> GetComplianOrder()
       {
           using (policy_FlowEntities ef = new policy_FlowEntities())
           {
               return ef.complainorder.ToList();
           }
       }
       public static complainorder GetSingleComplainOrder(string orderId)
       {
           using (policy_FlowEntities ef = new policy_FlowEntities())
           {
               return ef.complainorder.Where(cd => cd.order_Id == orderId).FirstOrDefault();
           }
       }
       public static bool AddComplianOrder(complainorder NewComp)
       {
           bool r = false;
           if (NewComp != null)
           {
               using (policy_FlowEntities ef = new policy_FlowEntities())
               {
                   var curD=ef.declareorder.Where(cd=>cd.order_Id==NewComp.order_Id).FirstOrDefault();
                   if (curD == null)
                   {
                       ef.AddTocomplainorder(NewComp);
                       var NewFlow = new complainflow();
                       NewFlow.sn = BaseId.getCompFlowId();
                       NewFlow.orderId = NewComp.order_Id;
                       NewFlow.complainId = NewComp.complainId.ToString();
                       NewFlow.oper_type = OperList.NewSave.Oper_Value;
                       NewFlow.oper_memo = NewComp.memo;
                       NewFlow.oper_date = DateTime.Now;
                       NewFlow.oper_user = NewComp.oper_user;
                       NewFlow.memo = "0";
                       NewFlow.pm = NewComp.next_oper;
                       NewFlow.attach_url = NewComp.attach_url;
                       ef.AddTocomplainflow(NewFlow);
                       ef.SaveChanges();
                       r = true;
                   }
               }
           }
           return r;
       }
       public static IEnumerable<v_complain_order> GetMyCtasks(string user_name,string deptId,string productId,string complainId, string startDate,string endDate)
       {
           List<v_complain_order> r = null;
           DataTable dt = getMyGtasksTab(user_name, deptId, productId,complainId, startDate, endDate);
           if (dt != null && dt.Rows.Count > 0)
           {
               r = new List<v_complain_order>();
               foreach (DataRow tr in dt.Rows)
               {
                   v_complain_order c = new v_complain_order();
                   c.order_Id = tr[0].ToString();
                   c.productId = tr[1].ToString();
                   c.ProductName = tr[2].ToString();
                   c.Departmentid =CommonUtility.CommonUtility.TryParseStringToInt32( tr[3].ToString());
                   c.Departmentname = tr[4].ToString();
                   c.complainId = CommonUtility.CommonUtility.TryParseStringToInt32(tr[5].ToString());
                   c.complainName = tr[6].ToString();
                   c.oper_date = Convert.ToDateTime(tr[7]);
                   c.oper_user = tr[8].ToString();
                   c.endate = Convert.ToDateTime(tr[9]);
                   c.last_oper = tr[10].ToString();
                   c.last_memo = tr[11].ToString();
                   r.Add(c);
               }
           }
           return r;
       }
       public static IEnumerable<v_complain_order> GetMyCtasks(string orderId)
       {
           List<v_complain_order> r = null;
           DataTable dt = getMyGtasksTab(orderId);
           if (dt != null && dt.Rows.Count > 0)
           {
               r = new List<v_complain_order>();
               foreach (DataRow tr in dt.Rows)
               {
                   v_complain_order c = new v_complain_order();
                   c.order_Id = tr[0].ToString();
                   c.productId = tr[1].ToString();
                   c.ProductName = tr[2].ToString();
                   c.Departmentid = CommonUtility.CommonUtility.TryParseStringToInt32(tr[3].ToString());
                   c.Departmentname = tr[4].ToString();
                   c.complainId = CommonUtility.CommonUtility.TryParseStringToInt32(tr[5].ToString());
                   c.complainName = tr[6].ToString();
                   c.oper_date = Convert.ToDateTime(tr[7]);
                   c.oper_user = tr[8].ToString();
                   c.endate = Convert.ToDateTime(tr[9]);
                   c.last_oper = tr[10].ToString();
                   c.last_memo = tr[11].ToString();
                   r.Add(c);
               }
           }
           return r;
       }
       public static DataTable getMyGtasksTab(string user_name,string deptId,string productId,string complainId, string startDate,string endDate)
       {
           DataTable dt = null;
           string sql = "select distinct o.order_Id,o.productId,b.ProductName,b.Departmentid,d.d_name as Departmentname,o.complainId,o.complainName,o.oper_date,o.oper_user,o.endate,o.last_oper,o.last_memo "
                      +" from complainorder o,bip_data b,dept d "
                      +" where o.productId=b.Productid  and b.Departmentid=d.d_id "
                       + " and o.next_oper like '%" + user_name + "%'";
           if (deptId != null && deptId != "" && deptId != "0")
               sql += " and b.Departmentid=" + deptId;
           if (productId != null && productId != "" && productId != "0")
               sql += " and o.productId='" + productId + "' ";
           if (complainId != null && complainId != "" && complainId != "0")
               sql += " and o.complainId=" + complainId;
           DateTime startD, endD;
           if (DateTime.TryParse(startDate, out startD) && DateTime.TryParse(endDate, out endD))
           {
               sql += " and date_format(o.oper_date,'%Y-%m-%d') between  '" + startD.ToString("yyyy-MM-dd") + "' and '" + endD.ToString("yyyy-MM-dd") + "' ";
           }
           dt = DBHelper.GetDataTable(sql);
           return dt;
       }
       public static DataTable getMyGtasksTab(string orderId)
       {
           DataTable dt = null;
           string sql = "select distinct o.order_Id,o.productId,b.ProductName,b.Departmentid,d.d_name as Departmentname,o.complainId,o.complainName,o.oper_date,o.oper_user,o.endate,o.last_oper,o.last_memo "
                      + " from complainorder o,bip_data b,dept d "
                      + " where o.productId=b.Productid  and b.Departmentid=d.d_id "
                       + " and o.order_Id ='" + orderId + "'";
           dt = DBHelper.GetDataTable(sql);
           return dt;
       }
    }
}
