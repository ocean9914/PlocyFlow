using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using PlocyFlow.DAL.App;
using PlocyFlow.DAL.Service;
namespace PlocyFlow.Service.Component
{
    public class BipService :IScheduleService
    {

        public void Execute()
        {
            ILog logger = log4net.LogManager.GetLogger("log4netMainLogger");
            bool islog;
            string temp = ConfigurationManager.AppSettings["logSwitch"];
            if (!bool.TryParse(temp, out islog))
                islog = false;
            logger.Debug("Send Message start:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")); 
            try
            {
                var bip = new GetBipData();
                bip.getData();
                if (bip.Error)
                    logger.Error(bip.Message);
            }
            catch (Exception ex)
            {
                logger.Error("PlocyFlow Get BipData:" + ex.Message);
            }
            logger.Debug("Send Message end:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        }
    }
}
