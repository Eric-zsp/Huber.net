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
    internal class PluginDal
    {

        internal void creatTable(SqlLiteHelper sqlLiteHelper)
        {
            //Id,Name,Describe,Status,Author,DefaultController,DefaultAction,PVersion,MenuShow,Icon
                string sql = @"CREATE TABLE `t_plugin` (
	                            `Id`	INTEGER NOT NULL,
	                            `Name`	TEXT NOT NULL,
	                            `Status`	INTEGER NOT NULL DEFAULT 0,
	                            `Describe`	TEXT NOT NULL,
	                            `Author`	TEXT NOT NULL,
	                            `DefaultController`	TEXT NOT NULL,
	                            `DefaultAction`	TEXT NOT NULL,
	                            `PVersion`	INTEGER,
	                            `MenuShow`	INTEGER,
	                            `Icon`	TEXT,
	                            PRIMARY KEY(Id)
                            );";
            sqlLiteHelper.RunSQL(sql);
        }
        /// <summary>获得所有插件（包括被禁用的） 
        /// </summary>
        public IEnumerable<PluginEntity> GetPlugins(int pageIndex, int pageSize, string searchName)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select Id,Name,Describe,Status,Author,DefaultController,DefaultAction,PVersion,MenuShow,Icon  from t_plugin");
            if (!string.IsNullOrEmpty(searchName))
            {

                sql.Append("  where Name=@Name  ");
            }
            if (pageSize != 0)
            {
                sql.Append("   Limit @pageSize");
            }
            if (pageSize != 0 && pageIndex != 0)
            {
                sql.Append("  offset  (@pageSize*(@pageIndex-1))");
            }


            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@pageSize", pageSize),
                new SQLiteParameter("@pageIndex", pageIndex),
                  new SQLiteParameter("@Name", searchName)
            };
            SqlLiteHelper sqlLiteHelper = new SqlLiteHelper();
            DataTable dataTable = sqlLiteHelper.GetDataTable(sql.ToString(), para);
            return (from DataRow dataRow in dataTable.Rows select Convert2Entity(dataRow)).ToList();
        }

        public int GetPluginsCount(string searchName)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select count(*) as c from t_plugin");

            if (!string.IsNullOrEmpty(searchName))
            {
                sql.Append(" where Name=@Name");

            }

            SQLiteParameter[] para = new SQLiteParameter[]
            {
              new SQLiteParameter("@Name", searchName)
            };

            SqlLiteHelper sqlLiteHelper = new SqlLiteHelper();
            DataTable dataTable = sqlLiteHelper.GetDataTable(sql.ToString(), para);

            return int.Parse(dataTable.Rows[0]["c"].ToString());
        }

        /// <summary>获得插件 
        /// </summary>
        /// <returns></returns>
        public PluginEntity GetPlugin(int id)
        {
            string sql = "select  Id,Name,Describe,Status,Author,DefaultController,DefaultAction,PVersion,MenuShow,Icon from t_plugin where Id = @Id";
            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@Id", id)
            };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            if (dataTable != null && dataTable.Rows.Count == 1)
            {
                return Convert2Entity(dataTable.Rows[0]);
            }
            return null;
        }
        /// <summary>获得插件 
        /// </summary>
        /// <returns></returns>
        public bool ExsitPlugin(string Name, int Version)
        {
            string sql = "select  Id from t_plugin where Name = @Name and PVersion=@PVersion";
            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@PVersion", Version),
                new SQLiteParameter("@Name", Name)
            };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>将row转换为实体类 
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private static PluginEntity Convert2Entity(DataRow dataRow)
        {
            PluginEntity pluginEntity = new PluginEntity();
            pluginEntity.Id = Int32.Parse(dataRow["Id"].ToString());
            pluginEntity.Name = dataRow["Name"].ToString();
            pluginEntity.Status = Int32.Parse(dataRow["Status"].ToString());
            pluginEntity.Describe = dataRow["Describe"].ToString();
            pluginEntity.DefaultAction = dataRow["DefaultAction"].ToString();
            pluginEntity.DefaultController = dataRow["DefaultController"].ToString();
            pluginEntity.Author = dataRow["Author"].ToString();
            pluginEntity.PVersion = int.Parse(dataRow["PVersion"].ToString());
            pluginEntity.MenuShow = int.Parse(dataRow["MenuShow"].ToString());
            pluginEntity.Icon = dataRow["Icon"].ToString();
            return pluginEntity;
        }
        /// <summary>添加插件（不启动） 
        /// </summary>
        public int AddPlugin(PluginEntity pluginDescriptor)
        {
            const string sql = "insert into t_plugin (Name,Describe,Author,DefaultController,DefaultAction,PVersion,MenuShow,Icon) values (@Name,@Describe,@Author,@DefaultController,@DefaultAction,@PVersion,@MenuShow,@Icon)";
            int result = 0;
            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@Name", pluginDescriptor.Name),
                new SQLiteParameter("@Describe", pluginDescriptor.Describe),
                new SQLiteParameter("@Author", pluginDescriptor.Author),
                new SQLiteParameter("@DefaultController", pluginDescriptor.DefaultController),
                new SQLiteParameter("@DefaultAction", pluginDescriptor.DefaultAction),
                new SQLiteParameter("@PVersion", pluginDescriptor.PVersion),
                new SQLiteParameter("@MenuShow", pluginDescriptor.MenuShow),
                new SQLiteParameter("@Icon", pluginDescriptor.Icon)
            };
            try
            {
                result = new SqlLiteHelper().RunSQL(sql, para);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        /// <summary>获取最大的id
        /// </summary>
        /// <returns></returns>
        public int GetMaxID()
        {
            const string sql = "select  max(Id) from t_plugin";
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return int.Parse(dataTable.Rows[0][0].ToString());
            }
            return 1;
        }
        /// <summary>计算插件的当前最大版本
        /// </summary>
        /// <returns></returns>
        public int GetPluginMaxID(int id)
        {
            const string sql = "select  PVersion from t_plugin where Id=@Id";
            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@Id", id)
            };
            DataTable dataTable = new SqlLiteHelper().GetDataTable(sql, para);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return int.Parse(dataTable.Rows[0][0].ToString());
            }
            return 0;
        }
        /// <summary>设置插件状态 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int SetPlugin(int id, int status)
        {
            string sql = "update t_plugin set Status = @Status where Id = @Id";
            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@Status", status),
                new SQLiteParameter("@Id", id)
            };
            return new SqlLiteHelper().RunSQL(sql, para);
        }
        /// <summary>删除插件 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DelPlugin(int id)
        {
            const string sql = "delete  from t_plugin where Id = @Id and Status=0;";
            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@Id", id)
            };
            return new SqlLiteHelper().RunSQL(sql, para);
        }
        /// <summary>设置是否显示在菜单中 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int ShowMenu(int id, int MenuShow)
        {

            string sql = "update t_plugin set MenuShow = @MenuShow where Id = @Id";
            SQLiteParameter[] para = new SQLiteParameter[]
            {
                new SQLiteParameter("@MenuShow", MenuShow),
                new SQLiteParameter("@Id", id)
            };
            return new SqlLiteHelper().RunSQL(sql, para);
        }

        /// <summary>
        /// 修改模块图标
        /// </summary>
        /// <param name="Id">模块id</param>
        /// <param name="Icon">模块图标</param>
        /// <returns></returns>
        internal int UpadteModuleIcon(int Id, string Icon)
        {
            string sql = "update t_plugin set Icon = @Icon where Id = @Id";
            SQLiteParameter[] param = new SQLiteParameter[] {
             new SQLiteParameter("@Icon",Icon),
             new SQLiteParameter("@Id",Id)
            };
            return new SqlLiteHelper().RunSQL(sql, param);
        }
    }
}
