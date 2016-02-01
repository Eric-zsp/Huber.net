using Huber.Framework.Bll;
using Huber.Framework.Handle;
using Huber.Kernel;
using Huber.Kernel.Entity;
using Huber.SandBonDriver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Huber.FrameworkView.App_Code
{
    public class HuberHttpModule : IHttpModule
    {
        public void Dispose() { }
        public void Init(HttpApplication application)
        {

            application.BeginRequest += new EventHandler(Application_BeginRequest);
        }

        // 请求拦截
        private void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            HttpResponse respond = application.Response;
            HttpRequest request = application.Request;
            string url = request.Url.AbsolutePath.ToString();
            //如果请求以“/plugins/”开头，表面我们需要对该请求做拦截处理了。
            if (url.ToLower().StartsWith("/plugins/"))
            {
                string action = url.Substring(url.LastIndexOf("/") + 1);
                //如果是非静态文件，即是action
                if (action.IndexOf(".") < 0)
                {
                    #region 匹配controller和action

                    var urlEntity = HuberPluginHandle.getUrlPathEntity(url.Substring(8), true);
                    #endregion
                    if (urlEntity != null && urlEntity.controller != null)
                    {
                        #region 获取路径中的插件名称等信息

                        #endregion
                        SandBoxDynamicLoader sandBox = HuberPluginHandle.getSandBox(urlEntity.pluginname, urlEntity.pluginversion);

                        if (sandBox != null)
                        {
                            List<RightEntity> userRight = new List<RightEntity>();
                            string uid = string.Empty;
                            int login = new UserBll().chekLogin(ref uid, false, userRight);
                            if (login == 2)//验证用户是否具有访问的权限
                            {
                                RefRequestEntity paras = new RefRequestEntity();
                                paras.PageRights = userRight;
                                paras.UserID = uid;
                                #region 获取http参数
                                RequestHandle.FillCorRefEntity(paras, request);
                                #endregion
                                //sandBox.InvokeMothod(urlEntity.controller, "InitChannel", paras)
                                var result = sandBox.InvokeMothod(urlEntity.controller, urlEntity.action, paras);
                                RequestHandle.ResposeResult(respond, result);

                            }
                            else if (login == 1)
                            {
                                RequestHandle.ResponseNoRight(request, respond);
                            }
                            else
                            {
                                RequestHandle.ResponseNoLogin(request, respond);
                            }

                        }
                    }
                    else
                    {
                        RequestHandle.ResponseNotfound(request, respond); ;
                    }

                    respond.End();
                }
            }
            else
            {
                if (!url.ToLower().Equals("/user/login"))
                {
                    string action = url.Substring(url.LastIndexOf("/") + 1);
                    if (action.IndexOf(".") < 0)
                    {
                        List<RightEntity> userRight = new List<RightEntity>();
                        string uid = string.Empty;
                        int login = new UserBll().chekLogin(ref uid, false, userRight);
                        if (login == 2)//验证用户是否具有访问的权限
                        {

                        }
                        else if (login == 1)
                        {
                            RequestHandle.ResponseNoRight(request, respond);
                        }
                        else
                        {
                            RequestHandle.ResponseNoLogin(request, respond);
                        }
                    }
                       
                }
            }

        }
    }
}
