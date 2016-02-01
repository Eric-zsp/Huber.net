using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huber.Framework.Entity
{
    public class MenuEntity
    {
        public MenuEntity(int id, string name, string url, bool FrameWork, string Icon = "fa fa-circle-o")
        {
            this.id = id;
            this.name = name;
            this.url = url;
            this.FrameWork = FrameWork;
            this.Icon = Icon;
            Children = new List<MenuEntity>();
        }
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public bool FrameWork { get; set; }
        public string Icon { get; set; }
        public List<MenuEntity> Children { get; set; }
    }
}
