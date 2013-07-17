using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
using System.Data;
namespace PlocyFlow.DAL.Context
{
    public class user_roleDataContext
    {
        public static IEnumerable<v_user_role> GetUser_Role()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.v_user_role.ToList();
            }
        }
    }
}
