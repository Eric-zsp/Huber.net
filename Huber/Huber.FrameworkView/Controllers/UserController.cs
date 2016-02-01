using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using Huber.Framework.Entity;
using Huber.Framework.Bll;
using Huber.FrameworkView.App_Code;

namespace Huber.FrameworkView.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        [CheckSysAdmin()]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>获取用户列表（分页）
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [CheckSysAdmin()]
        public PartialViewResult UserList(int pageindex, int pagesize, string callback, string userName)
        {
            UserBll userBll = new UserBll();
            int count = 0;
            List<UserEntity> modellist = userBll.GetUsers(pageindex, pagesize, userName, out count);
            ViewBag.PageIndex = pageindex;
            ViewBag.PageCount = count % pagesize == 0 ? count / pagesize : (count / pagesize) + 1;
            ViewBag.callback = callback;
            return PartialView(modellist);
        }

        /// <summary>禁用用户
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [CheckSysAdmin()]
        public ActionResult disableUser(string uid)
        {
            int result;
            if (UserBll.SuperAdminID == uid)
            {
                result = -2;
            }
            else
            {
                UserBll userBll = new UserBll();
                if (userBll.SetUserStatus(uid, 0))
                {
                    new UserBll().delLoginSign(uid);
                }
                result = userBll.SetUserStatus(uid, 0) ? 1 : -1;
            }
            return Json(result);
        }
        /// <summary>启用用户
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [CheckSysAdmin()]
        public bool enableUser(string uid)
        {
            UserBll userBll = new UserBll();
            return userBll.SetUserStatus(uid, 1);
        }
        /// <summary>添加用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns>1 成功 -1 失败</returns>
        [CheckSysAdmin()]
        public string AddUser(UserEntity userEntity, string Pwd)
        {
            UserBll userBll = new UserBll();
            userEntity.Photo = "/Content/img/photo/photo16.jpg" ;
            int result = userBll.AddUser(userEntity, Pwd);
            return result.ToString();
        }
        /// <summary>修改用户名称
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>1 成功 -1 失败</returns>
        [CheckSysAdmin()]
        public int modifyUserName(string uid, string name)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(name) )
                return -1;
            UserBll userBll = new UserBll();
            return userBll.UpdateName(uid, name) ? 1 : -1;
        }
        /// <summary>修改用户头像
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>1 成功 -1 失败</returns>
        public ActionResult modifyUserPhotoV()
        {
            UserEntity user = new UserBll().getCurUser();
            ViewBag.User = user;
            return View();
        }
        /// <summary>修改用户头像
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>1 成功 -1 失败</returns>
        public ActionResult modifyUserPhotoV2()
        {
            UserEntity user = new UserBll().getCurUser();
            ViewBag.User = user;
            return View();
        }
        /// <summary>修改用户头像
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>1 成功 -1 失败</returns>
        public int modifyUserPhoto()
        {
            UserEntity user = new UserBll().getCurUser();
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(Request.Form["data"]));
            Bitmap img = new Bitmap(stream);
            string dirName = AppDomain.CurrentDomain.BaseDirectory + "Photo\\";
            string file = dirName + user.Uid + ".jpg";
            if (!Directory.Exists(dirName))
            {
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
            }

            img.Save(file);
            if (user.Photo.ToLower() == "/content/img/photo/photo16.jpg")
            {
                UserBll userBll = new UserBll();
                userBll.UpdatePhoto(user.Uid, "/Photo/" + user.Uid + ".jpg");
            }
            //fileBase.SaveAs(file);//保存文件
            //savePhoto(Request.Files[0],user.Uid );
            return 1;
        }
        /// <summary>删除用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
        [CheckSysAdmin()]
        public ActionResult DeleteUser(string id)
        {
            int result;
            if (UserBll.SuperAdminID == id)
            {
                result = -2;
            }
            else
            {
                UserBll userBll = new UserBll();
                result = userBll.DeleteUser(id);
                new UserBll().delLoginSign(id);
            }
            return Json(result);
        }
       

        /// <summary>重置密码
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [CheckSysAdmin()]
        public int resetPwd(string uid, string pwd)
        {
            UserBll userBll = new UserBll();
            bool result = userBll.UpdatePassWord(uid, pwd);
            return result ? 1 : 0;
        }
        /// <summary>修改密码密码
        /// </summary>
        /// <param name="opwd"></param>
        /// <param name="pwd"></param>
        /// <returns>1 修改成功 0 密码不匹配 -1 失败</returns>
        public int modifyPwd(string opwd, string pwd)
        {
            UserEntity user = new UserBll().getCurUser();
            return new UserBll().UpdatePassWord(user.Uid, opwd, pwd);
        }

        /// <summary>获取用户的角色列表
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="roldIds"></param>
        /// <returns></returns>
        [CheckSysAdmin()]
        public ActionResult GetUserRoles(string uId, string roldIds)
        {
            RoleBll roleBll = new RoleBll();
            List<RoleEntity> roleEntities = roleBll.GetRoles(roldIds);
            ViewData["uId"] = uId;
            return PartialView(roleEntities);
        }

        /// <summary>删除用户的一个角色
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [CheckSysAdmin()]
        public ActionResult RemoveUserRole(string uId, int roleId)
        {
            UserBll userBll = new UserBll();
            bool result = userBll.DeleteRole(uId, roleId);
            return Json(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        [CheckSysAdmin()]
        public ActionResult RoleList(string uId)
        {
            return PartialView();
        }

        [CheckSysAdmin()]
        public ActionResult GetRoles(int pageIndex, int pageSize)
        {
            RoleBll roleBll = new RoleBll();
            int count = 0;
            List<RoleEntity> roleEntities = roleBll.GetRoles(pageIndex, pageSize,"", out count);
            return PartialView(roleEntities);
        }

        [CheckSysAdmin()]
        public ActionResult AddRoles(string uId, string roleIds)
        {
            UserBll userBll = new UserBll();
            bool result = userBll.AddRole(uId, roleIds);
            return Json(result);
        }

        [CheckSysAdmin()]
        public ActionResult GetRoles2()
        {
            RoleBll roleBll = new RoleBll();
            List<RoleEntity> roleEntities = roleBll.GetRoles();
            var data = roleEntities.Select(m => new { m.Id, m.Name });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [CheckSysAdmin()]
        public ActionResult UpdateRights(string userId, string roleIds)
        {
            UserBll userBll = new UserBll();
            bool rsult = userBll.UpdateRoles(userId, roleIds);
            return Json(rsult);
        }
        #region 登录相关
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string uid, string pwd, bool remember)
        {

            int result = new UserBll().login(uid, pwd, remember);
            return Json(result);
        }


        #endregion


        #region private
        private void savePhoto(HttpPostedFileBase fileBase,string uid)
        {
            string dirName = AppDomain.CurrentDomain.BaseDirectory + "Photo\\";
            string file =dirName+ uid + ".jpg";
            if (!Directory.Exists(dirName))
            {
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
            }

            fileBase.SaveAs(file);//保存文件
            
        }
        #endregion
    }
}
