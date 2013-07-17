using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using PlocyFlow.Models;
using PlocyFlow.DAL.Service;
namespace PlocyFlow.Service.Component
{
    public class SendService : IScheduleService
    {
        public void Execute()
        {
            ILog logger = log4net.LogManager.GetLogger("log4netMainLogger");
            bool islog;
            string temp = ConfigurationManager.AppSettings["logSwitch"];
            if (!bool.TryParse(temp, out islog))
                islog = false;

            logger.Debug("Send Message start:"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")); 
            
            try
            {
                var myveryfy = MyWorkedList.GetAllNClosed("", "", "", "");
                if (myveryfy != null && myveryfy.Count > 0)
                {

                    var sender = ConfigurationManager.AppSettings["Sender"];//发件人
                    var list = XmlHelper.ReadFromXml("veryfy");//发送内容
                    if (list != null && list.Count > 0)
                    {
                        var _title = list[0];
                        var _head = list[1];
                        var _content = list[2];
                        foreach (var w in myveryfy)
                        {
                            logger.Debug("Send Message to" + w.NextOper);
                            var to = getToList(w.NextOper);//收件人
                            _content.Replace("#Optype#", w.WorkedType).Replace("#ProductName#", w.ProductName).Replace("#Dept#", w.DeptName).Replace("#Opdate#", w.OperDate);
                            foreach (var str in to)
                            {
                                _head = _head.Replace("#username#", str);
                                if (islog)
                                {
                                    logger.Info("邮件标题：" + _title + "-" + w.WorkedType);
                                    logger.Info("邮件正文：" + _head + _content);
                                    logger.Info("开始发送邮件！");
                                }
                                if (SendNotice.SendMail(_title + "-" + w.WorkedType, _head + _content, sender, str))
                                { if (islog) logger.Info("邮件发送成功！"); }
                                else
                                { if (islog) logger.Info("邮件发送失败！"); }
                                if (SendNotice.SendRTXMessage(_title + "-" + w.WorkedType, sender, str, _head + _content))
                                { if (islog) logger.Info("RTX发送成功！"); }
                                else
                                { if (islog) logger.Info("RTX发送失败！"); }
                            }
                        }
                        if (islog) logger.Info("邮件发送结束！");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Info("待办发送失败", ex);
            }
            logger.Debug("Send Message end:"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")); 
        }
        private string[] getToList(string userList)
        {
            if (userList != null && userList.Trim() != "")
            {
                List<string> r = new List<string>();
                char[] sp = { ',' };
                string[] ss = userList.Split(sp);
                foreach (string s in ss)
                {
                    if (!string.IsNullOrEmpty(s))
                        r.Add(s.Trim());
                }
                return r.ToArray();
            }
            else
                return null;
        }
    }
}
