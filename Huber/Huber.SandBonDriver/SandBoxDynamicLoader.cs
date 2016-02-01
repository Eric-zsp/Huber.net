using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Huber.SandBonDriver
{
    /// <summary>沙箱启动器
    public class SandBoxDynamicLoader
    {
        /// <summary>沙箱对应的应用程序域
        /// </summary>
        private AppDomain appDomain;
        /// <summary>沙箱管道
        /// </summary>
        private SandBoxChannel channelChannel;
        /// <summary>程序域的ID
        /// </summary>
        public int AppDomainID { get; set; }
        /// <summary>插件名称
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>构造
        /// </summary>
        /// <param name="ApplicationBase">插件的所在的目录（bin目录）</param>
        /// <param name="_PluginName">插件名称</param>
        /// <param name="configPath">config文件位置</param>
        /// <param name="_AppDomainID">域标识（唯一）</param>
        public SandBoxDynamicLoader(string ApplicationBase, string _PluginName, string configPath, int _AppDomainID)
        {
            PluginName = _PluginName;
            AppDomainID = _AppDomainID;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = ApplicationBase;
            DirectoryInfo di=new DirectoryInfo(ApplicationBase);
            if (configPath != string.Empty)
            {
                setup.ConfigurationFile = configPath;
            }
            setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "private");
            setup.CachePath = setup.ApplicationBase;
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = setup.ApplicationBase+"\\SandBoxRunShadow";
            AppDomain.CurrentDomain.SetShadowCopyFiles();
            this.appDomain = AppDomain.CreateDomain(PluginName, null, setup);
            this.appDomain.SetData("APP_CONFIG_FILE", configPath);
            
            String name = Assembly.GetExecutingAssembly().GetName().FullName;
            try
            {
                this.channelChannel = (SandBoxChannel)this.appDomain.CreateInstanceAndUnwrap(name, typeof(SandBoxChannel).FullName);
            }
            catch (Exception ex)
            {
 
            }
            
        }

        /// <summary>加载程序集
        /// </summary>
        /// <param name="assemblyFile"></param>
        public void LoadAssembly(string assemblyFile)
        {
            channelChannel.LoadAssembly(assemblyFile);
        }

        /// <summary>获取当前模块内所有action
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, CAModel> GetAllAction()
        {
            if (channelChannel == null) return null;
            return channelChannel.GetAllAction();
        }
        /// <summary>方法调用
        /// </summary>
        /// <param name="typeName">类名称（全名称）</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public object InvokeMothod(string typeName, string methodName, params object[] args)
        {
            return channelChannel.InvokeMothod(typeName, methodName, args);
        }
        /// <summary>卸载
        /// </summary>
        public void Unload()
        {
            try
            {
                if (appDomain == null) return;
                AppDomain.Unload(this.appDomain);
                this.appDomain = null;
            }
            catch (CannotUnloadAppDomainException ex)
            {
                throw ex;
            }
        }
    }
}
