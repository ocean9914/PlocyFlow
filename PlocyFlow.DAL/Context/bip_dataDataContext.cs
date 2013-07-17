using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class bip_dataDataContext
    {
        public static IEnumerable<bip_data> GetBip_data()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.bip_data.ToList();
            }
        }
        public static bool updateBip_data(IEnumerable<bip_data> data)
        {
            bool r = false;
            if (data != null && data.Count() > 0)
            {
                using (policy_FlowEntities pfentity = new policy_FlowEntities())
                {
                    foreach (var b in data)
                    {
                        var bip = pfentity.bip_data.Where(cb => cb.Productid == b.Productid && cb.Policyid == b.Policyid).FirstOrDefault();
                        if (bip == null)
                        {
                            pfentity.AddTobip_data(b);
                        }
                        else
                        {
                            bip.ProductName = b.ProductName;
                            bip.Departmentid = b.Departmentid;
                            bip.ProductTypeid = b.ProductTypeid;
                            bip.OBDate = b.OBDate;
                            bip.GameTypeid = b.GameTypeid;
                            bip.Statusid = b.Statusid;
                            bip.Manager = b.Manager;
                            bip.IsDone = b.IsDone;
                            bip.Remark = b.Remark;
                        }
                    }
                    pfentity.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
        public static IEnumerable<bip_data> GetBip_data(string policyId, string deptId)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                if ((policyId == null || policyId == "0") && (deptId == null || deptId == "0"))
                    return pfentity.bip_data.ToList();
                else
                    if (policyId == null || policyId == "0")
                    {
                        int did = 0;
                        int.TryParse(deptId, out did);
                        return pfentity.bip_data.Where(cb => cb.Departmentid == did).ToList();
                    }
                    else
                        return pfentity.bip_data.Where(cb => cb.Policyid == policyId).ToList();
            }
        }
        public static IEnumerable<bip_data> GetBip_data(string deptId)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                if (deptId == null || deptId == "0")
                    return pfentity.bip_data.ToList();
                else
                {
                    int did = 0;
                    int.TryParse(deptId, out did);
                    return pfentity.bip_data.Where(cb => cb.Departmentid == did).ToList();
                }
            }
        }
        public static string GetProductName(string productId)
        {
            string r = "";
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                var p = pfentity.bip_data.Where(cb => cb.Productid == productId).FirstOrDefault();
                if (p != null)
                    r = p.ProductName;
            }
            return r;
        }
    }
}
