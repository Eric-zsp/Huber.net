using System.Collections.Generic;
using System.Web.Mvc;
using Huber.Framework.Entity;
using Huber.Framework.Bll;
using Newtonsoft.Json;
using Huber.FrameworkView.App_Code;

namespace Huber.FrameworkView.Controllers
{
    [CheckSysAdmin()]
    public class RoleController : Controller
    {
        //
        // GET: /Role/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>角色列表 
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public ActionResult RoleList(int pageIndex, int pageSize, string callBack, string RoleName)
        {
            int count = 0;
            RoleBll roleBll = new RoleBll();
            List<RoleEntity> roleEntities = roleBll.GetRoles(pageIndex, pageSize, RoleName, out count);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageCount = count % pageSize == 0 ? count / pageSize : (count / pageSize) + 1;
            ViewBag.callback = callBack;
            return PartialView(roleEntities);
        }
        /// <summary>添加角色 
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="isSuper">是否为管理员</param>
        /// <returns></returns>
        public ActionResult AddRole(string name, int isSuper)
        {
            RoleBll roleBll = new RoleBll();
            bool result = roleBll.AddRole(new RoleEntity()
             {
                 Name = name,
                 IsSuper = isSuper
             });
            return Json(result);
        }
        /// <summary>删除角色 
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public ActionResult DeleteRole(int roleId)
        {
            RoleBll roleBll = new RoleBll();
            bool result = roleBll.DeleteRole(roleId);
            return Json(result);
        }
        /// <summary>修改角色 
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <returns></returns>
        public ActionResult UpdateRole(RoleEntity roleEntity)
        {
            RoleBll roleBll = new RoleBll();
            bool result = roleBll.UpdateRole(roleEntity);
            return Json(result);
        }

        public ActionResult GetRights()
        {
            RightBll rightBll = new RightBll();
            return Content(JsonConvert.SerializeObject( rightBll.GetAllRights()));
        }

        public ActionResult UpdateRight(int rightId, string rights)
        {
            RoleBll roleBll = new RoleBll();
            bool result = roleBll.UpdateRight(rightId, rights);
            return Json(result);
        }
    }
}
