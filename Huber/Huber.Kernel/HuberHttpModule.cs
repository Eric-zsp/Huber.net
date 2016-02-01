using Huber.Kernel.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Huber.Kernel
{
    public class HuberHttpModule : IHttpModule
    {
        internal static Assembly CurDomainAssembly;
        internal static string CurDomainAssemblyName;
        static HuberHttpModule()
        {
            //根据Global.asax 读取插件程序集
            string GlobleAsax = File.ReadAllText(HuberVariable.CurWebDir + "Global.asax");
            if (GlobleAsax.IndexOf("Inherits") > 0)
            {
                CurDomainAssemblyName = GlobleAsax.Substring(GlobleAsax.IndexOf("Inherits") + 10);
                CurDomainAssemblyName = CurDomainAssemblyName.Substring(0, CurDomainAssemblyName.LastIndexOf("."));
                var ass = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var a in ass)
                {
                    if (a.GetName().Name == CurDomainAssemblyName)
                    {
                        CurDomainAssembly = a;
                        CurDomainAssemblyName = CurDomainAssembly.GetName().Name;
                    }
                }

            }
        }
        public void Dispose() { }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(Application_BeginRequest);
        }
        /// <summary>
        /// 从url读取控制器和action名称
        /// </summary>
        /// <param name="url"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        private static void getUrlPathEntity(string url, ref string controller, ref string action)
        {
            var matchs = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (matchs != null && matchs.Length > 1)
            {
                int index = 0;
                controller = CurDomainAssemblyName;
                if (matchs.Length == 3)//包含area
                {
                    controller += "." + matchs[index++];
                }
                controller += ".Controllers." + matchs[index++] + "Controller";
                action = matchs[index];
            }

        }

        private static Plugins GetPluginModel(string url)
        {
            url = url.ToLower();
            if (plugins.ContainsKey(url))
            {
                return plugins[url];
            }
            return null;

        }


        private static void Application_BeginRequest(object sender, EventArgs e)
        {

            HttpApplication application = sender as HttpApplication;
            HttpResponse respond = application.Response;
            HttpRequest request = application.Request;
            string url = request.Url.AbsolutePath.ToString();
            string _action = url.Substring(url.LastIndexOf("/") + 1);
            if (_action.IndexOf(".") < 0)//过滤非action请求
            {
                string controller = string.Empty;
                string action = string.Empty;
                // getUrlPathEntity(url, ref controller, ref action);

                Plugins plugin = GetPluginModel(url);
                if (plugin != null)
                {
                    controller = plugin.ControllerName;
                    action = plugin.ActionName;

                }


                if (controller != string.Empty && action != string.Empty)
                {

                    System.Type t = CurDomainAssembly.GetType(controller, false, true);
                    object dObj = Activator.CreateInstance(t);
                    System.Reflection.MethodInfo method = t.GetMethod(action);




                    RefRequestEntity paras = new RefRequestEntity();
                    //paras.CurPageRights = userRight;//设置本次请求的用户权限
                    #region 获取http参数
                    RequestHandle.FillCorRefEntity(paras, request);
                    #endregion
                    object result = method.Invoke(dObj, new object[] { paras });
                    RequestHandle.ResposeResult(respond, result);
                }
                else
                {
                    //respond.Write(FilterConfig.CODE404);
                }
            }
        }



        private static Dictionary<string, Plugins> plugins = new Dictionary<string, Plugins>();
        public static void InitCurentPlugins()
        {
            List<Type> Types = CurDomainAssembly.GetTypes().ToList().FindAll(p => p.Name.Contains("Controller") && (!p.FullName.ToLower().Contains("areas")));
            foreach (Type t in Types)
            {

                MethodInfo[] mis = t.GetMethods();
                foreach (MethodInfo mi in mis)
                {

                    string url = "/" + t.Name.Replace("Controller", "") + "/" + mi.Name;


                    plugins.Add(url.ToLower(), new Plugins
                    {
                        ActionName = mi.Name,
                        ControllerName = t.FullName,
                        PluginName = CurDomainAssembly.FullName
                    });
                }


            }


        }

    }

    [Serializable]
    public class Plugins
    {
        public string PluginName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }


    }
}
