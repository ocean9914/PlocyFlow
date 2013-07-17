using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class bip_view
    {
        public static IEnumerable<v_bipdata> GetBipData()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.v_bipdata.ToList();
            }
        }
        public static IEnumerable<v_bipdata> GetBipData(string policyId, int deptId, int stauteId)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.v_bipdata.Where(cb =>  cb.Policyid == policyId && cb.Departmentid == deptId && cb.Statusid == stauteId && cb.IsDone==false).ToList();
            }
        }
        public static IEnumerable<v_bipdata> GetBipDataByName(string pname)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.v_bipdata.Where(cp=>cp.ProductName.Contains(pname)).OrderBy(cp=>cp.Productid).ToList();
            }
        }
        public static IEnumerable<v_bipdata> GetBipData(string policyId, int deptId)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.v_bipdata.Where(cb => cb.Policyid == policyId && cb.Departmentid == deptId && cb.IsDone == false).ToList();
            }
        }
        public static v_bipdata GetBipSingle(string productId)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.v_bipdata.Where(cb => cb.Productid == productId).FirstOrDefault();
            }
        }
        public static v_bipdata GetBipSingle(string productId,string policyId)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.v_bipdata.Where(cb=>cb.Productid==productId && cb.Policyid==policyId).FirstOrDefault();
            }
        }
        public static v_bipdata GetBipSingle(string productId, string policyId,int deptId,int stauteId)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.v_bipdata.Where(cb => cb.Productid == productId && cb.Policyid == policyId && cb.Departmentid==deptId && cb.Statusid==stauteId).FirstOrDefault();
            }
        }
        
    }
}
