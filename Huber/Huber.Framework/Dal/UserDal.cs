using Huber.Framework.DBHelper;
using Huber.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Dal
{
    internal class UserDal
    {
        internal void creatTable(SqlLiteHelper sqlLiteHelper)
        {
            //Id,Name,Describe,Status,Author,DefaultController,DefaultAction,PVersion,MenuShow,Icon
            string sql = @"CREATE TABLE `t_users` (
	                        `Uid`	TEXT NOT NULL,
	                        `Name`	TEXT NOT NULL,
	                        `Photo`	TEXT,
	                        `RolesIds`	TEXT NOT NULL DEFAULT (','),
	                        `Status`	INTEGER NOT NULL DEFAULT 0,
	                        `PassWord`	TEXT NOT NULL DEFAULT ('000000'),
	                        PRIMARY KEY(Uid)
                        );";
            sqlLiteHelper.RunSQL(sql);
        }
        public List<UserEntity> GetUsers(int skip, int pagesize, string userName, out int count)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select Uid,Name,Photo,RolesIds,Status from t_users ");

            if (!string.IsNullOrEmpty(userName))
            {
                sql.Append(" where Uid like @Uid ");
            }
            if (pagesize != 0)
            {
                sql.Append(" Limit @count Offset @skip ");

            }


            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@count", pagesize),
                new SQLiteParameter("@skip", skip),
                new SQLiteParameter("@Uid", "%"+userName+"%")
            };
            SqlLiteHelper sqliteHelper = new SqlLiteHelper();
            DataTable dataTable = sqliteHelper.GetDataTable(sql.ToString(), para);
            List<UserEntity> userEntities = new List<UserEntity>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                userEntities.Add(Conver2Entity(dataRow));
            }
            count = sqliteHelper.GetCount("t_users");
            return userEntities;
        }

        private UserEntity Conver2Entity(DataRow dataRow)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.Name = dataRow["Name"].ToString();
            userEntity.Photo = dataRow["Photo"] == DBNull.Value ? "/Content/Img/Photo/photo16.jpg" : dataRow["Photo"].ToString();
            userEntity.RolesIds = dataRow["RolesIds"].ToString();
            userEntity.Uid = dataRow["Uid"].ToString();
            userEntity.Status = Int32.Parse(dataRow["Status"].ToString());
            return userEntity;
        }

        public bool SetUserStatus(string uId, int status)
        {
            string sql = "update t_users set Status=@status where Uid=@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@status", status), new SQLiteParameter("@uId", uId) };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }

        public bool AddUser(UserEntity userEntity, string pwd)
        {
            string sql = "Insert into t_users (Uid,Name,Photo,Status,PassWord ) values (@uId,@Name,@Photo,@status,@passWord)";
            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@status", userEntity.Status),
                new SQLiteParameter("@Name", userEntity.Name),
                new SQLiteParameter("@Photo", userEntity.Photo),
                new SQLiteParameter("@uId", userEntity.Uid),
                new SQLiteParameter("@passWord", pwd),
            };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>用户Id是否已存在
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public bool ExistUId(string uId)
        {
            string sql = "select Uid from t_users where Uid=@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@uId", uId) };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            if (dataTable != null && dataTable.Rows.Count > 1)
                return true;
            return false;
        }
        /// <summary>删除用户
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public int DeleteUser(string uId)
        {
            string sql = "delete from t_users  where Uid=@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@uId", uId) };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result;
        }
        /// <summary>修改用户昵称 
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="userName">用户昵称</param>
        /// <returns></returns>
        public bool UpdateName(string uId, string userName)
        {
            string sql = "update t_users set Name=@userName where Uid=@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@userName", userName), new SQLiteParameter("@uId", uId) };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>修改用户头像
         /// </summary>
         /// <param name="uId">用户Id</param>
         /// <param name="photo">用户头像</param>
         /// <returns></returns>
        public bool UpdatePhoto(string uId, string photo)
        {
            string sql = "update t_users set Photo=@Photo  where Uid=@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@Photo", photo), new SQLiteParameter("@uId", uId) };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>为用户添加角色 
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public bool AddRole(string uId, int roleId)
        {
            string sql = "update t_users set RolesIds = RolesIds ||@roleId||',' where Uid =@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@roleId", roleId), new SQLiteParameter("@uId", uId) };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }

        public UserEntity GetUser(string uId)
        {
            string sql = "select Uid,Name,Photo,RolesIds,Status from t_users where Uid=@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@uId", uId) };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            if (dataTable != null && dataTable.Rows.Count == 1)
            {
                UserEntity userEntity = Conver2Entity(dataTable.Rows[0]);
                return userEntity;
            }
            return null;
        }
        /// <summary>删除用户角色 
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public bool DeleteRole(string uId, int roleId)
        {
            string sql = "update t_users set RolesIds = Replace(RolesIds,','||@roleId||',',',') where Uid=@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@roleId", roleId), new SQLiteParameter("@uId", uId) };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }

        /// <summary>修改密码 
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public bool UpdatePassWord(string uId, string passWord)
        {
            string sql = "update t_users set PassWord =@passWord where Uid =@uId;";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@passWord", passWord), new SQLiteParameter("@uId", uId) };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>修改密码 
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public int UpdatePassWord(string uId, string passWord, string newPWD)
        {
            string sql = "select  PassWord from t_users where Uid =@uId ;";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@uId", uId) };

            DataTable dt = new SqlLiteHelper().GetDataTable(sql, para);
            if (dt == null || dt.Rows.Count == 0)
            {
                return -1;
            }
            else
            {
                if (dt.Rows[0]["PassWord"].ToString() != passWord)
                {
                    return 0;
                }
                else
                {
                    sql = "update t_users set PassWord =@passWord where Uid =@uId ;";
                    para = new SQLiteParameter[] { new SQLiteParameter("@passWord", newPWD), new SQLiteParameter("@uId", uId) };
                    int result = new SqlLiteHelper().RunSQL(sql, para);
                    if (result > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }


        }
        /// <summary>移除角色 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool RemoveRole(int roleId)
        {
            string sql = "update t_users set RolesIds = Replace(RolesIds,','||@roleId||',',',');";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@roleId", roleId), new SQLiteParameter("@roleId", roleId) };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }

        public bool AddRole(string uId, List<int> roleIds)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update t_users set ");
            foreach (int roleId in roleIds)
            {
                sb.AppendFormat("RolesIds = Replace(RolesIds,',{0},',',') , ", roleId);
            }
            sb.AppendFormat("RolesIds = RolesIds || '{0},' ", string.Join(",", roleIds));
            sb.Append("where Uid = @uId;");
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@uId", uId) };
            int result = new SqlLiteHelper().RunSQL(sb.ToString(), para);
            return result > 0;
        }

        public bool UpdateRole(string uId, string roleIds)
        {
            string sql = "update t_users set RolesIds = @roleIds where Uid = @uId";
            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@uId", uId),
                new SQLiteParameter("@roleIds",roleIds)
            };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }

        /// <summary>登录
        /// </summary>
        /// <param name="uId">用户id</param>
        /// <param name="pwd">密码（加密之后的）</param>
        /// <returns>1登录成功，-1账号不存在，-2密码错误， -3账户已被禁用</returns>
        public int login(string uId, string pwd)
        {
            string sql = "select Uid,Name,RolesIds,Status,PassWord from t_users where Uid=@uId";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@uId", uId) };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            if (dataTable != null && dataTable.Rows.Count == 1)
            {
                DataRow dataRow = dataTable.Rows[0];
                if (dataRow["Status"].ToString() == "1")
                {
                    if (dataRow["PassWord"].ToString() == pwd)
                    {
                        return 1;
                    }
                    else
                    {
                        return -2;
                    }
                }
                else
                {
                    return -3;
                }
            }
            else
            {
                return -1;
            }
        }


    }
}
