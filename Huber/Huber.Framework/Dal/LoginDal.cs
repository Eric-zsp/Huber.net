using Huber.Framework.DBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Dal
{
    internal class LoginDal
    {
        internal void creatTable(SqlLiteHelper sqlLiteHelper)
        {
            
            string sql = @"CREATE TABLE `t_userLogin` (
	                            `Uid`	TEXT NOT NULL,
	                            `Sign`	TEXT,
	                            `ExtTime`	TEXT
                            );";
            sqlLiteHelper.RunSQL(sql);
        }
        /// <summary>令牌是否有效
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="sign"></param>
        /// <param name="onlyOne"></param>
        /// <returns>1：可以使用   -1：不存在或者已过期   -2：其他用户已在线（该值只有onlyOne=true时有效）</returns>
        public int exsitLoginSign(string uid, string sign, bool onlyOne)
        {
            string sql = "select Uid,Sign,ExtTime,datetime('now') as sqlTime from t_userlogin where Uid=@uid ";
            SQLiteParameter[] para = null;
            if (onlyOne)
            {
                para = new SQLiteParameter[] {
                    new SQLiteParameter("@uid", uid)
                };
            }
            else
            {
                sql += "and Sign=@sign ";
                para = new SQLiteParameter[] {
                    new SQLiteParameter("@uid", uid) ,
                    new SQLiteParameter("@sign", sign)
                };
            }
            DataTable data = new DBHelper.SqlLiteHelper().GetDataTable(sql, para);
            int result = -1;
            if (data != null && data.Rows.Count > 0)
            {
                if (DateTime.Parse(data.Rows[0]["ExtTime"].ToString()) > DateTime.Parse(data.Rows[0]["sqlTime"].ToString()))//令牌未过期
                {
                    if (onlyOne)
                    {
                        if (data.Rows[0]["Sign"].ToString() != sign)
                        {
                            result = -2;
                        }
                    }
                    else
                    {

                        result = 1;
                    }
                }
            }
            return result;
        }

        /// <summary>删除所有过期的令牌
        /// </summary>
        public void DeleteLoginSign()
        {
            string sql = "delete from t_userlogin where datetime(ExtTime)<datetime('now') ";
            new DBHelper.SqlLiteHelper().RunSQL(sql);
        }
        public void delLoginSign(string uid)
        {
            string sql = "delete from t_userlogin where Uid=@uid ";
            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@uid", uid)
            };
            new DBHelper.SqlLiteHelper().RunSQL(sql, para);
        }

        /// <summary>插入令牌
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="sign">令牌</param>
        /// <param name="onlyOne">是否一个账号同时只允许一个用户在线</param>
        public void addLoginsign(string uid, string sign, bool onlyOne, int keepDays = 1)
        {
            string sql = "insert into t_userlogin ( Uid,Sign,ExtTime) values(@uid,@Sign,@ExtTime ) ;";
            if (onlyOne)
            {
                sql += " delete from t_userlogin where Uid=@uid and Sign=@Sign;";
            }
            SQLiteParameter[] para = new SQLiteParameter[] {
                new SQLiteParameter("@uid", uid) ,
                new SQLiteParameter("@Sign", sign) ,
                new SQLiteParameter("@ExtTime", DateTime.Now.AddDays(keepDays).ToString("yyyy-MM-dd HH:mm:ss"))
            };
            int result = new DBHelper.SqlLiteHelper().RunSQL(sql, para);

        }
    }
}
