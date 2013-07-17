using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class complainFlowDataContext
    {
        public static IEnumerable<complainflow> GetComplainFlow()
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.complainflow.ToList();
            }
        }
        public static complainflow GetSingleComplainFlow(string sn)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.complainflow.Where(cd=>cd.sn==sn).FirstOrDefault();
            }
        }
        public static bool HasComplainFlow(string orderId)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {

                var ct = ef.complainflow.Where(cd => cd.orderId == orderId).Count();
                if (ct == 0)
                    return false;
                else
                    return true;
            }
        }
        public static IEnumerable<complainflow>GetComplainFlowByOrder(string orderId)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.complainflow.Where(cd => cd.orderId == orderId).OrderBy(c => c.oper_date).ToList();
            }
        }
        public static IEnumerable<complainflow> GetComplainFlowByOrderDesc(string orderId)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.complainflow.Where(cd => cd.orderId == orderId).OrderByDescending(c => c.oper_date).ToList();
            }
        }
    }
}
