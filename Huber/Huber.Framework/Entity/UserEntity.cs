using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Entity
{
    public class UserEntity
    {
        /// <summary>用户id
        /// </summary>
        public string Uid { get; set; }
        /// <summary>用户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>用户头像
        /// </summary>
        public string Photo { get; set; }
        /// <summary>角色Id列表
        /// </summary>
        public string RolesIds { get; set; }
        /// <summary>用户状态 0：未启用 1：启用
        /// </summary>
        public int Status { get; set; }
    }
    /// <summary>用户实体类，带权限列表，不建议用于列表展示
    /// </summary>
    public class UserSingleEntitiy
    {
        public UserSingleEntitiy()
        {

        }
        public UserSingleEntitiy(UserEntity user)
        {
            Rights = new List<string>();
            foreach (FieldInfo fi in typeof(UserEntity).GetFields())
            {
                FieldInfo appNameInfo = typeof(UserSingleEntitiy).GetField(fi.Name);
                appNameInfo.SetValue(this, fi.GetValue(user));
            }
        }
        /// <summary>权限列表 
        /// </summary>
        public List<string> Rights { get; set; }
    }
}
