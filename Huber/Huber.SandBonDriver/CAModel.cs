using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.SandBonDriver
{
    /// <summary>controller/action 封包类
    /// </summary>
    public  class CAModel
    {
         public string ControllerName{get;set;}
        public string ActionName{get;set;}
        public string UrlPath { get; set; }
        /// <summary>构造
        /// </summary>
        /// <param name="controllerName">controller 全名（带命名空间）</param>
        /// <param name="actionName">action 全名（不带参数）</param>
        public CAModel(string controllerName,string actionName)
        {
            ControllerName=controllerName;
            ActionName=actionName;
            UrlPath = controllerName.Replace(".Areas.", "/").Replace(".Controllers.", "/");//controller转Url
            if (UrlPath.EndsWith("Controller"))
            {
                UrlPath = string.Format("/{0}/{1}", UrlPath.Substring(0, UrlPath.Length - 10),actionName);
            }
        }

        public CAModel()
        { 
        }
    }
}
