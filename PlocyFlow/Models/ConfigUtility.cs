using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PlocyFlow.Models
{
    /*
     * Web.config配置节点信息
     */
    public class ConfigUtility
    {
        public static int SystemID
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["SystemID"]); }
        }

        public static string SystemName
        {
            get { return ConfigurationManager.AppSettings["SystemName"]; }
        }

        public static string HomePage
        {
            get { return ConfigurationManager.AppSettings["HomePage"]; }
        }

        public static bool IsDev
        {
            get
            {
                var isDev = ConfigurationManager.AppSettings["IsDev"];
                if (!string.IsNullOrEmpty(isDev))
                {
                    return bool.Parse(isDev);
                }
                return false;
            }
        }
        public static string DevUser
        {
            get { return ConfigurationManager.AppSettings["DevUser"]; }
        }
        public static string BipUrl
        {
            get { return ConfigurationManager.AppSettings["bipUrl"]; }
        }
    }

    public class BaseTabMain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
    }
}
