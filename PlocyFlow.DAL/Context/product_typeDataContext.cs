using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class product_typeDataContext
    {
        public static IEnumerable<product_type> GetProduct_type()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.product_type.ToList();
            }
        }
        public static bool updateProduct_type(string Id, string Name)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                var p = pfentity.product_type.Where(cp => cp.p_id == Id).FirstOrDefault();
                if (p == null)
                {
                    product_type np = new product_type();
                    np.p_id = Id;
                    np.p_name = Name;
                    pfentity.AddToproduct_type(np);
                    pfentity.SaveChanges();
                    r = true;
                }
                else
                {
                    p.p_name = Name;
                    pfentity.SaveChanges();
                    r = true;
                }
            }
            return r;
        }

    }
}
