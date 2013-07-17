using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class policyDataContext
    {
        public static IEnumerable<policy> GetPolicy()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.policy.ToList();
            }
        }
        public static string GetPolicyName(string policyId)
        {
            string r = "";
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                policy p = ef.policy.Where(cp => cp.p_id == policyId).First();
                if(p!=null)
                r = p.p_name;
            }
            return r;
        }
        public static bool AddPolicy(string policyid,string policyname)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                int ct = ef.policy.Where(cp => cp.p_id == policyid && cp.p_name == policyname).Count();
                if (ct == 0)
                {
                    policy p = new policy();
                    p.p_id = policyid;
                    p.p_name = policyname;
                    ef.AddTopolicy(p);
                    ef.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
        public static bool DelPolicy(int id)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                policy p = ef.policy.Where(cp => cp.id == id).First();
                ef.DeleteObject(p);
                ef.SaveChanges();
                r = true;
            }
            return r;
        }
        public static bool updatePolicy(string policyid, string policyname)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                var p = ef.policy.Where(cp => cp.p_id == policyid).FirstOrDefault();
                if (p == null)
                {
                    policy np = new policy();
                    np.p_id = policyid;
                    np.p_name = policyname;
                    ef.AddTopolicy(np);
                    ef.SaveChanges();
                    r = true;
                }
                else
                {
                    p.p_name = policyname;
                    ef.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
    }
}
