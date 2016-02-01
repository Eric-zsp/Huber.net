using Huber.Kernel.Entity;
using RazorEngine;
using RazorEngine.Compilation.ImpromptuInterface;
using RazorEngine.Templating;
using RazorEngine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel
{
    public class RazorHtmlHelper
    {

        /// <summary>
        /// 调用Action视图
        /// </summary>
        /// <param name="actionName">action方法名称</param>
        /// <param name="controllerName">控制器名称</param>
        /// <returns></returns>
        public IEncodedString Action(string actionName, string controllerName)
        {
            return Action(actionName, controllerName, new { });

        }

        /// <summary>
        /// 调用Action视图
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues">传入参数</param>
        /// <returns></returns>
        public IEncodedString Action(string actionName, string controllerName, object routeValues)
        {
            RefRequestEntity paras = SetParamValue(routeValues);

            var t = HuberHttpModule.CurDomainAssembly.GetType(HuberHttpModule.CurDomainAssemblyName + ".Controllers." + controllerName + "Controller");
            var m = t.GetMethod(actionName);
            object dObj = Activator.CreateInstance(t);
            object result = m.Invoke(dObj, new object[] { paras });
            return new RawString((result as RefRespondEntity).ResultContext.ToString());
        }

        /// <summary>
        /// 根据model设置传入参数
        /// </summary>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        private static RefRequestEntity SetParamValue(object routeValues)
        {
            RefRequestEntity paras = new RefRequestEntity();

            Type t1 = routeValues.GetType();
            PropertyInfo[] pis = t1.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                paras.Request.Add(pi.Name, pi.GetValue(routeValues));

            }
            return paras;
        }


        public IEncodedString RenderAction(string actionName, string controllerName)
        {
            return Action(actionName, controllerName, new { });
        }

        public IEncodedString RenderAction(string actionName, string controllerName, object routeValues)
        {
            return Action(actionName, controllerName, routeValues);
        }

        public IEncodedString RenderPartial(string partialViewName, string controllerName)
        {
            return RenderPartial(partialViewName, controllerName, new { }, new DynamicViewBag());
        }

        // Renders the partial view with the given view data and, implicitly, the given view data's model
        public IEncodedString RenderPartial(string partialViewName, string controllerName, DynamicViewBag ViewBag)
        {
            return RenderPartial(partialViewName, controllerName, new { }, ViewBag);
        }

        // Renders the partial view with an empty view data and the given model
        public IEncodedString RenderPartial(string partialViewName, string controllerName, object model)
        {
            return RenderPartial(partialViewName, controllerName, model, new DynamicViewBag());
        }


        // Renders the partial view with a copy of the given view data plus the given model
        /// <summary>
        /// 部分视图
        /// </summary>
        /// <param name="partialViewName">部分视图名称</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="model"> model</param>
        /// <param name="ViewBag">ViewBag</param>
        /// <returns></returns>
        public IEncodedString RenderPartial(string partialViewName, string controllerName, object model, DynamicViewBag ViewBag)
        {


            RefRequestEntity paras = SetParamValue(model);

            var t = HuberHttpModule.CurDomainAssembly.GetType(HuberHttpModule.CurDomainAssemblyName + ".Controllers." + controllerName + "Controller");
            var ActionFunc = t.GetMethod(partialViewName);
            object dObj = Activator.CreateInstance(t);

            var AddViewBageFunc = t.GetMethod("AddViewBageValues");

            foreach (string key in ViewBag.GetDynamicMemberNames())
            {

                AddViewBageFunc.Invoke(dObj, new object[] { key, Impromptu.InvokeGet(ViewBag, key) });
            }

            object result = ActionFunc.Invoke(dObj, new object[] { paras });
            return new RawString((result as RefRespondEntity).ResultContext.ToString());
        }


        public IEncodedString ViewPartial(string partialViewName, string controllerName = "", object model = null, DynamicViewBag ViewBag = null)
        {

            if (controllerName == string.Empty)
            {
                controllerName = "Shared";
            }
            string key = "\\Views\\" + controllerName + "\\" + partialViewName + ".cshtml";
            var tKey = Engine.Razor.GetKey(key, ResolveType.Include);
            return new RawString(new CompileView().RunCompile(tKey, null, model, ViewBag));
        }

    }
}
