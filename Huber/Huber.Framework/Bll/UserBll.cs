using Huber.Framework.Dal;
using Huber.Framework.Entity;
using Huber.Framework.Handle;
using Huber.Framework.Tools;
using Huber.Kernel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Huber.Framework.Bll
{
    public class UserBll
    {
        /// <summary>获得用户列表 
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public List<UserEntity> GetUsers(int pageIndex, int pageSize, string userName, out int count)
        {
            count = 0;
            int skip = (pageIndex - 1) * pageSize;
            UserDal userDal = new UserDal();
            //TODO 查询权限
            return userDal.GetUsers(skip, pageSize, userName, out count);
        }
        /// <summary>获得用户 
        /// </summary>
        /// <returns></returns>
        public UserEntity GetUser(string uId)
        {

            UserEntity userEntity = new UserDal().GetUser(uId);
            //TODO 查询权限
            return userEntity;
        }
        /// <summary>设置用户状态 
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="status">0：不启用 1：启用</param>
        /// <returns></returns>
        public bool SetUserStatus(string uId, int status)
        {
            if (status != 0 && status != 1)
                return false;
            UserDal userDal = new UserDal();
            return userDal.SetUserStatus(uId, status);
        }
        /// <summary>添加用户 
        /// </summary>
        /// <param name="userEntity"></param>
        /// <returns>1：成功 -1：失败 -2：已存在相同用户</returns>
        public int AddUser(UserEntity userEntity, string pwd)
        {
            UserDal userDal = new UserDal();
            if (!userDal.ExistUId(userEntity.Uid))
            {
                return userDal.AddUser(userEntity, pwd) ? 1 : -1;
            }
            return -2;
        }
        /// <summary>删除用户
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public int DeleteUser(string uId)
        {
            UserDal userDal = new UserDal();
            return userDal.DeleteUser(uId);
        }
        /// <summary>修改用户昵称 
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="userName">用户昵称</param>
        /// <returns></returns>
        public bool UpdateName(string uId, string userName)
        {
            if (string.IsNullOrEmpty(uId) || string.IsNullOrEmpty(userName))
                return false;
            UserDal userDal = new UserDal();
            return userDal.UpdateName(uId, userName);
        }/// <summary>修改用户头像 
         /// </summary>
         /// <param name="uId">用户Id</param>
         /// <param name="Photo">用户头像</param>
         /// <returns></returns>
        public bool UpdatePhoto(string uId, string Photo)
        {
            if (string.IsNullOrEmpty(uId) || string.IsNullOrEmpty(Photo))
                return false;
            UserDal userDal = new UserDal();
            return userDal.UpdatePhoto(uId, Photo);
        }
        /// <summary>为用户添加角色 
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        /// <returns>1：成功 -1：失败 -2：已存在角色Id -3：用户不存在</returns>
        public int AddRole(string uId, int roleId)
        {
            UserDal userDal = new UserDal();
            UserEntity userEntity = userDal.GetUser(uId);
            if (userEntity != null)
            {
                foreach (string item in userEntity.RolesIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (item == roleId.ToString())
                    {
                        return -2;
                    }
                }
                return userDal.AddRole(uId, roleId) ? 1 : -1;
            }
            return -3;
        }
        /// <summary>删除角色 
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public bool DeleteRole(string uId, int roleId)
        {
            UserDal userDal = new UserDal();
            return userDal.DeleteRole(uId, roleId);
        }
        /// <summary>修改密码 
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public bool UpdatePassWord(string uId, string passWord)
        {
            UserDal userDal = new UserDal();
            return userDal.UpdatePassWord(uId, passWord);
        }
        /// <summary>修改密码
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="opwd">旧密码</param>
        /// <param name="pwd">新密码</param>
        /// <returns></returns>
        public int UpdatePassWord(string uId, string opwd, string pwd)
        {
            UserDal userDal = new UserDal();
            return userDal.UpdatePassWord(uId, opwd, pwd);
        }

        /// <summary>移除角色 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool RemoveRole(int roleId)
        {
            UserDal userDal = new UserDal();
            return userDal.RemoveRole(roleId);
        }

        public bool AddRole(string uId, List<int> roleIds)
        {
            UserDal userDal = new UserDal();
            return userDal.AddRole(uId, roleIds);
        }
        public bool AddRole(string uId, string roleIds)
        {
            List<int> roleList = new List<int>();
            foreach (string item in roleIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                int roleId;
                if (Int32.TryParse(item, out roleId))
                {
                    roleList.Add(roleId);
                }
            }
            return AddRole(uId, roleList);
        }

        public bool UpdateRoles(string uId, string roleIds)
        {
            List<int> roleList = new List<int>();
            foreach (string item in roleIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                int roleId;
                if (Int32.TryParse(item, out roleId))
                {
                    roleList.Add(roleId);
                }
            }

            if (!roleList.Any())
            {
                roleIds = ",";
            }
            else
            {
                roleIds = "," + string.Join(",", roleList) + ",";
            }
            return new UserDal().UpdateRole(uId, roleIds);
        }



        #region login
        static readonly string EnSignKey = "Huber_key";
        public static readonly string CoSignKey = "Huber_Sign";
        public static readonly string SuperAdminID = "User_root";//超级管理员ID

        public static bool IsOnlyOne { get; set; }
        /// <summary>登录
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pwd">密码（加密之后的）</param>
        /// <returns>1登录成，-1账号不存在，-2密码错误， -3账户已被禁用</returns>
        public int login(string uid, string pwd, bool remember)
        {
            int result = new UserDal().login(uid, pwd);
            if (result == 1)
            {
                string sign = ensign(uid, pwd);
                new LoginDal().addLoginsign(uid, sign, IsOnlyOne);

                if (remember)
                {
                    CookieFunc.writeCookie(CoSignKey, sign);
                }
                else
                {
                    CookieFunc.writeCookie(CoSignKey, sign, 15);
                }

                new Task(() => { new LoginDal().DeleteLoginSign(); }).Start();
            }
            return result;
        }


        /// <summary>验证登录
        /// </summary>
        /// <returns>2 具有访问权限 1 没有权限  0 未登录</returns>
        public int chekLogin(ref string uid, bool liwai, List<RightEntity> userRights)
        {
            int result = 0;
            string sign = CookieFunc.ReadCookie(CoSignKey);
            if (sign != null && sign != string.Empty)
            {
                uid = string.Empty;
                string pwd = string.Empty;
                DateTime dt = DateTime.Now;
                if (design(sign, ref uid, ref pwd, ref dt))
                {
                    if (dt.AddDays(15) > DateTime.Now)//令牌未过期
                    {
                        int signState = new LoginDal().exsitLoginSign(uid, sign, IsOnlyOne);
                        if (signState == 1)
                        {
                            result = 1;
                        }
                        else if (signState == -1)
                        {
                            if (1 == new UserDal().login(uid, pwd))
                            {
                                result = 1;
                            }
                        }
                        if (result > 0)
                        {
                            if (!liwai)
                            {
                                #region 获取当前页面的权限
                                UrlPathEntity urlEntity = null;
                                List<RightEntity> rlist = null;
                                if (HttpContext.Current.Request.RawUrl.StartsWith("/Plugins/"))
                                {

                                    urlEntity = HuberPluginHandle.getUrlPathEntity(HttpContext.Current.Request.RawUrl.Substring(8), true);
                                    rlist = new RightBll().UserGetRights("/" + urlEntity.pluginname + "/" + urlEntity.controller + "/" + urlEntity.action);
                                }
                                else
                                {
                                    urlEntity = HuberPluginHandle.getUrlPathEntity(HttpContext.Current.Request.RawUrl, false);
                                    rlist = new RightBll().UserGetRights("/" + urlEntity.controller + "/" + urlEntity.action);
                                }
                                UserEntity CurUer = new UserDal().GetUser(uid);
                                if (CurUer != null)
                                {
                                    if (rlist.Count > 0)
                                    {
                                        List<RightEntity> urights = new List<RightEntity>();
                                        string rightCompara = ",{0},";

                                        if (CurUer.Uid == SuperAdminID)//如果是超级管理员，不需要对权限筛选
                                        {
                                            urights = rlist;
                                        }
                                        else
                                        {
                                            List<RoleEntity> uRoles = new RoleBll().GetRoles(CurUer.RolesIds);
                                            if (uRoles != null && uRoles.Count > 0)
                                            {
                                                foreach (RightEntity right in rlist)
                                                {
                                                    foreach (RoleEntity role in uRoles)
                                                    {
                                                        if (role.RightIds.IndexOf(string.Format(rightCompara, right.Id)) > -1)
                                                        {
                                                            urights.Add(right);
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                        userRights = urights;
                                        result = 2;
                                    }
                                    else
                                    {
                                        if (CurUer.Uid == SuperAdminID)//如果是超级管理员，不需要对权限筛选
                                        {
                                            result = 2;
                                        }
                                    }
                                }



                                #endregion
                            }
                            else
                            {
                                result = 2;
                            }

                        }
                    }
                }
            }
            return result;
        }

        /// <summary>获取当前用户的账号和姓名
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="uname"></param>
        /// <returns></returns>
        public UserEntity getCurUser()
        {
            UserEntity result = null;

            string sign = CookieFunc.ReadCookie(CoSignKey);
            if (sign != null && sign != string.Empty)
            {
                string uid = string.Empty;
                string pwd = string.Empty;
                DateTime dt = DateTime.Now;
                if (design(sign, ref uid, ref pwd, ref dt))
                {
                    result = new UserBll().GetUser(uid);
                }
            }
            return result;
        }

        public void delLoginSign(string uid)
        {
            new LoginDal().delLoginSign(uid);
        }
        /// <summary>生成令牌
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        string ensign(string uid, string pwd)
        {
            return EncryptionFunc.Encrypt(uid + "\f" + pwd + "\f" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), EnSignKey);
        }
        /// <summary>解密令牌
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        bool design(string sign, ref string uid, ref string pwd, ref DateTime dt)
        {
            try
            {

                string temp = EncryptionFunc.Decrypt(sign, EnSignKey);
                string[] SignData = temp.Split('\f');
                uid = SignData[0];
                pwd = SignData[1];
                dt = DateTime.Parse(SignData[2]);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
