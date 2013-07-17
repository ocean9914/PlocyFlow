using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.DAL.Context
{
    public class game_statusDataContext
    {
        public static IEnumerable<game_status> GetGame_Status()
        {
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                return pfentity.game_status.ToList();
            }
        }
        public static bool updateGame_status(int Id, string Name)
        {
            bool r = false;
            using (policy_FlowEntities pfentity = new policy_FlowEntities())
            {
                var p = pfentity.game_status.Where(cp => cp.s_id == Id).FirstOrDefault();
                if (p == null)
                {
                    game_status ng = new game_status();
                    ng.s_id = Id;
                    ng.s_name = Name;
                    pfentity.AddTogame_status(ng);
                    pfentity.SaveChanges();
                    r = true;
                }
                else
                {
                    p.s_name = Name;
                    pfentity.SaveChanges();
                    r = true;
                }
            }
            return r;
        }
    }
}
