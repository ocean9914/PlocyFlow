using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class apr_user_listDataContext
    {
        public static IEnumerable<v_approve_userlist> GetApprove_userList()
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.v_approve_userlist.ToList();
            }
        }
        public static IEnumerable<v_approve_userlist> GetFirstApprove(string policyId)
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.v_approve_userlist.Where(ca => ca.p_id == policyId && ca.name == "初审").ToList();
            }
        }
        //public static string GetFirstApproves()
        //{
        //    string r = "";
        //    using (policy_FlowEntities ef = new policy_FlowEntities())
        //    {
        //        var cu= ef.v_approve_userlist.Where(ca => ca.name == "初审").ToList();
        //        if (cu != null)
        //        {
        //            StringBuilder sbd = new StringBuilder();
        //            foreach (var u in cu)
        //            {
        //                sbd.Append(u.user_name + ",");
        //            }
        //            r = sbd.ToString();
        //            if (r != "")
        //                r = r.Substring(0, r.Length - 1);
        //        }
        //    }
        //    return r;
        //}
        public static string GetFirstApproves(string policyId)
        {
            string r = "";
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                var cu = ef.v_approve_userlist.Where(ca => ca.p_id==policyId && ca.name == "初审").ToList();
                if (cu != null)
                {
                    StringBuilder sbd = new StringBuilder();
                    foreach (var u in cu)
                    {
                        sbd.Append(u.user_name + ",");
                    }
                    r = sbd.ToString();
                    if (r != "")
                        r = r.Substring(0, r.Length - 1);
                }
            }
            return r;
        }
        public static IEnumerable<v_approve_userlist> GetLastApprove()
        {
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                return ef.v_approve_userlist.Where(ca => ca.name == "终审").ToList();
            }
        }
        //public static string GetLastApproves()
        //{
        //    string r = "";
        //    using (policy_FlowEntities ef = new policy_FlowEntities())
        //    {
        //        var cu= ef.v_approve_userlist.Where(ca => ca.name == "终审").ToList();
        //        if (cu != null)
        //        {
        //            StringBuilder sbd = new StringBuilder();
        //            foreach (var u in cu)
        //            {
        //                sbd.Append(u.user_name + ",");
        //            }
        //            r = sbd.ToString();
        //            if (r != "")
        //                r = r.Substring(0, r.Length - 1);
        //        }
        //    }
        //    return r;
        //}
        public static string GetLastApproves( string policyId)
        {
            string r = "";
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                var cu = ef.v_approve_userlist.Where(ca => ca.p_id == policyId && ca.name == "终审").ToList();
                if (cu != null)
                {
                    StringBuilder sbd = new StringBuilder();
                    foreach (var u in cu)
                    {
                        sbd.Append(u.user_name + ",");
                    }
                    r = sbd.ToString();
                    if (r != "")
                        r = r.Substring(0, r.Length - 1);
                }
            }
            return r;
        }
    }
}
