using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel.Entity
{
    /// <summary>返回的结果类型
    /// </summary>
    public enum RespondType
    {
        _String,//字符串
        _Int,
        _Long,
        _Float,
        _Double,
        _Bool,
        _Stream,//流（如：文件流可用于文件导出）
        _Redirect//跳转
    }
}
