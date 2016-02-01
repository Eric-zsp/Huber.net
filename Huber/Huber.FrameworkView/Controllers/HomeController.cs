using Huber.Framework.Bll;
using Huber.Framework.Entity;
using Huber.Framework.Handle;
using Huber.Kernel.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Huber.FrameworkView.Controllers
{

    public class HomeController : Controller
    {
        
       
        // GET: /Home/

        public ActionResult Index()
        {
            UserEntity user = new UserBll().getCurUser();
            ViewBag.User = user;
            #region 菜单呈现
            List<string> urights = new List<string>();
            List<MenuEntity> menus = new List<MenuEntity>();
            if (user != null)
            {

                IEnumerable<PluginEntity> pluginEntities = HuberPluginHandle.getEntityForMenu();
                if (pluginEntities.Any())
                {
                    #region 用户的权限
                    List<RoleEntity> uroles = new RoleBll().GetRoles(user.RolesIds);
                    string[] rightRange = null;
                    string[] splitchar = new string[] { "," };
                    if (uroles != null)
                        foreach (RoleEntity role in uroles)
                        {
                            rightRange = role.RightIds.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
                            if (rightRange != null)
                            {
                                urights.AddRange(rightRange);
                            }
                        }
                    #endregion
                    List<RightEntity> rightEntities = new RightBll().GetAllMenu(urights, user.Uid == UserBll.SuperAdminID);
                    foreach (PluginEntity pluginEntity in pluginEntities)
                    {
                        RightEntity rightEntity = rightEntities.SingleOrDefault(m => m.Category == pluginEntity.Id && m.ParentId == 0);
                        if (rightEntity == null)
                            continue;
                        MenuEntity menuEntity = GetMenu(rightEntity, pluginEntity);
                        menus.Add(menuEntity);
                    }
                }
                #region 添加系统菜单
                if (user.Uid == UserBll.SuperAdminID)
                {
                    MenuEntity CoreMenu = new MenuEntity(-1, "系统管理", string.Empty, true, "fa fa-cog");
                    CoreMenu.Children.Add(new MenuEntity(-1, "用户管理", "/user/index", true));
                    CoreMenu.Children.Add(new MenuEntity(-1, "权限管理", "/right/index", true));
                    CoreMenu.Children.Add(new MenuEntity(-1, "角色管理", "/role/index", true));
                    CoreMenu.Children.Add(new MenuEntity(-1, "模块管理", "/module/index", true));
                    menus.Add(CoreMenu);
                    //menus.Add(CoreMenu);
                    //menus.Add(CoreMenu);
                    //menus.Add(CoreMenu);
                    //menus.Add(CoreMenu);
                }
                #endregion
            }
            #endregion



            ViewBag.Menus = menus;
            return View();
        }

        private MenuEntity GetMenu(RightEntity rightEntity, PluginEntity pluginEntity)
        {
            string startWith = "/" + pluginEntity.Name + "/" ;
            string replace = "/" + pluginEntity.Name + "/" +pluginEntity.PVersion + "/";

            MenuEntity mm = new MenuEntity(rightEntity.Id, rightEntity.Name, "/plugins" + rightEntity.Url.Replace(startWith, replace), false, pluginEntity.Icon);
            foreach (var item in rightEntity.Children)
            {
                mm.Children.Add(GetMenu(item, pluginEntity));
            }
            return mm;
        }

    }
}
