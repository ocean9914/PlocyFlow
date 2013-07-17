using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Configuration;
using Tencent.OA.Framework.Web.Security;
using PlocyFlow.DAL.App;
namespace PlocyFlow.Models
{
    /*
     * 登录
     */
    public class LoginHandler : IHttpHandler, IRequiresSessionState
    {
        protected virtual void ProcessRequest(HttpContextBase context)
        {

            string returnUrl = context.Request.QueryString["returnUrl"];
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = ConfigUtility.HomePage;
            bool isDev;
            string temp = ConfigurationManager.AppSettings["IsDev"];
            if (!bool.TryParse(temp, out isDev))
                isDev = false;
            //if (getExt(returnUrl) == "ashx")
            //{
            //    context.Response.Redirect(returnUrl);
            //}
            //else
            //{
                PassportAuthentication passport = new PassportAuthentication();
                // 登出
                if (context.Request.QueryString["action"] == "signout")
                {
                    cacheBase.EmptyCache();
                    context.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
                    FormsAuthentication.SignOut();
                    if (!isDev)
                    {
                        passport.SignOut(
                            HttpUtility.UrlDecode(ConfigUtility.SystemName),
                            HttpUtility.UrlEncode(returnUrl)
                        );
                    }
                    context.Response.Redirect("~/");
                }

                if (!context.Request.IsAuthenticated)
                {
                    // 如果未通过身份验证，则跳转到TOF登陆界面
                    //
                    if (!isDev)
                    {
                        if (!passport.IsAuthenticated())
                        {
                            passport.SignIn(
                                HttpUtility.UrlDecode(ConfigUtility.SystemName),
                                HttpUtility.UrlEncode(context.Request.Url.AbsoluteUri)
                            );
                        }
                        else
                        {
                            FormsAuthentication.SetAuthCookie(passport.Ticket.LoginName, true);
                            FormsAuthentication.RedirectFromLoginPage(passport.Ticket.LoginName, true);
                        }
                    }
                    else
                    {
                        string devUser = ConfigurationManager.AppSettings["DevUser"];
                        FormsAuthentication.SetAuthCookie(devUser, true);
                        FormsAuthentication.RedirectFromLoginPage(devUser, true);
                    }
                }
                else
                {
                    context.Response.Redirect(returnUrl);
                }
            //}
        }
        private string getExt(string url)
        {
            string r = "";
            if (url != "")
            {
                string absolute = url;
                int startIdx = absolute.LastIndexOf("/") + 1;
                int endIdx = absolute.LastIndexOf("?");
                if (endIdx != -1)
                {
                    r = absolute.Substring(startIdx, endIdx - startIdx);
                }
                else
                    r = absolute.Substring(startIdx);
                char[] sp = {'.'};
                string[] ss = r.Split(sp);
                if (ss.Length > 1)
                    r = ss[1].ToLower();
            }
            return r;
        }

        bool IHttpHandler.IsReusable
        {
            get { return false; }
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            this.ProcessRequest(new HttpContextWrapper(context));
        }
    }
}
