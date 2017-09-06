using Mao.Infrastructure.DB.Base;
using Mao.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Ioc
{
    public class TxIocHook : IIocHook
    {
        public Type AttrType
        {
            get { return typeof(TxAttribute); }
        }
        public T Resolve<T>(string objName = null)
        {
            return TxObjectPool.Get<T>(objName);
        }

        public void RegisterSingleton<T>(string name, T impl)
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(string key)
        {
            return IocUtil.Contains(key);
        }

        public void DoRegister(object attr, Type implType, Type intfType)
        {
            TxAttribute txAttr = attr as TxAttribute;
            if (txAttr != null)
            {
                var imple = Activator.CreateInstance(implType);
                var objName = string.IsNullOrEmpty(txAttr.IocObjectName)
                    ? intfType.FullName
                    : txAttr.IocObjectName;
                TxObjectPool.Register(imple, txAttr.DbName, objName);
            }
        }
    }
}
