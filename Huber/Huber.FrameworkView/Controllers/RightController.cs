using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Huber.Kernel.Entity;
using Huber.Framework.Bll;
using Newtonsoft.Json;
using Huber.Framework.Entity;
using Huber.Framework.Handle;
using Huber.FrameworkView.App_Code;

namespace Huber.FrameworkView.Controllers
{
    [CheckSysAdmin()]
    public class RightController : Controller
    {
        //
        // GET: /Right/

        public ActionResult Index()
        {
            //var s = new RightBll().GetAllRights("");
            return View();
        }

        /// <summary>获取权限树
        /// </summary>
        /// <returns></returns>
        public string GetRightTree()
        {
            //List<RightEntity> list = new List<RightEntity>();
            //for (int i = 0; i < 3; i++)
            //{
            //    RightEntity ri = new RightEntity();
            //    list.Add(ri);
            //    ri.Id = i;
            //    ri.IsMenu = 1;
            //    ri.Level = 1;
            //    ri.Name = "test" + i;
            //    ri.ParentId = 0;
            //    for (int j = 0; j < 3; j++)
            //    {
            //        RightEntity ro = new RightEntity();
            //        ri.Children.Add(ro);
            //        ro.Id = i*10+j;
            //        ro.IsMenu = 1;
            //        ro.Level = 2;
            //        ro.Name = "test" + i+"-"+j;
            //        ro.ParentId = i;
            //    }
            //}
            //return Newtonsoft.Json.JsonConvert.SerializeObject(list);

            var result = new RightBll().GetAllRights();
            return JsonConvert.SerializeObject(result);
        }

        public int AddRight(RightEntity rightEntity)
        {
            rightEntity.Describe = HttpUtility.UrlDecode(rightEntity.Describe);
            rightEntity.Name = HttpUtility.UrlDecode(rightEntity.Name);
            return new RightBll().AddRight(rightEntity);
        }

        public ActionResult GetModule()
        {
            PluginBll _PluginBll = new PluginBll();
            List<PluginEntity> pluginEntities = _PluginBll.GetPlugins(0,0,"").ToList();
            RightBll rightBll = new RightBll();
            List<int> list = rightBll.GetCategory();
            if (list != null)
                pluginEntities.RemoveAll(m => list.Contains(m.Id));
            StringBuilder sb = new StringBuilder();
            foreach (var item in pluginEntities)
            {
                sb.AppendFormat("<option value = '{0}'>", item.Id);
                sb.Append(item.Name);
                sb.Append("</option>");
            }
            return Content(sb.ToString());
        }
        /// <summary>修改权限 
        /// </summary>
        /// <param name="rightEntity"></param>
        /// <returns></returns>
        public int EditRight(RightEntity rightEntity)
        {
            rightEntity.Describe = HttpUtility.UrlDecode(rightEntity.Describe);
            rightEntity.Name = HttpUtility.UrlDecode(rightEntity.Name);
            return new RightBll().UpdateRight(rightEntity);
        }
        /// <summary>移出权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool removeRight(int id)
        {
            return new RightBll().DeleteRight(id);
        }

        public ActionResult GetAllAction()
        {
            var result = HuberPluginHandle.GetALlURL();
            
            return Json(result.ToList(),JsonRequestBehavior.AllowGet);
        }
    }
}
