using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Entity
{
    public class RoleEntity
    {
        /// <summary>角色Id 
        /// </summary>        
        public int Id { get; set; }
        /// <summary>角色名称 
        /// </summary>
        public string Name { get; set; }
        /// <summary>权限列表 
        /// </summary>
        public string RightIds { get; set; }
        /// <summary>是否管理员 0：否 1：是 
        /// </summary>
        public int IsSuper { get; set; }
    }
}
