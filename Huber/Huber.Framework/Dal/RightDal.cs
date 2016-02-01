using Huber.Framework.DBHelper;
using Huber.Kernel.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Dal
{
    internal class RightDal
    {
        internal void creatTable(SqlLiteHelper sqlLiteHelper)
        {
            //Id,Name,Describe,Status,Author,DefaultController,DefaultAction,PVersion,MenuShow,Icon
                string sql = @"CREATE TABLE `t_rights` (
	                            `Id`	INTEGER NOT NULL,
	                            `Name`	TEXT NOT NULL,
	                            `Url`	TEXT NOT NULL UNIQUE,
	                            `Level`	INTEGER NOT NULL,
	                            `ParentId`	INTEGER NOT NULL,
	                            `Describe`	TEXT NOT NULL,
	                            `IsMenu`	INTEGER NOT NULL DEFAULT 0,
	                            `Deleted`	INTEGER NOT NULL DEFAULT 0,
	                            `Category`	INTEGER NOT NULL,
	                            PRIMARY KEY(Id)
                            );";
            sqlLiteHelper.RunSQL(sql);
        }
        /// <summary>获取权限列表 
        /// </summary>
        /// <returns></returns>
        public List<RightEntity> GetAllRights()
        {
            const string sql = "select Id,Name,Url,Level,ParentId,Describe,IsMenu,Deleted,Category from t_rights where Deleted=0";
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql);
            List<RightEntity> list = new List<RightEntity>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                list.Add(Conver2Entity(dataRow));
            }
            return list;
        }
        /// <summary>获得权限列表
        /// </summary>
        /// <param name="category">类别，插件Id</param>
        /// <returns></returns>
        public List<RightEntity> GetRights(int category)
        {
            const string sql = "select Id,Name,Url,Level,ParentId,Describe,IsMenu,Deleted,Category from t_rights where Category = @category;";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@category", category) };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            List<RightEntity> list = new List<RightEntity>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                list.Add(Conver2Entity(dataRow));
            }
            return list;
        }
        public List<RightEntity> GetRights(string rightIds)
        {
            string sql =
                string.Format(
                    "select Id,Name,Url,Level,ParentId,Describe,IsMenu,Deleted from t_rights where Id in ({0})",
                    rightIds);
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql);
            List<RightEntity> list = new List<RightEntity>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                list.Add(Conver2Entity(dataRow));
            }
            return list;
        }

        /// <summary>转换成实体类  
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private RightEntity Conver2Entity(DataRow dataRow)
        {
            RightEntity rightEntity = new RightEntity();
            rightEntity.Id = Int32.Parse(dataRow["Id"].ToString());
            rightEntity.Name = (string)dataRow["Name"];
            rightEntity.Url = (string)dataRow["Url"];
            rightEntity.Level = Int32.Parse(dataRow["Level"].ToString());
            rightEntity.ParentId = Int32.Parse(dataRow["ParentId"].ToString());
            rightEntity.Describe = (string)dataRow["Describe"];
            rightEntity.IsMenu = Int32.Parse(dataRow["IsMenu"].ToString());
            rightEntity.Deleted = Int32.Parse(dataRow["Deleted"].ToString());
            rightEntity.Category = Int32.Parse(dataRow["Category"].ToString());
            return rightEntity;
        }

        /// <summary>添加一个节点 
        /// </summary>
        /// <param name="rightEntity"></param>
        /// <returns></returns>
        public int AddRight(RightEntity rightEntity)
        {
            string sql = "insert into t_rights (Name,Url,Level,ParentId,Describe,IsMenu,Deleted,Category) values (@Name,@Url,@Level,@ParentId,@Describe,@IsMenu,@Deleted,@Category);select last_insert_rowid();";
            SQLiteParameter[] para = new SQLiteParameter[]{
                    new SQLiteParameter("@Name",rightEntity.Name),
                    new SQLiteParameter("@Url", rightEntity.Url),
                    new SQLiteParameter("@Level", rightEntity.Level),
                    new SQLiteParameter("@ParentId", rightEntity.ParentId),
                    new SQLiteParameter("@Describe",rightEntity.Describe),
                    new SQLiteParameter("@IsMenu",rightEntity.IsMenu),
                    new SQLiteParameter("@Deleted",rightEntity.Deleted),
                    new SQLiteParameter("@Category",rightEntity.Category)
                };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            if (dataTable != null && dataTable.Rows.Count == 1)
            {
                return Int32.Parse(dataTable.Rows[0][0].ToString());
            }
            return -1;
        }
        public int Exist(string url)
        {
            string sql = "select Id from t_rights where Url=@url and Deleted=0";
            SQLiteParameter[] para = new SQLiteParameter[] { new SQLiteParameter("@url", url) };
            DataTable result = new SqlLiteHelper().GetDataTable(sql, para);
            int id = -1;
            if (result != null && result.Rows.Count == 1)
            {
                id = Int32.Parse(result.Rows[0]["Id"].ToString());
            }
            return id;
        }
        /// <summary>删除权限，假山
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        public bool DeleteRight(List<int> rightId)
        {
            string rightIds = string.Join(",", rightId);
            string sql = string.Format("update t_rights set Deleted=1 where Id in ({0})", rightIds);
            int result = new DBHelper.SqlLiteHelper().RunSQL(sql);
            return result > 0;
        }

        /// <summary>删除权限，真删
        /// </summary>
        /// <param name="rightIds"></param>
        /// <returns></returns>
        public bool DeleteRightRel(List<int> rightIds)
        {
            string sql = "delete from t_rights where Id in (@rightId)";
            SQLiteParameter[] para = new SQLiteParameter[]{
                    new SQLiteParameter("@rightId",string.Join(",",rightIds))
                };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;

        }
        /// <summary>修改权限 
        /// </summary>
        /// <param name="rightEntity"></param>
        /// <returns></returns>
        public bool UpdateRight(RightEntity rightEntity)
        {
            const string sql = "update t_rights set Name=@Name,Url=@Url,Level=@Level,ParentId=@ParentId,Describe=@Describe,IsMenu=@IsMenu,Deleted=@Deleted where Id=@Id";
            SQLiteParameter[] para = new SQLiteParameter[]{
                    new SQLiteParameter("@Name",rightEntity.Name),
                    new SQLiteParameter("@Url", rightEntity.Url),
                    new SQLiteParameter("@Level", rightEntity.Level),
                    new SQLiteParameter("@ParentId", rightEntity.ParentId),
                    new SQLiteParameter("@Describe",rightEntity.Describe),
                    new SQLiteParameter("@IsMenu",rightEntity.IsMenu),
                    new SQLiteParameter("@Deleted",rightEntity.Deleted),
                    new SQLiteParameter("@Id",rightEntity.Id)
                };
            int result = new SqlLiteHelper().RunSQL(sql, para);
            return result > 0;
        }
        public List<RightEntity> UserGetRights(string url)
        {
            string sql = "select Id,Name,Url,Level,ParentId,Describe,IsMenu,Deleted  from t_rights where Id=(select Id from t_rights where Url=@url) and Deleted=0";
            SQLiteParameter[] para = new SQLiteParameter[]{
                    new SQLiteParameter("@url", url)
                };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            List<RightEntity> list = new List<RightEntity>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                list.Add(Conver2Entity(dataRow));
            }
            return list;
        }
        /// <summary>获得所有已经启用的模块Id
        /// </summary>
        /// <returns></returns>
        public List<int> GetCategory()
        {
            string sql = "select Category from t_rights where ParentId = 0";
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql);
            List<int> list = null;
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                list = new List<int>();
                foreach (DataRow row in dataTable.Rows)
                {
                    list.Add(Int32.Parse(row["Category"].ToString()));
                }
            }
            return list;
        }
    }
}
