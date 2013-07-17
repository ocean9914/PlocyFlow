using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class game_typeDataContext
    {
        public static IEnumerable<game_type> GetGame_type()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.game_type.ToList();
            }
        }
       public static bool updateGame_type(string Id, string Name)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                var p = pfentity.game_type.Where(cp => cp.g_id == Id).FirstOrDefault();
                if (p == null)
                {
                    game_type ng=new game_type ();
                    ng.g_id=Id;
                    ng.g_name=Name;
                    pfentity.AddTogame_type(ng);
                    pfentity.SaveChanges();
                    r = true;
                }
                else
                {
                    p.g_name = Name;
                    pfentity.SaveChanges();
                    r = true;
                }
            }
            return r;
        }

    }
    
}
