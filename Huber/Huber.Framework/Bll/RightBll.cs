using Huber.Framework.Dal;
using Huber.Kernel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Bll
{
    public class RightBll
    {
        /// <summary>获取权限列表
        /// </summary>
        /// <returns>json字符串</returns>
        public RightEntity GetAllRights()
        {
            RightDal rightDal = new RightDal();
            var rightEntities = rightDal.GetAllRights();
            var rootEntities = rightEntities.Where(m => m.ParentId == 0);
            AppendChild(rootEntities, rightEntities);
            RightEntity rightEntity = new RightEntity();
            rightEntity.Id = 0;
            rightEntity.IsMenu = 0;
            rightEntity.Level = 0;
            rightEntity.Name = "权限管理";
            rightEntity.ParentId = 0;
            rightEntity.Url = "..";
            rightEntity.Category = -1;
            rightEntity.Deleted = 0;
            rightEntity.Describe = "..";
            rightEntity.Children = rootEntities.ToList();
            return rightEntity;
        }
        /// <summary>获得权限列表
        /// </summary>
        /// <param name="category">插件Id</param>
        /// <returns></returns>
        public List<RightEntity> GetRights(int category)
        {
            RightDal rightDal = new RightDal();
            return rightDal.GetRights(category);
        }
        public List<RightEntity> GetRightEntities()
        {
            RightDal rightDal = new RightDal();
            var rightEntities = rightDal.GetAllRights();
            return rightEntities;
        }
        /// <summary>获得权限json 
        /// </summary>
        /// <param name="rightIdList"></param>
        /// <returns></returns>
        public List<RightEntity> GetRights(List<int> rightIdList)
        {
            if (rightIdList == null || !rightIdList.Any())
                return null;
            rightIdList = rightIdList.Distinct().ToList();
            RightDal rightDal = new RightDal();
            var rightEntities = rightDal.GetRights(string.Join(",", rightIdList));
            var rootEntities = rightEntities.Where(m => m.ParentId == 0);
            return rootEntities == null ? null : rootEntities.ToList();
        }
        private IEnumerable<RightEntity> AppendChild(IEnumerable<RightEntity> list, IEnumerable<RightEntity> all)
        {
            foreach (RightEntity item in list)
            {
                var s = AppendChild(all.Where(m => m.ParentId == item.Id).ToList(), all);
                item.Children.AddRange(s);
            }
            return list;
        }
        /// <summary>添加节点 
        /// </summary>
        /// <param name="rightEntity"></param>
        /// <returns>1：成功 -1：失败 -2：已存在相同url的节点</returns>
        public int AddRight(RightEntity rightEntity)
        {
            RightDal rightDal = new RightDal();
            if (rightDal.Exist(rightEntity.Url) > 0)
            {
                return -2;
            }
            else
            {
                return rightDal.AddRight(rightEntity);

            }
        }
        public bool Exist(string url)
        {
            RightDal rightDal = new RightDal();
            return rightDal.Exist(url) > 0;
        }
        public bool DeleteRight(int rightId)
        {
            RightDal rightDal = new RightDal();
            List<RightEntity> rightEntities = rightDal.GetAllRights();
            List<int> childIds = GetChildIds(rightId, rightEntities);
            childIds.Add(rightId);
            return DeleteRightRel(childIds);
            //return rightDal.DeleteRight(childIds);
        }
        private List<int> GetChildIds(int rightId, List<RightEntity> rightEntities)
        {
            List<RightEntity> list = rightEntities.Where(m => m.ParentId == rightId).ToList();
            List<int> rightIds = list.Select(m => m.Id).ToList();
            foreach (RightEntity rightEntity in list)
            {
                rightIds.AddRange(GetChildIds(rightEntity.Id, rightEntities));
            }
            return rightIds;
        }
        /// <summary>修改节点属性 
        /// </summary>
        /// <param name="rightEntity"></param>
        /// <returns>1：成功 -1：失败 -2：已存在相同的url</returns>
        public int UpdateRight(RightEntity rightEntity)
        {
            RightDal rightDal = new RightDal();
            int result = rightDal.Exist(rightEntity.Url);
            if (result > 0 && result != rightEntity.Id)
            {
                return -2;
            }
            return rightDal.UpdateRight(rightEntity) ? 1 : -1;
        }
        public List<RightEntity> UserGetRights(string url)
        {
            return new RightDal().UserGetRights(url);
        }
        /// <summary>获取所有菜单
        /// </summary>
        /// <returns></returns>
        public List<RightEntity> GetAllMenu(List<string> rights, bool superAdmin)
        {
            RightDal rightDal = new RightDal();
            var rightEntities = rightDal.GetAllRights().Where(m => m.IsMenu == 1 && m.Deleted == 0);
            if (!superAdmin)
            {
                rightEntities = rightEntities.Where(m => rights.Contains(m.Id.ToString()));
            }
            var rootEntities = rightEntities.Where(m => m.ParentId == 0);
            AppendChild(rootEntities, rightEntities);
            return rootEntities.ToList();
        }
        /// <summary>删除权限，真删，同时移出角色表中的权限
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        public bool DeleteRightRel(List<int> rightIds)
        {
            foreach (var item in rightIds)
            {
                //在角色表中移出权限
                RoleDal roleDal = new RoleDal();
                roleDal.RemoveRight(item);
            }

            //删除权限
            RightDal rightDal = new RightDal();
            return rightDal.DeleteRightRel(rightIds);
        }
        /// <summary>获得所有已经启用的模块Id
        /// </summary>
        /// <returns></returns>
        public List<int> GetCategory()
        {
            RightDal rightDal = new RightDal();
            return rightDal.GetCategory();
        }
    }
}
