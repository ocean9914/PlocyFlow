using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class approve_typeDataContext
    {
        public static IEnumerable<approve_type> GetApprove_Type()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.approve_type.ToList();
            }
        }
        public static bool AddApprove(string approveName)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                int ct = pfentity.approve_type.Where(ca => ca.name == approveName).Count();
                if(ct==0)
                {
                    approve_type a = new approve_type();
                    a.name = approveName;
                    pfentity.AddToapprove_type(a);
                    pfentity.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
        public static bool DelApprove(int id)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                approve_type a= pfentity.approve_type.Where(ca=>ca.id==id).First();
                pfentity.DeleteObject(a);
                pfentity.SaveChanges();
                r = true;
            }
            return r;
        }
    }
}
