using Huber.Framework.Dal;
using Huber.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Bll
{
    public class RoleBll
    {
        /// <summary>获得角色列表 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<RoleEntity> GetRoles(int pageIndex, int pageSize, string RoleName, out int count)
        {
            int skip = (pageIndex - 1) * pageSize;
            RoleDal roleDal = new RoleDal();
            List<RoleEntity> roleEntities = roleDal.GetRoles(skip, pageSize, RoleName, out count);
            return roleEntities;
        }
        /// <summary>获得角色列表 
        /// </summary>
        /// <param name="roleIdList"></param>
        /// <returns></returns>
        public List<RoleEntity> GetRoles(List<int> roleIdList)
        {
            if (roleIdList == null || !roleIdList.Any())
                return new List<RoleEntity>();
            roleIdList = roleIdList.Distinct().ToList();
            RoleDal roleDal = new RoleDal();
            return roleDal.GetRoles(roleIdList);
        }
        public List<RoleEntity> GetRoles(string roleIds)
        {
            List<int> roleIdList = new List<int>();
            foreach (string item in roleIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                int roleId;
                if (Int32.TryParse(item, out roleId))
                {
                    roleIdList.Add(roleId);
                }
            }
            if (roleIdList.Count > 0)
            {

                return GetRoles(roleIdList);

            }
            else
            {
                return null;
            }
        }

        public List<RoleEntity> GetRoles()
        {
            RoleDal roleDal = new RoleDal();
            List<RoleEntity> roleEntities = roleDal.GetRoles();
            return roleEntities;
        }

        /// <summary>添加角色 
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <returns></returns>
        public bool AddRole(RoleEntity roleEntity)
        {
            RoleDal roleDal = new RoleDal();
            return roleDal.AddRole(roleEntity);
        }
        /// <summary>删除角色 
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public bool DeleteRole(int roleId)
        {
            RoleDal roleDal = new RoleDal();
            if (roleDal.DeleteRole(roleId))
            {
                UserDal userDal = new UserDal();
                return userDal.RemoveRole(roleId);
            }
            return false;
        }
        /// <summary>修改角色信息 
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <returns></returns>
        public bool UpdateRole(RoleEntity roleEntity)
        {
            RoleDal roleDal = new RoleDal();
            return roleDal.UpdateRole(roleEntity);
        }

        /// <summary>添加权限 
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="rightId">权限Id</param>
        /// <returns></returns>
        public bool AddRight(int roleId, int rightId)
        {
            RoleDal roleDal = new RoleDal();
            return roleDal.AddRight(roleId, rightId);
        }

        /// <summary>移除权限 
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="rightId">权限Id</param>
        /// <returns></returns>
        public bool RemoveRight(int roleId, int rightId)
        {
            RoleDal roleDal = new RoleDal();
            return roleDal.RemoveRight(roleId, rightId);
        }

        public bool UpdateRight(int rightId, string rights)
        {
            RoleDal roleDal = new RoleDal();
            return roleDal.UpdateRight(rightId, rights);
        }
    }
}
