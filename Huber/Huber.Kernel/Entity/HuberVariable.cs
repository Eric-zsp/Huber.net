using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel.Entity
{
    public class HuberVariable
    {
        /// <summary>当前模块（web站点文件）根目录
        /// </summary>
        public static string CurWebDir { get; set; }
        /// <summary>当前模块（web站点url）根路径
        /// </summary>
        public static string CurWebUrl { get; set; }
        static HuberVariable()
        {
            CurWebDir = AppDomain.CurrentDomain.BaseDirectory;
            CurWebUrl = "";
        }
        /// <summary>模块作为应用程序域启动时，设置该模块的路径
        /// </summary>
        public void SetCurWebDir(string webDirPath, string webUrl)
        {
            CurWebDir = webDirPath;
            CurWebUrl = webUrl;
        }
    }
}
