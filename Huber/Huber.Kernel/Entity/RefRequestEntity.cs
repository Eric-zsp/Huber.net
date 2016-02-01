using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel.Entity
{
    /// <summary>用户的请求信息
    /// </summary>
    [Serializable]
    public class RefRequestEntity
    {
        /// <summary>当前用户在本页面具备的所有权限
        /// </summary>
        public List<RightEntity> PageRights;
        /// <summary>用户请求携带的所有参数
        /// </summary>
        public HuberRequest<string, object> Request;
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }
        public RefRequestEntity()
        {
            PageRights = new List<RightEntity>();
            Request = new HuberRequest<string, object>();
        }
    }
}
