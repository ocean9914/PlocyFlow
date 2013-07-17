using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class deptDataContext
    {
        public static IEnumerable<dept> GetDept()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.dept.ToList();
            }
        }
        public static bool AddDeptName(int dept_id, string deptName)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                int ct = pfentity.dept.Where(cd => cd.d_id== dept_id && cd.d_name == deptName).Count();
                if (ct == 0)
                {
                    dept newDept = new dept();
                    newDept.d_id = dept_id;
                    newDept.d_name = deptName;
                    pfentity.AddTodept(newDept);
                    pfentity.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
        public static bool DelDept(int id)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                dept d = pfentity.dept.Where(cd => cd.id == id).First();
                pfentity.DeleteObject(d);
                pfentity.SaveChanges();
                r = true;
            }
            return r;
        }
        public static bool updateDept(int dept_id,string deptName)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                var d = pfentity.dept.Where(cd => cd.d_id == dept_id).FirstOrDefault();
                if (d==null)
                {
                    dept newDept = new dept();
                    newDept.d_id = dept_id;
                    newDept.d_name = deptName;
                    pfentity.AddTodept(newDept);
                    pfentity.SaveChanges();
                    r = true;
                }
                else
                {
                    d.d_name = deptName;
                    pfentity.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
    }
}
