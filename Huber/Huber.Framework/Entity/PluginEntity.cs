using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Entity
{
    public class PluginEntity
    {
        ///<summary>插件Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>插件名--与版本联合唯一，不支持中文
        /// </summary>
        public string Name { get; set; }
        /// <summary>负责人-作者 
        /// </summary>
        public string Author { get; set; }
        /// <summary>默认控制器 
        /// </summary>
        public string DefaultController { get; set; }
        /// <summary>默认action 
        /// </summary>
        public string DefaultAction { get; set; }
        /// <summary>描述 
        /// </summary>
        public string Describe { get; set; }
        /// <summary>插件状态 0：禁用 1：启用 
        /// </summary>
        public int Status { get; set; }
        /// <summary>插件版本，用来划分应用程序域引用的DLL
        /// </summary>
        public int PVersion { get; set; }
        /// <summary>是否在菜单显示 0:不显示  1：显示
        /// </summary>
        public int MenuShow { get; set; }
        /// <summary>图标
        /// </summary>
        public string Icon { get; set; }
    }
}
