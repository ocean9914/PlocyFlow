using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PlocyFlow.HelpExtensions
{
    public static class HelpsExtensions
    {
        public static MvcHtmlString BuilderButton(this HtmlHelper html, string test)
        {
            var s = "<input type=\"button\" value=\"" + test + "\" />";
            return MvcHtmlString.Create(s);
        }
    }
}