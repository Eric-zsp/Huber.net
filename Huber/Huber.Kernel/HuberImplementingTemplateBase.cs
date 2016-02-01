using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel
{

    /// <summary>页面帮助类
    /// A simple helper demonstrating the @Html.Raw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HuberImplementingTemplateBase<T> : TemplateBase<T>
    {
        /// <summary>
        /// A simple helper demonstrating the @Html.Raw
        /// </summary>
        public HuberImplementingTemplateBase()
        {
            Html = new RazorHtmlHelper();
        }

        /// <summary>
        /// A simple helper demonstrating the @Html.Raw
        /// 
        /// </summary>
        public RazorHtmlHelper Html { get; set; }
  
    }
}
