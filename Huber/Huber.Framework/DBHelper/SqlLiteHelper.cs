using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace Huber.Framework.DBHelper
{

    public class SqlLiteHelper
    {
        static bool useInMemory = false; //数据库是否为内存数据库
        static SQLiteConnection memoryConn = null;
        /// <summary>物理数据库连接串
        /// </summary>
        static string strFileCon = ConfigurationManager.ConnectionStrings["Huber_data_sqlite_file"].ConnectionString.Replace("|Path|", AppDomain.CurrentDomain.BaseDirectory);
        /// <summary>内存模式连接串
        /// </summary>
        static string strMemCon = "Data Source=:memory:;Version=3;New=True;";


        /// <summary>获得一个数据库连接
        /// </summary>
        /// <returns></returns>
        private SQLiteConnection GetCon()
        {
            return GetCon(useInMemory);
        }

        /// <summary>获得一个数据库连接
        /// </summary>
        /// <param name="_useInMemory">数据库是否为内存数据库 true:内存，false：文件</param>
        /// <returns></returns>
        private SQLiteConnection GetCon(bool _useInMemory = false)
        {
            string strFileCon = ConfigurationManager.ConnectionStrings["Huber_data_sqlite_file"].ConnectionString.Replace("|Path|", AppDomain.CurrentDomain.BaseDirectory);
            //string strMemCon = ":memory:;Pooling=true;FailIfMissing=false";

            if (_useInMemory)
            {
                return memoryConn == null ? new SQLiteConnection(strMemCon) : memoryConn;
            }
            else
            {
                SQLiteConnection sqliteCon = new SQLiteConnection(strFileCon);
                return sqliteCon;

            }

        }

        public bool InitDb()
        {
            //strFileCon
            var connectStrTemp = strFileCon.Split(';');
            if (connectStrTemp != null)
            {
                string DBFilePath = string.Empty;
                foreach (var a in connectStrTemp)
                {
                    if (a.StartsWith("Data Source"))
                    {
                        DBFilePath = a;
                        DBFilePath = DBFilePath.Substring(11).Trim().Substring(1).Trim();
                        break;
                    }
                }
                if (DBFilePath != string.Empty)
                {
                    if (!File.Exists(DBFilePath))
                    {
                        string dir = DBFilePath.Substring(0, DBFilePath.LastIndexOf("\\"));
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        SQLiteConnection.CreateFile(DBFilePath);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>  数据库操作类
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int RunSQL(string sql, params SQLiteParameter[] paras)
        {
            int result = 0;
            if (useInMemory)
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(sql, memoryConn);
                    if (paras != null)
                    {
                        cmd.Parameters.AddRange(paras);
                    }
                    result = cmd.ExecuteNonQuery();
                    new Task(() => {
                        DumpDatabase();
                    }).Start();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                using (SQLiteConnection conn = GetCon())
                {
                    try
                    {
                        conn.Open();
                        SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                        if (paras != null)
                        {
                            cmd.Parameters.AddRange(paras);
                        }
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }

            return result;
        }



        /// <summary> 获得datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params SQLiteParameter[] paras)
        {
            DataSet ds = null;
            DataTable dt = null;
            if (useInMemory)
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(sql, memoryConn);
                    if (paras != null)
                    {
                        cmd.Parameters.AddRange(paras);
                    }
                    using (SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd))
                    {
                        ds = new DataSet();
                        sda.Fill(ds);//将结果填充到ds中
                        dt = ds.Tables[0];
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                using (SQLiteConnection conn = GetCon())
                {
                    try
                    {
                        SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                        if (paras != null)
                        {
                            cmd.Parameters.AddRange(paras);
                        }
                        using (SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd))
                        {
                            ds = new DataSet();
                            sda.Fill(ds);//将结果填充到ds中
                            dt = ds.Tables[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }


            return dt;
        }

        /// <summary> 返回记录总条数
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public int GetCount(string strTableName)
        {
            string strSql = "select count(*) from " + strTableName;
            int count = 0;
            DataTable dtCount = GetDataTable(strSql);
            count = int.Parse(dtCount.Rows[0][0].ToString());
            return count;
        }

        /// <summary>内存中的数据dump到磁盘
        /// </summary>
        public void DumpDatabase()
        {

            new SqlLiteHelper().BackupDatabase(false);
        }
        /// <summary>磁盘中的数据加载到内存
        /// </summary>
        public void LoadDatabaseToMemory()
        {

            BackupDatabase(true);
        }
        #region 备份数据库


        /// <summary>备份数据库
        /// </summary>
        /// <param name="isFileToMemory">是否是文件数据库备份到内存数据库；
        /// isFileToMemory为true指的是从文件数据库导入到当前内存数据库；
        /// isFileToMemory为false指的是从当前内存数据库导出到文件数据库。
        /// </param>
        private void BackupDatabase(bool isFileToMemory)
        {
            using (SQLiteConnection dbfileConnection = GetCon(false))
            {
                memoryConn = GetCon(true);
                //如果连接是关闭状态就打开
                if (dbfileConnection.State == ConnectionState.Closed)
                {
                    dbfileConnection.Open();
                }
                if (memoryConn.State == ConnectionState.Closed)
                {
                    memoryConn.Open();
                }
                if (isFileToMemory)
                {
                    dbfileConnection.BackupDatabase(memoryConn, "main", "main", -1, null, 0);
                    //memoryConn = dbmemConnection;

                    useInMemory = true;
                }
                else
                {
                    memoryConn.BackupDatabase(dbfileConnection, "main", "main", -1, null, 0);
                }
            }

        }

        #endregion
    }
}
