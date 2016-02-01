using Huber.Framework.DBHelper;
using Huber.Framework.Entity;
using Huber.Framework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Dal
{
    internal class DbCheckDal
    {
        /// <summary>初始化数据库
        /// </summary>
        public void InitDb(UserEntity user)
        {
            SqlLiteHelper herpler = new SqlLiteHelper();
            if (herpler.InitDb())
            {
                new LoginDal().creatTable(herpler);
                new PluginDal().creatTable(herpler);
                new RightDal().creatTable(herpler);
                new RoleDal().creatTable(herpler);
                new UserDal().creatTable(herpler);
               
                
                new UserDal().AddUser(user, EncryptionFunc.MD5Encrypt(user.Uid+"\f123456"));
            }
        }
    }
}
