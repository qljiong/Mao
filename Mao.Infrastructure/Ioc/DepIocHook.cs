using Mao.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Ioc
{
    public class DepIocHook : IIocHook
    {
        public Type AttrType
        {
            get { return typeof(DependenceAttribute); }
        }

        public T Resolve<T>(string objName = null)
        {
            return IocUtil.Get<T>(objName);
        }

        public void RegisterSingleton<T>(string name, T impl)
        {
            IocUtil.RegisterSingleton(name, impl);
        }

        public bool Contains(string key)
        {
            return IocUtil.Contains(key);
        }

        public void DoRegister(object attr, Type implType, Type intfType)
        {
            var depAttr = attr as DependenceAttribute;
            if (depAttr != null)
            {
                var instance = Activator.CreateInstance(implType);
                var key = string.IsNullOrEmpty(depAttr.ObjName)
                    ? intfType.FullName
                    : depAttr.ObjName;
                IocUtil.RegisterSingleton(key, instance);
            }

        }
    }
}
