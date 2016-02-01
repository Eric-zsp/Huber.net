using Huber.Kernel.Entity;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Huber.Kernel
{
    /// <summary>
    /// 视图编译类
    /// </summary>
    public class CompileView
    {
        private static Regex layoutEx = new Regex("Layout\\s*=\\s*@?\"(\\S*)\";");//匹配视图中的layout
        static InvalidatingCachingProvider cache = new InvalidatingCachingProvider();
        static FileSystemWatcher m_Watcher = new FileSystemWatcher();

        static CompileView()
        {
            var config = new TemplateServiceConfiguration();
            config.BaseTemplateType = typeof(HuberImplementingTemplateBase<>);
            config.ReferenceResolver = new HuberReferenceResolver();
            config.CachingProvider = cache;
            cache.InvalidateAll();
            Engine.Razor = RazorEngineService.Create(config);
            //添加文件修改监控，以便在cshtml文件修改时重新编译该文件
            m_Watcher.Path = HuberVariable.CurWebDir;
            m_Watcher.IncludeSubdirectories = true;
            m_Watcher.Filter = "*.*";
            m_Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            m_Watcher.Created += new FileSystemEventHandler(OnChanged);
            m_Watcher.Changed += new FileSystemEventHandler(OnChanged);
            m_Watcher.Deleted += new FileSystemEventHandler(OnChanged);

            m_Watcher.EnableRaisingEvents = true;
        }
        //当视图被修改后清除缓存
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.EndsWith(".cshtml"))
            {
                string s = e.FullPath.Replace(HuberVariable.CurWebDir, "/");

                var key = Engine.Razor.GetKey(s);
                cache.InvalidateCache(key);
            }

        }

        public CompileView()
        {
        }

        public string RunCompile(ITemplateKey key, Type modelType, object model, DynamicViewBag viewBag)
        {
            //判断唯一视图的缓存
            string path = (HuberVariable.CurWebDir + key.Name).Replace(@"\\", @"\");
            ICompiledTemplate cacheTemplate;
            cache.TryRetrieveTemplate(key, null, out cacheTemplate);
            if (cacheTemplate == null || !cacheTemplate.Key.Name.Trim().Equals(key.Name.Trim()))
            {
                CompileViewAndLayout(key, null, model, viewBag);
            }
            //当缓存存在返回结果
            return Engine.Razor.RunCompile(key, null, model, viewBag);
        }
        /// <summary>
        /// 编译视图和层layout
        /// </summary>
        /// <param name="key">视图的唯一路径</param>
        /// <param name="modelType">视图类型 :视图/layout</param>
        /// <param name="model">页面 MODEL</param>
        /// <param name="viewBag">viewBag</param>
        public void CompileViewAndLayout(ITemplateKey key, Type modelType, object model, DynamicViewBag viewBag)
        {
            //获取视图
            string FullPath = (HuberVariable.CurWebDir + key.Name.Replace("/", @"\")).Replace(@"\\", @"\");
            string content = System.IO.File.ReadAllText(FullPath);
            //匹配layout
            var matchs = layoutEx.Matches(content);
            string layoutPath = string.Empty;
            if (matchs != null)
            {

                foreach (Match m in matchs)
                {
                    layoutPath = m.Groups[1].Value;
                }
            }
            if (layoutPath != string.Empty)
            {
                //添加layout到模板
                string FullLayoutPath = (HuberVariable.CurWebDir + layoutPath.Replace("/", @"\")).Replace(@"\\", @"\");

                if (File.Exists(FullLayoutPath))
                {
                    ITemplateKey layoutKey = Engine.Razor.GetKey(layoutPath, ResolveType.Layout);
                    CompileViewAndLayout(layoutKey, null, model, viewBag);
                }
            }
            if (key.TemplateType == ResolveType.Layout)
            {
                Engine.Razor.AddTemplate(key, content);
            }
            else
            {
                //编译视图
                Engine.Razor.RunCompile(content, key, null, model);
            }

        }
    }
}
