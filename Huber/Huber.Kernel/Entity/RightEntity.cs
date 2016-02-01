using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel.Entity
{
    [Serializable]
    public class RightEntity
    {
        public RightEntity()
        {
            Children = new List<RightEntity>();
        }
        /// <summary>权限id
        /// </summary>
        public int Id { get; set; }
        /// <summary>权限名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>权限对应的访问URL路径
        /// </summary>
        public string Url { get; set; }
        public int Level { get; set; }
        /// <summary>父级ID
        /// </summary>
        public int ParentId { get; set; }
        public string Describe { get; set; }
        /// <summary>是否可以作为菜单展示
        /// </summary>
        public int IsMenu { get; set; }
        /// <summary>子权限
        /// </summary>
        public List<RightEntity> Children { get; set; }
        /// <summary>类别 插件Id
        /// </summary>
        public int Category { get; set; }
        /// <summary>已被删除       0 未被删除 1 已被删除
        /// </summary>
        public int Deleted { get; set; }
    }
}
