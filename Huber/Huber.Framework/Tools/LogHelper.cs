using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Tools
{
    /// <summary>日志操作类
    /// 使用本类必须保证配置节上存在log4net的配置（web/app.config中）
    /// </summary>  
    public class LogHelper
    {
        private static ILog log;
        /// <summary>日志记录实体类
        /// </summary>
        public static ILog Log
        {
            get { return LogHelper.log; }
            set { LogHelper.log = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configPath"></param>
        static LogHelper()
        {

            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log4net.Config.XmlConfigurator.Configure();

        }
        /// <summary>加载日志记录配置
        /// </summary>
        /// <param name="configFilePath"></param>
        public static void Config(string configFilePath)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(configFilePath));
        }



        private static Dictionary<string, ILog> logbyConfig;
        //private static ILog logbyConfig;
        static object logbyConfiglock = new object();
        /// <summary>根据配置的log名称获取Log实例
        /// </summary>
        /// <param name="logname">log配置节点名称</param>
        /// <returns></returns>
        /// <remarks>每次调用都会创建一个实例，使用时考虑重复利用问题</remarks>
        public static ILog getLogByConfigLogName(string logname)
        {
            if (logbyConfig == null)
            {
                lock (logbyConfiglock)
                {
                    logbyConfig = new Dictionary<string, ILog>();
                }
            }
            if (!logbyConfig.ContainsKey(logname))
            {
                lock (logbyConfiglock)
                {
                    if (!logbyConfig.ContainsKey(logname))
                    {
                        ILog log = log4net.LogManager.GetLogger(logname);
                        logbyConfig.Add(logname, log);
                    }
                }
            }

            return logbyConfig[logname];
        }

       
    }
}
