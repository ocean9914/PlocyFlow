using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class t_pageDataContext
    {
        public static IEnumerable<t_page> GetT_Page()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.t_page.ToList();
            }
        }
        public static t_page GetT_page(int id)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.t_page.Where(page => page.id == id).First();
            }
        }
        public static t_page GetT_page(string page_url)
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.t_page.Where(page => page.page_url == page_url).First();
            }
        }
        public static List<t_page> Get_ShowPage()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.t_page.Where(page => page.is_show == "1").ToList();
            }
        }
    }
}
