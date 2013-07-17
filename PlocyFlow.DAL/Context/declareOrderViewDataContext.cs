using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class declareOrderViewDataContext
    {
        public static IEnumerable<v_declare_order> GetDeclareOrderView()
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.v_declare_order.ToList();
            }
        }
        public static IEnumerable<v_declare_order> GetDeclareOrderGtasksView()
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.v_declare_order.Where(cd=>cd.IsDone==false).ToList();
            }
        }
    }
}
