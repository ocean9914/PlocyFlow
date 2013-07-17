using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.CommonUtility;
namespace PlocyFlow.DAL.Context
{
    public class declareOrderDataContext
    {
        public static IEnumerable<declareorder> GetDeclareOrder()
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.declareorder.ToList();
            }
        }
        public static declareorder GetSingleDeclareOrder(string orderId)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.declareorder.Where(cd=>cd.order_Id==orderId).FirstOrDefault();
            }
        }
        public static bool AddDeclareOrder(declareorder NewDeclare)
        {
            bool r = false;
            if (NewDeclare != null)
            {
                using (policy_FlowEntities ef = new policy_FlowEntities())
                {
                    var curD=ef.declareorder.Where(cd => cd.order_Id == NewDeclare.order_Id).FirstOrDefault();
                    if (curD == null)
                    {
                        ef.AddTodeclareorder(NewDeclare);
                        var NewFlow = new declareflow();
                        NewFlow.sn = BaseId.getDeclareFlowId();
                        NewFlow.order_Id = NewDeclare.order_Id;
                        NewFlow.oper_type = OperList.NewSave.Oper_Value;
                        NewFlow.oper_memo = NewDeclare.memo;
                        NewFlow.oper_date = DateTime.Now;
                        NewFlow.oper_user = NewDeclare.oper_user;
                        NewFlow.memo = "0";
                        ef.AddTodeclareflow(NewFlow);
                        ef.SaveChanges();
                        r = true;
                    }
                }
            }
            return r;
        }
        public static IEnumerable<v_declare_order> GetMyGtasks(string user_name,string deptId,string policyId,string productId,string startDate,string endDate)
        {
            List<v_declare_order> r = null;
            DataTable dt = getMyGtasksTab(user_name, deptId, policyId, productId,startDate,endDate);
            if (dt != null && dt.Rows.Count > 0)
            {
                r = new List<v_declare_order>();
                foreach (DataRow tr in dt.Rows)
                {
                    v_declare_order d = new v_declare_order();
                    d.order_Id = tr[0].ToString();
                    d.policyId = tr[1].ToString();
                    d.policyName = tr[2].ToString();
                    d.product_Id = tr[3].ToString();
                    d.ProductName = tr[4].ToString();
                    d.IsDone = tr[5].ToString() == "1" ? true : false;
                    d.dept_id = CommonUtility.CommonUtility.TryParseStringToInt32(tr[6].ToString());
                    d.dept_name = tr[7].ToString();
                    d.oper_user = tr[8].ToString();
                    d.oper_date = Convert.ToDateTime(tr[9]);
                    d.last_oper = tr[10].ToString();
                    d.last_memo = tr[11].ToString();
                    r.Add(d);
                }
            }
            return r;
        }
        public static IEnumerable<v_declare_order> GetMyGtasks(string orderId)
        {
            List<v_declare_order> r = null;
            DataTable dt = getMyGtasksTab(orderId);
            if (dt != null && dt.Rows.Count > 0)
            {
                r = new List<v_declare_order>();
                foreach (DataRow tr in dt.Rows)
                {
                    v_declare_order d = new v_declare_order();
                    d.order_Id = tr[0].ToString();
                    d.policyId = tr[1].ToString();
                    d.policyName = tr[2].ToString();
                    d.product_Id = tr[3].ToString();
                    d.ProductName = tr[4].ToString();
                    d.IsDone = tr[5].ToString() == "1" ? true : false;
                    d.dept_id = CommonUtility.CommonUtility.TryParseStringToInt32(tr[6].ToString());
                    d.dept_name = tr[7].ToString();
                    d.oper_user = tr[8].ToString();
                    d.oper_date = Convert.ToDateTime(tr[9]);
                    d.last_oper = tr[10].ToString();
                    d.last_memo = tr[11].ToString();
                    r.Add(d);
                }
            }
            return r;
        }
        public static DataTable getMyGtasksTab(string orderId)
        {
            DataTable dt = null;
            string sql = "select o.order_Id,o.policyId ,p.p_name ,o.product_Id ,b.ProductName,"
                       + " b.IsDone ,b.Departmentid ,d.d_name ,o.oper_user,o.oper_date,o.last_oper,o.memo1 AS last_memo "
                       + " from declareorder o,policy p,bip_data b,dept d "
                       + " where o.staute=0 and o.product_Id = b.Productid and o.policyId = b.Policyid "
                       + " and b.Policyid = p.p_id and b.Departmentid = d.d_id "
                       + " and o.order_Id= '" + orderId + "'";
            
            dt = DBHelper.GetDataTable(sql);
            return dt;
        }
        public static DataTable getMyGtasksTab(string user_name, string deptId, string policyId, string productId, string startDate, string endDate)
        {
            DataTable dt = null;
            string sql = "select o.order_Id,o.policyId ,p.p_name ,o.product_Id ,b.ProductName,"
                       +" b.IsDone ,b.Departmentid ,d.d_name ,o.oper_user,o.oper_date,o.last_oper,o.memo1 AS last_memo "
                       +" from declareorder o,policy p,bip_data b,dept d "
                       +" where o.staute=0 and o.product_Id = b.Productid and o.policyId = b.Policyid "
                       +" and b.Policyid = p.p_id and b.Departmentid = d.d_id "
                       +" and o.next_oper like '%"+user_name+"%'";
            if (deptId != null && deptId!="" && deptId != "0")
                sql += " and b.Departmentid=" + deptId;
            if (policyId != null && policyId != "" && policyId != "0")
                sql += " and o.policyId='" + policyId + "' ";
            if (productId != null && productId != "" && productId != "0")
                sql += " and o.product_Id='" + productId + "' ";
            DateTime startD, endD;
            if (DateTime.TryParse(startDate, out startD) && DateTime.TryParse(endDate, out endD))
            {
                sql += " and date_format(o.oper_date,'%Y-%m-%d') between  '" + startD.ToString("yyyy-MM-dd") + "' and '" + endD.ToString("yyyy-MM-dd") + "' ";
            }
            dt = DBHelper.GetDataTable(sql);
            return dt;
        }
        public static IEnumerable<declareorder>GetAllMyGtasks()
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.declareorder.Where(cd=>!cd.staute).ToList();
            }
        }
        /// <summary>
        /// 撤销申报订单，如果订单已产生工作流水,撤销失败
        /// </summary>
        /// <param name="order_id">申报订单号</param>
        /// <returns></returns>
        public static bool DeleteDeclareOrder(string order_id)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
               
            }
            return r;
        }
    }
}
