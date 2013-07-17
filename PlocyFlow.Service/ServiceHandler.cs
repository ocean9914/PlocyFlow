using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using PlocyFlow.DAL.Service;
namespace PlocyFlow.Service
{
    partial class ServiceHandler : ServiceBase
    {
        public ServiceHandler()
        {
            InitializeComponent();
        }

        private ScheduleServiceFactory ssf = new ScheduleServiceFactory();

        public void StartSchedule()
        {
            ssf.Start();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            StartSchedule();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            ssf.Stop();
            base.OnStop();
        }
    }
}
