using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel.Entity
{
     public class ResponseCodeEntity
    {
        /// <summary>404
        /// </summary>
        public static string ULR404 = "/NotPageFound/_404";
        /// <summary>404ajax
        /// </summary>
        public static string CODE404 = "NotPage";
        /// <summary>登录页URL
        /// </summary>
        public static string LoginURL = "/User/Login";
        /// <summary>未登录ajax
        /// </summary>
        public static string NoLogin = "NoLogin";
        /// <summary>没有权限ajax
        /// </summary>
        public static string NoRight = "NoRight";
        /// <summary>没有权限url
        /// </summary>
        public static string NoRightURL = "/User/NoRight";
    }
}
