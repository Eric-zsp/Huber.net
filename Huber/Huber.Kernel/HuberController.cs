using RazorEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;

using Encoding = RazorEngine.Encoding;
using RazorEngine.Compilation.ImpromptuInterface;
using System.Web.Mvc;
using Huber.Kernel.Entity;

namespace Huber.Kernel
{

    public class HuberController
    {

        public dynamic ViewBag = new DynamicViewBag();
        public ViewDataDictionary ViewData = new ViewDataDictionary();

       
        /// <summary>设置ViewBag的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        internal void AddViewBageValues(string key, object value)
        {

            Impromptu.InvokeSet(ViewBag, key, value);

        }

        /// <summary>返回试图的执行结果
        /// </summary>
        /// <returns></returns>
        protected string View()
        {
            var tKey = Engine.Razor.GetKey(getActionPath(), ResolveType.Global);
            return new CompileView().RunCompile(tKey, null, null, ViewBag);
        }
        /// <summary>返回试图的执行结果
        /// </summary>
        /// <typeparam name="T">model的类型</typeparam>
        /// <param name="model">model</param>
        /// <returns></returns>
        protected string View<T>(T model)
        {
            var tKey = Engine.Razor.GetKey(getActionPath(), ResolveType.Global);
            return new CompileView().RunCompile(tKey, typeof(T), model, ViewBag);
        }
        /// <summary>返回试图的执行结果
        /// </summary>
        /// <param name="viewName">视图的全路径（相对于运行目录的全路径）</param>
        /// <returns></returns>
        protected string View(string viewName)
        {
            var tKey = Engine.Razor.GetKey(getActionPathWith(viewName), ResolveType.Global);
            return new CompileView().RunCompile(tKey, null, null, ViewBag);
        }
        /// <summary>返回试图的执行结果
        /// </summary>
        /// <typeparam name="T">model的类型</typeparam>
        /// <param name="viewName">视图的全路径（相对于运行目录的全路径）</param>
        /// <param name="model">model</param>
        /// <returns></returns>
        protected string View<T>(string viewName, T model)
        {
            var tKey = Engine.Razor.GetKey(getActionPathWith(viewName), ResolveType.Global);
            return new CompileView().RunCompile(tKey, typeof(T), model, ViewBag);
        }

        /// <summary>返回局部试图的执行结果
        /// </summary>
        /// <returns></returns>
        protected string PartialView()
        {
            var tKey = Engine.Razor.GetKey(getActionPath(), ResolveType.Include);
            return new CompileView().RunCompile(tKey, null, null, ViewBag);
        }
        /// <summary>返回局部试图的执行结果
        /// </summary>
        /// <typeparam name="T">model的类型</typeparam>
        /// <param name="model">model</param>
        /// <returns></returns>
        protected string PartialView<T>(T model)
        {
            var tKey = Engine.Razor.GetKey(getActionPath(), ResolveType.Include);
            return new CompileView().RunCompile(tKey, typeof(T), model, ViewBag);
        }
        /// <summary>返回局部试图的执行结果
        /// </summary>
        /// <param name="viewName">视图的全路径（相对于运行目录的全路径）</param>
        /// <returns></returns>
        protected string PartialView(string viewName)
        {
            var tKey = Engine.Razor.GetKey(getActionPathWith(viewName), ResolveType.Include);
            return new CompileView().RunCompile(tKey, null, null, ViewBag);
        }
        /// <summary>返回局部试图的执行结果
        /// </summary>
        /// <typeparam name="T">model的类型</typeparam>
        /// <param name="viewName">视图的全路径（相对于运行目录的全路径）</param>
        /// <param name="model">model</param>
        /// <returns></returns>
        protected string PartialView<T>(string viewName, T model)
        {
            var tKey = Engine.Razor.GetKey(getActionPathWith(viewName), ResolveType.Include);
            return new CompileView().RunCompile(tKey, typeof(T), model, ViewBag);
        }



        /// <summary>获取action对应view的物理文件地址
        /// </summary>
        /// <returns></returns>
        private string getActionPath()
        {
            string key = string.Empty;
            StackTrace trace = new StackTrace();
            MethodBase methodName = trace.GetFrame(2).GetMethod();
            string className = methodName.ReflectedType.FullName;

            string assName = HuberHttpModule.CurDomainAssemblyName;
            key = className.Substring(assName.Length);
            key = key.Replace(".Controllers.", ".Views.");
            key = key.Substring(0, key.Length - 10);
            key = key.Replace(".", "\\");
            key += "\\" + methodName.Name + ".cshtml";
            return key;
        }
        /// <summary>根据action名获取其对应view的物理文件地址
        /// </summary>
        /// <param name="ActionName">action名（同一controller中）</param>
        /// <returns></returns>
        private string getActionPathWith(string ActionName)
        {
            string key = string.Empty;
            StackTrace trace = new StackTrace();
            MethodBase methodName = trace.GetFrame(2).GetMethod();
            string className = methodName.ReflectedType.FullName;

            string assName = HuberHttpModule.CurDomainAssemblyName;
            key = className.Substring(assName.Length);
            key = key.Replace(".Controllers.", ".Views.");
            key = key.Substring(0, key.Length - 10);
            key = key.Replace(".", "\\");
            key += "\\" + ActionName + ".cshtml";
            return key;
        }
    }
}
