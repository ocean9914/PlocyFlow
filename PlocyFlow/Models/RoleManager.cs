using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.Context;
using PlocyFlow.DAL.CommonUtility;
namespace PlocyFlow.Models
{
    public static class RoleManager
    {
        public static bool setRole(int roleID, int[] pageIdList)
        {
            bool r = false;
            if (pageIdList != null && pageIdList.Length > 0)
            {
                using (policy_FlowEntities pfentiry = new policy_FlowEntities())
                {
                    string vcode = "0x1";
                    List<string> pvlist = new List<string>();
                    foreach (int pid in pageIdList)
                    {
                        t_page p = pfentiry.t_page.Where(cp => cp.id == pid).First();
                        if (p != null)
                        {
                            pvlist.Add(p.vcode);
                        }
                    }
                    if (pvlist.Count > 0)
                    {
                        foreach (string pv in pvlist)
                        {
                            vcode = getVCode(vcode, pv);
                        }
                    }
                    role role = pfentiry.role.Where(cr => cr.id == roleID).First();
                    if (role != null)
                    {
                        role.vcode = vcode;
                        var userList = pfentiry.t_user.Where(cu => cu.role_id == roleID).ToList();
                        if (userList != null)
                        {
                            foreach (var u in userList)
                            {
                                u.vcode = vcode;
                            }
                        }
                        pfentiry.SaveChanges();
                        r = true;
                    }
                }
            }
            return r;
        }
        public static bool removeAllRole(int roleID)
        {
            bool r = false;
            using (policy_FlowEntities pfentiry = new policy_FlowEntities())
            {
                role role = pfentiry.role.Where(cr => cr.id == roleID).First();
                if (role != null)
                {
                    role.vcode = "0x0";
                    var userList = pfentiry.t_user.Where(cu => cu.role_id == roleID).ToList();
                    if (userList != null)
                    {
                        foreach (var u in userList)
                        {
                            u.vcode = "0x0";
                        }
                    }
                    pfentiry.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
        private static string getVCode(string curV, string pageV)
        {
            ulong r = Convert.ToUInt64(curV, 16) | Convert.ToUInt64(pageV, 16);
            return "0x" + CommonUtility.DtoX(r);
        }
        private static string getDelCode(string curV, string pageV)
        {
            UInt64 r = Convert.ToUInt64(curV, 16) ^ Convert.ToUInt64(pageV, 16);
            return "0x" + CommonUtility.DtoX(r);
        }

    }
}