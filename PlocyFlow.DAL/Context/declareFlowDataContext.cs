using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;

namespace PlocyFlow.DAL.Context
{
    public class declareFlowDataContext
    {
        public static IEnumerable<declareflow> GetDeclareFlow()
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.declareflow.ToList();
            }
        }
        public static declareflow GetSingleDeclareFlow(string sn)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.declareflow.Where(cd => cd.sn == sn).FirstOrDefault();
            }
        }
        /// <summary>
        /// 判断申报订单是否产生了流水处理
        /// </summary>
        /// <param name="orderId">申报订单号</param>
        /// <returns></returns>
        public static bool HasDeclareFlow(string orderId)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                var ct= ef.declareflow.Where(cd => cd.order_Id == orderId).Count();
                if (ct==0)
                    return false;
                else
                    return true;
            }
        }
        public static IEnumerable<declareflow> GetDeclareFlowByOrder(string order_id)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.declareflow.Where(cd => cd.order_Id == order_id).OrderBy(o=>o.oper_date).ToList();
            }
        }
        public static IEnumerable<declareflow> GetDeclareFlowByOrderDesc(string order_id)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.declareflow.Where(cd => cd.order_Id == order_id).OrderByDescending(o => o.oper_date).ToList();
            }
        }
    }
}
