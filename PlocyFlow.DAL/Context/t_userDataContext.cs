using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class t_userDataContext
    {
        public static IEnumerable<t_user> GetT_User()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.t_user.ToList();
            }
        }
        public static t_user Get_User(string _user_name)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.t_user.Where(cuser => cuser.user_name == _user_name).FirstOrDefault();
            }
        }
        public static t_user Get_User(int user_id)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.t_user.Where(cuser => cuser.user_id == user_id).FirstOrDefault();
            }
        }
        public static IEnumerable<t_user>Get_UserByRoleID(int roleID)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.t_user.Where(cu=>cu.role_id==roleID).ToList();
            }
        }
        public static bool AddUser(string user_name,int uid,int role_id)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                int ct = ef.t_user.Where(cu => cu.user_id == uid && cu.user_name == user_name).Count();
                if (ct == 0)
                {

                    role rl = roleDataContext.GetRole(role_id);
                    string vcode = CommonUtility.CommonUtility.defaultCode;
                    if (rl.vcode != "")
                        vcode = rl.vcode;
                    t_user newU = new t_user();
                    newU.user_id = uid;
                    newU.user_name = user_name;
                    newU.vcode = vcode;
                    newU.role_id = role_id;
                    ef.AddTot_user(newU);
                    ef.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
        public static bool DelUser(int id)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                t_user u = pfentity.t_user.Where(cd => cd.id == id).FirstOrDefault();
                pfentity.DeleteObject(u);
                pfentity.SaveChanges();
                r = true;
            }
            return r;
        }
        public static bool UpdateUser(int uid,int role_id)
        {
            bool r = false;
            using (policy_FlowEntities ef = new policy_FlowEntities())
            {
                var nu = ef.t_user.Where(cu =>cu.id == uid ).FirstOrDefault();
                if (nu!=null)
                {
                    role rl = roleDataContext.GetRole(role_id);
                    string vcode = CommonUtility.CommonUtility.defaultCode;
                    if (rl.vcode != "")
                        vcode = rl.vcode;
                    nu.vcode = vcode;
                    nu.role_id = role_id;
                    ef.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
    }
}
