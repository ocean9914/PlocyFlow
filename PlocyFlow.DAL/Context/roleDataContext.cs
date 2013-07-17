using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class roleDataContext
    {
        public static IEnumerable<role> GetRole()
        {
            using (policy_FlowEntities pfentiry = new policy_FlowEntities())
            {
                return pfentiry.role.ToList();
            }
        }
        public static role GetRole(int roleId)
        {
            using (policy_FlowEntities pfentiry = new policy_FlowEntities())
            {
                return pfentiry.role.Where(cr=>cr.id==roleId).First();
            }
        }
        public static bool AddRole(string rolename)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                int ct = ef.role.Where(cr => cr.name == rolename).Count();
                if (ct == 0)
                {
                    role rl = new role();
                    rl.name = rolename;
                    rl.vcode = "0x1";
                    ef.AddTorole(rl);
                    ef.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
        public static bool DelRole(int id)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                role rl = ef.role.Where(cr => cr.id == id).First();
                ef.DeleteObject(rl);
                ef.SaveChanges();
                r = true;
            }
            return r;
        }
    }
}
