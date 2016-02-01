using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using Huber.Framework.Bll;
using Huber.Framework.Entity;
using Huber.Kernel.Entity;
using Huber.FrameworkView.App_Code;

namespace Huber.FrameworkView.Controllers
{
    [CheckSysAdmin()]
    public class ModuleController : Controller
    {
        private static readonly PluginBll _PluginBll = new PluginBll();
        //
        // GET: /Module/
        /// <summary>模块管理首页 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>获得模块列表 
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleList(int pageIndex, int pageSize, string searchName)
        {
            List<PluginEntity> pluginEntities;
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            pluginEntities = _PluginBll.GetPlugins(pageIndex, pageSize, searchName).ToList();



            ViewBag.count = _PluginBll.GetPlusginsCount(searchName);
            ViewBag.count = ViewBag.count % pageSize == 0 ? ViewBag.count / pageSize : (ViewBag.count / pageSize + 1);
            ViewBag.pageindex = pageIndex;
            return PartialView(pluginEntities);
        }




        /// <summary>添加模块 
        /// </summary>
        /// <param name="pluginEntity"></param>
        /// <returns></returns>
        public string AddModule(PluginEntity pluginEntity)
        {
            if (Request.Files.Count < 1)
            {
                return "0";
            }
            HttpPostedFileBase fileBase = Request.Files[0];
            if (fileBase == null || !fileBase.FileName.EndsWith(".zip"))
            {
                return "0";
            }
            pluginEntity.Icon = "fa fa-files-o";
            pluginEntity.DefaultAction = string.Empty;
            pluginEntity.DefaultController = string.Empty;
            PluginEntity result = _PluginBll.AddPlugin(pluginEntity);
            if (result != null)
            {
                saveZip(fileBase, result);
                UnZipFile(pluginEntity, fileBase.InputStream);
                return "1";
            }
            return "-2";
        }

        /// <summary>
        /// 修改模块图标
        /// </summary>
        /// <param name="Id">模块id</param>
        /// <param name="Icon">模块图标</param>
        /// <returns>1成功 -1失败</returns>
        public string UpadteModuleIcon(int Id, string Icon)
        {
            if (Id == 0||string.IsNullOrEmpty(Icon))
                return "-1";

            int Result = _PluginBll.UpadteModuleIcon(Id, Icon);

            return Result.ToString();

        }


        /// <summary>设置插件状态 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult SetStatus(int id, int status)
        {
            bool result = false;
            if (status == 0)
            {
                result = _PluginBll.DisablePlugin(id);
            }
            else
            {
                result = _PluginBll.EnablePlugin(id);
            }
            return result ? Json(1) : Json(0);
        }

        /// <summary>设置插件状态 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult SetMenuShow(int id, int status)
        {
            bool result = false;
            if (status == 0)
            {
                result = _PluginBll.CloseMenu(id);
            }
            else
            {
                result = _PluginBll.ShowMenu(id);
            }
            return result ? Json(1) : Json(0);
        }

        /// <summary>删除模块 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DelModule(int id)
        {
            var entity = _PluginBll.GetPlugin(id);
            if (entity.Status == 1)
            {
                _PluginBll.DisablePlugin(id);
            }
            int result = _PluginBll.DelPlugin(id);
            if (entity.MenuShow == 1)
            {
                RightBll rightBll = new RightBll();
                List<RightEntity> rightEntities = rightBll.GetRights(id);
                rightBll.DeleteRightRel(rightEntities.Select(m => m.Id).ToList());
            }
            return Json(result);
        }

        /// <summary>升级模块 
        /// </summary>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        public string UpdateModule(int pluginId)
        {
            if (Request.Files.Count < 1)
            {
                return "0";
            }
            HttpPostedFileBase fileBase = Request.Files[0];
            if (fileBase == null || !fileBase.FileName.EndsWith(".zip"))
            {
                return "0";
            }
            PluginEntity pluginEntity = _PluginBll.UpdatePlugin(pluginId);
            if (pluginEntity != null)
            {

                saveZip(fileBase, pluginEntity);
                UnZipFile(pluginEntity, fileBase.InputStream);
                return "1";
            }

            return "-2";
        }



        #region Private
        /// <summary>解压 
        /// </summary>
        /// <param name="zipName"></param>
        /// <param name="baseStream"></param>
        private static void UnZipFile(PluginEntity pluginEntity, Stream baseStream)
        {
            using (ZipInputStream s = new ZipInputStream(baseStream))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\" + pluginEntity.Name + "\\" + pluginEntity.PVersion + "\\";
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = baseDir + Path.GetDirectoryName(theEntry.Name) + "\\";
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = System.IO.File.Create(directoryName + fileName))
                        {
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                var size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                }


            }
        }

        private static void saveZip(HttpPostedFileBase fileBase, PluginEntity pluginEntity)
        {
            string dirName = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\Plugin_updates_temp\\";
            string zipName = dirName + pluginEntity.Name + "." + pluginEntity.PVersion + ".zip";
            if (!Directory.Exists(dirName))
            {
                lock (_PluginBll)
                {
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                }
            }

            fileBase.SaveAs(zipName);//保存文件

        }
        #endregion

    }
}
