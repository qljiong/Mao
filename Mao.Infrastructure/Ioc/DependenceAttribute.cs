using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Ioc
{
    // 自动注册
    [AttributeUsage(AttributeTargets.Class)]
    public class DependenceAttribute : Attribute
    {
        public bool Singleton { get; set; }
        public string ObjName { get; set; }
        public DependenceAttribute(bool singleton = true)
        {
            this.Singleton = singleton;
        }

        public DependenceAttribute(string objName, bool singleton = true)
            : this(singleton)
        {
            this.ObjName = objName;
        }
    }
}
