using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Huber.SandBonDriver
{
    /// <summary>沙箱管道
    /// </summary>
    public class SandBoxChannel : MarshalByRefObject
    {
        /// <summary>沙箱内加载的程序集
        /// </summary>
        private Assembly _assembly;

        /// <summary>加载程序集
        /// </summary>
        /// <param name="assemblyFile">程序集文件路径（主程序类库路径，依赖类库自动加载）</param>
        public void LoadAssembly(string assemblyFile)
        {
            try
            {
                _assembly = Assembly.LoadFrom(assemblyFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>沙箱内方法调用
        /// </summary>
        /// <param name="typeName">类名称（全名称）</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public object InvokeMothod(string typeName, string methodName, params object[] args)
        {
            var _assembly_temp = _assembly;
            if (_assembly_temp == null)
                return null;
            
            if (typeName == "Huber.Kernel.Entity.CurVariable" && methodName == "SetCurWebDir")
            {//设置当前沙箱内的系统变量，非外部方法
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (a.GetName().Name == "Huber.Kernel")
                    {
                        _assembly_temp = a;
                        break;
                    }
                }
            }
            Type tp = _assembly_temp.GetType(typeName, true, false);
            if (tp == null)
                return null;
            MethodInfo method = tp.GetMethod(methodName);
            if (method == null)
                return null;
            Object obj = Activator.CreateInstance(tp);
            //ContentResult
            //FileContentResult
            //FileStreamResult
            //FilePathResult
            //HttpNotFoundResult
            //JavaScriptResult
            //JsonResult
            //PartialViewResult
            //RedirectToRouteResult
            //RedirectResult
            //ViewResult
            object result = method.Invoke(obj, args);
            return result;

        }




        /// <summary>获取所有的action
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, CAModel> GetAllAction()
        {
            Dictionary<string, CAModel> result = null;
            Type[] types = _assembly.GetTypes();
            MethodInfo[] meths = null;
            string controller = string.Empty;
            if (types != null)
            {
                result = new Dictionary<string, CAModel>();
                string url = string.Empty;
                CAModel temp;
                foreach (var t in types)
                {
                    if (t.BaseType != null && t.BaseType.ToString() == "Huber.Kernel.MVC.HuberController")
                    {

                        meths = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                        foreach (var m in meths)
                        {
                            temp = new CAModel(t.ToString(), m.Name);
                            result.Add(temp.UrlPath.ToLower(), temp);
                        }
                    }
                }
            }

            return result;
        }


        public override object InitializeLifetimeService()
        {
            //Remoting对象 无限生存期
            return null;
        }
    }
}
