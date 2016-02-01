using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Huber.Framework.Tools
{
    class CookieFunc
    {
        /// <summary>写入cookie
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        /// <param name="day"></param>
        /// <param name="_doMain"></param>
        public static void writeCookie(string strName, string strValue, int day = 0, string _doMain = "")
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;

            if (!string.IsNullOrEmpty(_doMain))
            {
                HttpContext.Current.Response.AddHeader("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
                cookie.Domain = _doMain;
            }
            if (day != 0)
            {
                cookie.Expires = DateTime.Now.AddDays(day);
            }
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 读取cookie 的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadCookie(string key)
        {
            string strValue = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
            {
                strValue = cookie.Value;
            }
            return strValue;
        }

        /// <summary>
        /// 删除cookie的方法
        /// </summary>
        /// <param name="strName"></param>
        public static void RemoveCookie(string strName, string _domainValue = "")
        {
            if (string.IsNullOrEmpty(strName))
            {
                return;
            }


            HttpCookie cookie = new HttpCookie(strName);
            if (!string.IsNullOrEmpty(_domainValue))
            {
                cookie.Domain = _domainValue;
            }
            // 删除Cookie
            cookie.Expires = new DateTime(1900, 1, 1);
            HttpContext.Current.Response.Cookies.Add(cookie);

        }
    }
}
