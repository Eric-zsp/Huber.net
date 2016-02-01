using Huber.Framework.Bll;
using Huber.Framework.Entity;
using Huber.SandBonDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Handle
{
    public class HuberPluginHandle
    {
        static List<PluginEntity> _Plugins = new List<PluginEntity>();
        static HashSet<string> AllUrl = new HashSet<string>();
        static Dictionary<string, CAModel> UrlRefAction = new Dictionary<string, CAModel>();
        static PluginBll _pluginBll = new PluginBll();
        static Dictionary<string, SandBoxDynamicLoader> _LoaderDic = new Dictionary<string, SandBoxDynamicLoader>();

        static string _PluginBasePath = "";

        static string _ApplicationBase = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\";
        public static string _AppdomainName = "{0}:{1}";
        private static void AddUrls(SandBoxDynamicLoader loader)
        {
            Dictionary<string, CAModel> Urls = loader.GetAllAction();
            foreach (var url in Urls)
            {
                try
                {
                    AllUrl.Add(url.Value.UrlPath);
                }
                catch (Exception)
                { }
                try
                {
                    UrlRefAction.Add(url.Key, url.Value);
                }
                catch (Exception)
                { }
            }
        }

        public static HashSet<string> GetALlURL()
        {
            return AllUrl;
        }
        public static Dictionary<string, CAModel> GetUrlRefAction()
        {
            return UrlRefAction;
        }
        /// <summary>初始化现有的所有插件
        /// </summary>
        /// <param name="PluginBasePath">插件存放的根目录</param>
        public static void InitPlugin(string PluginBasePath)
        {
            lock (_Plugins)
            {
                _PluginBasePath = PluginBasePath;
                var Plugins = _pluginBll.GetPlugins(0, 0, "");
                if (Plugins != null)
                {
                    Plugins = Plugins.Where(p => p.Status == 1);

                    foreach (var p in Plugins)
                    {
                        LoadPlugin(p);
                    }
                }

            }
        }
        /// <summary>加载组件
        /// </summary>
        /// <param name="plugin"></param>
        public static void LoadPlugin(PluginEntity plugin)
        {
            lock (_Plugins)
            {
                if (!_Plugins.Exists(p => p.Id == plugin.Id))
                {
                    string webPath = _ApplicationBase + plugin.Name + "\\" + plugin.PVersion + "\\";
                    string appPath = webPath + "bin\\";
                    try
                    {
                        System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\Huber.SandBonDriver.dll", appPath + "Huber.SandBonDriver.dll", true);
                        System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Huber.Kernel.dll", appPath + "Huber.Kernel.dll", true);
                    }
                    catch (Exception)
                    {
                    }
                    var loader = new SandBoxDynamicLoader(
                        appPath,
                        string.Format(_AppdomainName, plugin.Name, plugin.PVersion.ToString()),
                        webPath + "Web.config",
                        plugin.Id);
                    loader.LoadAssembly(appPath + plugin.Name + ".dll");
                    _LoaderDic.Add(loader.PluginName, loader);
                    _Plugins.Add(plugin);
                    AddUrls(loader);
                    loader.InvokeMothod("Huber.Kernel.Entity.HuberVariable", "SetCurWebDir", webPath, "/Plugins/" + plugin.Name + "/" + plugin.PVersion);

                }

            }
        }


        /// <summary>卸载组件
        /// </summary>
        /// <param name="plugin"></param>
        public static void UnLoadPlugin(PluginEntity plugin)
        {
            lock (_Plugins)
            {
                if (_Plugins.Exists(p => p.Id == plugin.Id))
                {
                    var loader = getSandBox(plugin.Name, plugin.PVersion);
                    loader.Unload();
                    _LoaderDic.Remove(loader.PluginName);
                    var plist = _Plugins.Where(p => p.Id == plugin.Id);
                    if (plist.Any())
                    {
                        var pplist = plist.ToList();
                        for (int i = 0; i < pplist.Count; i++)
                        {
                            _Plugins.Remove(pplist[i]);
                            break;
                        }
                    }

                }

            }
        }
        /// <summary>根据url获取其所属的子应用程序域
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static SandBoxDynamicLoader getSandBox(string pluginName, int version)
        {
            SandBoxDynamicLoader result = null;
            _LoaderDic.TryGetValue(string.Format(_AppdomainName, pluginName, version), out result);
            return result;
        }


        /// <summary>获取所有可以在菜单中显示的插件
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PluginEntity> getEntityForMenu()
        {
            return _Plugins.Where(p => p.MenuShow == 1 && p.Status == 1);
        }

        /// <summary>url解析成对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isPlugin">是否为插件</param>
        /// <returns></returns>
        public static UrlPathEntity getUrlPathEntity(string url, bool isPlugin)
        {
            UrlPathEntity result = null;
            var matchs = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (isPlugin)
            {
                //var matchs = PluginRgx.Matches(url);
                if (matchs != null && matchs.Length > 0)
                {
                    int _index = 0;
                    result = new UrlPathEntity();
                    result.pluginname = matchs[_index++];//插件名称
                    string _pluginversion = matchs[_index++];//插件版本
                    int pluginversion = -1;
                    int.TryParse(_pluginversion, out pluginversion);
                    result.pluginversion = pluginversion;
                    string urltemp = "/" + result.pluginname;
                    for (; _index < matchs.Length - 1;)
                    {
                        urltemp += "/" + matchs[_index++];
                    }
                    result.action = matchs[_index];//action名称
                    urltemp += "/" + result.action;

                    CAModel controller = null;//控制器名称（包含area）
                    UrlRefAction.TryGetValue(urltemp.ToLower(), out controller);
                    if (controller != null)
                    {
                        result.controller = controller.ControllerName.Replace("/", ".");
                        result.action = controller.ActionName;
                    }
                }
            }
            else
            {
                if (matchs != null && matchs.Length > 0)
                {
                    int _index = 0;
                    result = new UrlPathEntity();
                    result.controller = string.Empty;//控制器名称（包含area）
                    for (; _index < matchs.Length - 1;)
                    {
                        result.controller += "." + matchs[_index++];
                    }
                    result.controller = result.controller.Substring(1);
                    result.action = matchs[_index];//action名称
                }
            }
            return result;
        }

        public static void setShowMenu(int id, int show)
        {
            var plist = _Plugins.Where(p => p.Id == id);
            if (plist != null && plist.Any())
            {
                foreach (var p in plist)
                {
                    p.MenuShow = show;
                }
            }
        }
    }
}
