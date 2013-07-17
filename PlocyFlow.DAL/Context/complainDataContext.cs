using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class complainDataContext
    {
        public static IEnumerable<complain> GetComplain()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.complain.ToList();
            }
        }
        public static complain GetComplain(int complainId)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.complain.Where(cc=>cc.id==complainId).FirstOrDefault();
            }
        }
        public static bool AddComplain(string complainName)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                int ct = ef.complain.Where(cc => cc.name == complainName).Count();
                if (ct == 0)
                {
                    complain c = new complain();
                    c.name = complainName;
                    ef.AddTocomplain(c);
                    ef.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
        public static bool DelComplain(int id)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                complain c = ef.complain.Where(cc => cc.id == id).First();
                ef.DeleteObject(c);
                ef.SaveChanges();
                r = true;
            }
            return r;
        }
    }
}
