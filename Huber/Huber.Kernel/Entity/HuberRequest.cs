using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Kernel.Entity
{
    [Serializable]
    public class HuberRequest<K, T>
    {
        public Dictionary<K, T> UserParam;
        public T this[K key]
        {
            get
            {
                if (UserParam.ContainsKey(key))
                    return UserParam[key];
                return default(T);

            }
            set
            {
                UserParam[key] = value;
            }

        }
        public HuberRequest()
        {
            UserParam = new Dictionary<K, T>();

        }

        public void Add(K key, T value)
        {
            UserParam[key] = value;
        }
    }
}
