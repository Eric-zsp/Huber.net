using Huber.Framework.Dal;
using Huber.Framework.Entity;
using Huber.Framework.Handle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Bll
{
    public class PluginBll
    {
        private static readonly PluginDal PluginDao = new PluginDal();

        /// <summary>插件字典
        /// </summary>
        //private static readonly List<PluginDescriptor> PluginDescriptorlist = new List<PluginDescriptor>();

        //private static List<PluginEntity> PluginEntitys;
        /// <summary>初始化插件 
        /// </summary>
        public static void InitPlugins()
        {

            //从数据库中读取所有的已有插件信息
            //try
            //{
            //    PluginEntitys = PluginDao.GetPlugins().Where(m => m.Status == 1).ToList();
            //}
            //catch (Exception exception)
            //{
            //    LogHelper.Log.Debug("获取插件列表失败：" + exception.Message);
            //    return;
            //}
            //foreach (PluginEntity pluginEntity in PluginEntitys)
            //{
            //    StartPlugin(pluginEntity);
            //}
        }


        /// <summary>启动插件 
        /// </summary>
        /// <param name="plugipEntity"></param>
        public void StartPlugins(PluginEntity plugipEntity)
        {
            //PluginDescriptor pluginDescriptor = PluginLoader.Load(plugipEntity);
            //if (pluginDescriptor == null)
            //{
            //    return;
            //}
            //Type[] types = pluginDescriptor.PluginAssembly.GetTypes();
            ////继承的MVC控制器版本必须是程序集 System.Web.Mvc.dll, v4.0.0.0
            //pluginDescriptor.ControllerTypes = types.Where(t => typeof(IController).IsAssignableFrom(t) && t.Name.EndsWith("Controller")).ToArray();

            //RouteValueDictionary routeValueDictionary = new RouteValueDictionary
            //{
            //    {"controller", plugipEntity.DefaultController},
            //    {"action", plugipEntity.DefaultAction},
            //    {"id", UrlParameter.Optional},
            //    {"pluginName", plugipEntity.Name}
            //};
            //Route route = new Route("plugins" + "/" + pluginDescriptor.Plugin.Name + "/{controller}/{action}/{id}", routeValueDictionary, new MvcRouteHandler(ControllerBuilder.Current.GetControllerFactory()));
            //RouteTable.Routes.Insert(0, route);
            //PluginDescriptorlist.Add(pluginDescriptor);
        }



        public IEnumerable<PluginEntity> GetPlugins(int pageIndex, int pageSize, string searchName)
        {
            return new PluginDal().GetPlugins(pageIndex, pageSize, searchName);
        }

        public int GetPlusginsCount(string searchName)
        {

            return new PluginDal().GetPluginsCount(searchName);

        }
        /// <summary>
        /// 修改模块图标
        /// </summary>
        /// <param name="Id">模块id</param>
        /// <param name="Icon">模块图标</param>
        /// <returns>1成功 -1失败</returns>
        public int UpadteModuleIcon(int Id, string Icon)
        {
            int result = new PluginDal().UpadteModuleIcon(Id, Icon);
            return result > 0 ? 1 : -1;
        }

        public PluginEntity GetPlugin(int id)
        {
            return new PluginDal().GetPlugin(id);
        }

        /// <summary>添加插件，暂不启用 
        /// </summary>
        /// <param name="pluginEntity"></param>
        /// <returns>如果该插件已存在则返回null</returns>
        public PluginEntity AddPlugin(PluginEntity pluginEntity)
        {
            lock (PluginDao)
            {
                int result = 0;
                if (!PluginDao.ExsitPlugin(pluginEntity.Name, pluginEntity.PVersion))
                {

                    pluginEntity.MenuShow = 0;
                    pluginEntity.Status = 0;
                    pluginEntity.DefaultAction = string.Empty;
                    pluginEntity.DefaultController = string.Empty;
                    result = PluginDao.AddPlugin(pluginEntity);
                    if (result > 0)
                    {
                        pluginEntity.Id = PluginDao.GetMaxID();
                        return pluginEntity;
                    }
                }
                return null;
            }

        }
        /// <summary>升级插件，新版本暂不启用 
        /// </summary>
        /// <param name="pluginEntity"></param>
        /// <returns>如果该插件已存在则返回null</returns>
        public PluginEntity UpdatePlugin(int id)
        {
            lock (PluginDao)
            {
                var pluginEntity = PluginDao.GetPlugin(id);
                if (pluginEntity != null)
                {
                    int nextVersion = PluginDao.GetPluginMaxID(pluginEntity.Id) + 1;
                    int result = 0;
                    pluginEntity.PVersion = nextVersion;
                    pluginEntity.Status = 0;
                    pluginEntity.MenuShow = 0;

                    result = PluginDao.AddPlugin(pluginEntity);
                    if (result > 0)
                    {
                        pluginEntity.Id = PluginDao.GetMaxID();
                        return pluginEntity;
                    }
                }
                return null;
            }

        }
        /// <summary>启用插件 
        /// </summary>
        /// <param name="id"></param>
        public bool EnablePlugin(int id)
        {
            int result = PluginDao.SetPlugin(id, 1);
            if (result > 0)
            {
                PluginEntity pluginEntity = PluginDao.GetPlugin(id);
                if (pluginEntity != null)
                {
                    HuberPluginHandle.LoadPlugin(pluginEntity);
                }
            }
            return result > 0;
        }
        /// <summary>停用插件 
        /// </summary>
        /// <param name="id"></param>
        public bool DisablePlugin(int id)
        {
            int result = PluginDao.SetPlugin(id, 0);
            if (result > 0)
            {
                PluginEntity pluginEntity = PluginDao.GetPlugin(id);
                if (pluginEntity != null)
                {
                    HuberPluginHandle.UnLoadPlugin(pluginEntity);
                }
            }
            return result > 0;
        }
        /// <summary>删除插件
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DelPlugin(int id)
        {

            int result = PluginDao.DelPlugin(id);
            return result;
        }

        /// <summary>在菜单中显示
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ShowMenu(int id)
        {
            int result = PluginDao.ShowMenu(id, 1);
            if (result > 0)
            {
                HuberPluginHandle.setShowMenu(id, 1);
            }
            return result > 0;
        }
        /// <summary>不在菜单中显示
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CloseMenu(int id)
        {
            int result = PluginDao.ShowMenu(id, 0); if (result > 0)
            {
                HuberPluginHandle.setShowMenu(id, 0);
            }
            return result > 0;
        }
    }
}
