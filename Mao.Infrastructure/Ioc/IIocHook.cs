using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Ioc
{
    public interface IIocHook
    {
        Type AttrType { get; }
        T Resolve<T>(string objName = null);
        void RegisterSingleton<T>(string name, T impl);
        bool Contains(string key);
        void DoRegister(object attr, Type implType, Type intfType);
    }
}
