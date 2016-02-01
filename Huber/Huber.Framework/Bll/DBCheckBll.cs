using Huber.Framework.Dal;
using Huber.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Bll
{
    /// <summary>数据库初始化
    /// </summary>
    public class DBCheckBll
    {
        public void InitDb()
        {
            UserEntity user = new UserEntity();
            user.Name = "超级管理员";
            user.Photo = "";
            user.RolesIds = "";
            user.Status = 1;
            user.Uid = UserBll.SuperAdminID;// "sys_root";
            new DbCheckDal().InitDb(user);
        }
    }
}
