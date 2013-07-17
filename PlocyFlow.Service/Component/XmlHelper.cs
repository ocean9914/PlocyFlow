using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace PlocyFlow.Service
{
    public class XmlHelper
    {
        public static readonly string templateUrl = System.AppDomain.CurrentDomain.BaseDirectory + "MailTemplate.xml";

        /// <summary>
        /// 从xml文档中读取指定id节点
        /// </summary>
        /// <param name="mailID"></param>
        /// <returns></returns>
        public static List<string> ReadFromXml(string mailID)
        {
            var list = new List<string>();
            try
            {
                //实例化xml文档
                XDocument document = XDocument.Load(templateUrl);
                //读取指定节点
                var elements = document.Root.Elements("mail");
                foreach (var mail in elements)
                {
                    if (mail.Element("id").Value == mailID)
                    {
                        list.Add(mail.Element("title").Value);
                        list.Add(mail.Element("head").Value);
                        list.Add(mail.Element("content").Value);
                        break;
                    }
                }
            }
            catch (Exception err)
            {
                list.Clear();
                throw err;
            }
            return list;
        }


    }
}
