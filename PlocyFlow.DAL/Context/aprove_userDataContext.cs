using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class aprove_userDataContext
    {
        public static IEnumerable<aprove_user> GetAprove_user()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.aprove_user.ToList();
            }
        }
        public static bool updateApprove_User(int pid,int aid,int[] uidList)
        {
            bool r = false;
            using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
            {
                try
                {
                    List<MySqlParameter> paras = new List<MySqlParameter>();
                    MySqlParameter para = new MySqlParameter("pid", MySqlDbType.Int32);
                    para.Value = pid;
                    paras.Add(para);
                    para = new MySqlParameter("aid", MySqlDbType.Int32);
                    para.Value = aid;
                    paras.Add(para);
                    tran.ExcuteSQL("delete from aprove_user where pid=?pid and aid=?aid", paras.ToArray());
                    para = new MySqlParameter("uid", MySqlDbType.Int32);
                    paras.Add(para);
                    foreach (int uid in uidList)
                    {
                        paras[2].Value = uid;
                        tran.ExcuteSQL("insert into aprove_user(pid,aid,uid) values(?pid,?aid,?uid)", paras.ToArray());
                    }
                    tran.Commit();
                    r = true;
                }
                catch
                {
                    tran.Roback();
                }
            }
            return r;
        }
    }
}
