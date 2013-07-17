using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tencent.OA.Framework.Messages;
using Tencent.OA.Framework.Messages.DataContract;
using PlocyFlow.DAL.Service;
namespace PlocyFlow.Service
{
    /// <summary>
    /// 发送Email,RTX,短信消息的辅助类
    /// </summary>
    public class SendNotice
    {
        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件正文</param>
        /// <param name="to">接收人，多个接收人用,分割</param>       
        /// <returns>是否成功。true:成功 false:失败</returns>
        public static bool SendMail(string title, string content, string sender, string to)
        {
            return SendMail(title, content, sender, to, null, null);
        }

        public static bool SendMail(string title, string content, string sender, string to, string cc)
        {
            return SendMail(title, content, sender, to, cc, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件正文</param>
        /// <param name="to">接收人，多个接收人用,分割</param>
        /// <param name="attachments">附件</param>
        /// <returns>是否成功。true:成功 false:失败</returns>
        public static bool SendMail(string title, string content, string sender, string to, List<TencentMailAttachment> attachments)
        {
            return SendMail(title, content, sender, to, null, attachments);
        }

        public static bool SendMail(string title, string content, string sender, string to, string cc, List<TencentMailAttachment> attachments)
        {
            try
            {
                TencentEmail te = new TencentEmail();
                te.Title = title;
                te.Content = content;
                te.From = sender;
                te.To = to;
                if (!string.IsNullOrEmpty(cc))
                    te.CC = "";
                te.Priority = MessagePriority.Normal;
                te.BodyFormat = TencentMailFormat.Html;
                te.EmailType = TencentMailType.SEND_TO_ENCHANGE;
                if (attachments != null)
                    te.Attachments = attachments;

                //Logger.Write("开始发送邮件");
                return MessageHelper.SendMail(te);
            }
            catch (Exception ex)
            {
               Logger.Write(ex,ex.Message );
                return false;
            }
        }

        #endregion

        #region 发送RTX消息
        /// <summary>
        /// 发送RTX消息
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="receiver">发件人</param>
        /// <param name="msgInfo">信息内容</param>
        /// <returns>是否成功。true:成功 false:失败</returns>
        public static bool SendRTXMessage(string title, string sender, string receiver, string msgInfo)
        {
            RTXMessage rtxMessage = new RTXMessage()
            {
                MsgInfo = msgInfo,
                Receiver = receiver,
                Title = title,
                Priority = MessagePriority.Normal,
                Sender = sender
            };
            return MessageHelper.SendRTXMessage(rtxMessage);
        }
        #endregion

        #region 发送短信
        /// <summary>
        /// 发送短信
        /// </summary>     
        /// <param name="receiver">接收人</param>
        /// <param name="msgInfo">信息内容</param>
        /// <returns>是否成功。true:成功 false:失败</returns>
        public static bool SendSMS(string sender, string receiver, string msgInfo)
        {
            return MessageHelper.SendSMSMessage(sender,"", msgInfo);
        }

        #endregion
        
    }
}
