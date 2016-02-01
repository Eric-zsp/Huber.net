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
    internal class RoleDal
    {
        internal void creatTable(SqlLiteHelper sqlLiteHelper)
        {
            //Id,Name,Describe,Status,Author,DefaultController,DefaultAction,PVersion,MenuShow,Icon
            string sql = @"CREATE TABLE `t_roles` (
	                        `Id`	INTEGER NOT NULL,
	                        `Name`	TEXT NOT NULL,
	                        `RightIds`	TEXT NOT NULL DEFAULT ',',
	                        `IsSuper`	INTEGER NOT NULL DEFAULT 0,
	                        PRIMARY KEY(Id)
                        );";
            sqlLiteHelper.RunSQL(sql);
        }
        /// <summary>获取角色列表 
        /// </summary>
        /// <param name="skip">跳过的记录数</param>
        /// <param name="count">获取的数量</param>
        /// <returns></returns>
        public List<RoleEntity> GetRoles(int skip, int pagesize, string RoleName, out int count)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select Id,Name,RightIds,IsSuper from t_roles ");



            if (!string.IsNullOrEmpty(RoleName))
            {
                sql.Append("  where Name=@Name  ");

            }
            if (pagesize != 0)
            {
                sql.Append("  Limit @count Offset @skip ");

            }

            SqlLiteHelper sqliteHelper = new SqlLiteHelper();
            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@count", pagesize), new SQLiteParameter("@skip", skip),
                 new SQLiteParameter("@Name", RoleName)
            };
            DataTable dataTable = sqliteHelper.GetDataTable(sql.ToString(), para);
            List<RoleEntity> roleEntities = new List<RoleEntity>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                roleEntities.Add(Conver2Entity(dataRow));
            }
            count = sqliteHelper.GetCount("t_roles");
            return roleEntities;
        }
        public List<RoleEntity> GetRoles()
        {
            string sql = "select Id,Name,RightIds,IsSuper from t_roles";
            //, count, skip);
            SqlLiteHelper sqliteHelper = new SqlLiteHelper();
            DataTable dataTable = sqliteHelper.GetDataTable(sql);
            List<RoleEntity> roleEntities = new List<RoleEntity>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                roleEntities.Add(Conver2Entity(dataRow));
            }
            return roleEntities;
        }
        /// <summary>获取角色列表  
        /// </summary>
        /// <param name="roleIds">角色id串</param>
        /// <returns></returns>
        public List<RoleEntity> GetRoles(List<int> roleIds)
        {
            string sql = string.Format("select Id,Name,RightIds,IsSuper from t_roles where Id in ({0})", string.Join(",", roleIds));
            DataTable dataTable = new  SqlLiteHelper().GetDataTable(sql);
            List<RoleEntity> roleEntities = new List<RoleEntity>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                roleEntities.Add(Conver2Entity(dataRow));
            }
            return roleEntities;
        }
        /// <summary>转换为实体类 
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private RoleEntity Conver2Entity(DataRow dataRow)
        {
            RoleEntity roleEntity = new RoleEntity();
            roleEntity.Id = Int32.Parse(dataRow["Id"].ToString());
            roleEntity.IsSuper = Int32.Parse(dataRow["IsSuper"].ToString());
            roleEntity.Name = dataRow["Name"].ToString();
            roleEntity.RightIds = dataRow["RightIds"].ToString();
            //roleEntity.Rights = new List<RightEntity>();
            return roleEntity;
        }
        /// <summary>添加角色 
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <returns></returns>
        public bool AddRole(RoleEntity roleEntity)
        {
            string sql = "insert into t_roles (Name,IsSuper) values (@Name,@IsSuper);";//, roleEntity.Name, roleEntity.IsSuper);

            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@Name", roleEntity.Name), new SQLiteParameter("@IsSuper", roleEntity.IsSuper) };
            int result = new  SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>删除角色 
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public bool DeleteRole(int roleId)
        {
            string sql = "delete from t_roles where Id = @Id;";

            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@Id", roleId) };
            int result = new  SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>修改角色信息 
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <returns></returns>
        public bool UpdateRole(RoleEntity roleEntity)
        {
            string sql = "update t_roles set Name = @Name , IsSuper = @issuper where Id = @Id;";
            //roleEntity.Name, roleEntity.RightIds, roleEntity.IsSuper, roleEntity.Id);

            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@Name", roleEntity.Name) ,
                new SQLiteParameter("@issuper", roleEntity.IsSuper) ,
                new SQLiteParameter("@Id", roleEntity.Id) ,
            };
            int result = new  SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>添加权限 
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="rightId">权限Id</param>
        /// <returns></returns>
        public bool AddRight(int roleId, int rightId)
        {
            string sql = "update t_roles set RightIds = Replace(RightIds,','||@rightId||',',',') || @rightId||',' where Id = @roleId";
            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@roleId",roleId) ,
                new SQLiteParameter("@rightId", rightId) ,
            };
            int result = new  SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>移除权限 
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="rightId">权限Id</param>
        /// <returns></returns>
        public bool RemoveRight(int roleId, int rightId)
        {
            string sql = "update t_roles set RightIds = Replace(RightIds,','||@rightId||',',',') where Id = @roleId";
            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@roleId",roleId) ,
                new SQLiteParameter("@rightId", rightId) ,
            };
            int result = new  SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>修改权限
        /// </summary>
        /// <param name="rightId"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public bool UpdateRight(int rightId, string rights)
        {
            if (rights == null)
            {
                return false;
            }
            string[] rightList = rights.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string sql = "update t_roles set RightIds = @RightIds where Id = @rightId;";
            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@rightId",rightId) ,
                new SQLiteParameter("@RightIds", ","+string.Join(",",rightList)+",") ,
            };
            int result = new  SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        /// <summary>移出权限
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        public bool RemoveRight(int rightId)
        {
            string sql = "update t_roles set RightIds = Replace(RightIds,','||@rightId||',',',')";
            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@rightId", rightId) ,
            };
            int result = new  SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
    }
}
