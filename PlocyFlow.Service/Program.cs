using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace PlocyFlow.Service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {

#if DEBUG
            ServiceHandler cs = new ServiceHandler();
            cs.StartSchedule();
            Console.WriteLine("启动成功！");
            Console.ReadLine();
            cs.Stop();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ServiceHandler() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
