using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel.Entity
{
    [Serializable]
    public class RefRespondEntity
    {
        public RefRespondEntity(RespondType type)
        {
            ResultType = type;
        }
        /// <summary>返回结果的数据类型
        /// </summary>
        public RespondType ResultType { get; set; }
        /// <summary>返回结果的内容
        /// 如果是ResultType=_Redirect那么ResultContext=301
        /// 如果是ResultType=_Stream那么ResultContext="文件名.zip",当然这个文件名可以随意定义
        /// </summary>
        public object ResultContext { get; set; }
        /// <summary>返回结果的文件流
        /// </summary>
        public byte[] ResultStream { get; set; }
    }
}
