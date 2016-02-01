using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Entity
{
    public class UrlPathEntity
    {
        /// <summary>
        /// //插件名称
        /// </summary>  
        public string pluginname { get; set; }
        /// <summary>
        /// //插件版本
        /// </summary>  
        public int pluginversion { get; set; }
        /// <summary>
        /// //控制器名称（包含area）
        /// </summary>   
        public string controller { get; set; }
        /// <summary>
        /// //action名称
        /// </summary>
        public string action { get; set; }
    }
}
