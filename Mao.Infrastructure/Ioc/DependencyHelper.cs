using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Ioc
{
    public class DependencyHelper : IDependencyHelper
    {
        private readonly List<Type> _depTypes;
        private readonly object _lockRegister = new object();

        private readonly Type _attrType;

        private readonly IIocHook _ioc;
        public DependencyHelper(IIocHook ioc)
        {
            this._ioc = ioc;
            this._attrType = ioc.AttrType;
            this._depTypes = new List<Type>();
        }

        public void AddObjectClusterInAssemble(string assembly)
        {
            var assemble = Assembly.Load(assembly);
            var types = this.LoadTypesFromAssemble(assemble);
            this._depTypes.AddRange(types);
        }

        public void AddObjectClusterInAssemble(Assembly assembly)
        {
            this._depTypes.AddRange(this.LoadTypesFromAssemble(assembly));
        }


        private List<Type> LoadTypesFromAssemble(Assembly ass)
        {
            var types = ass.GetTypes()
                .Where(t => Attribute.IsDefined((MemberInfo) t, this._attrType));
            return types.ToList();
        }

        private void _registerImpl<T>(string objName)
        {
            var intfType = typeof(T);
            if (!intfType.IsInterface)
            {
                throw new Exception(
                    string.Format("Faild to Regiester type {0} with name {1}. Only interface could be register into the DI Pool!",
                        intfType.FullName, objName));
            }
            var types = this._depTypes.Where(t =>
                t.GetInterfaces().Contains(intfType));
            var implTypes = types as IList<Type> ?? types.ToList();
            var count = implTypes.Count();
            if (count == 0)
            {
                throw new Exception(
                    string.Format("Interface {0} does not have an implementation.",
                        intfType.FullName));
            }
            foreach (var implType in implTypes)
            {
                var depAttrs = implType.GetCustomAttributes(this._attrType, false);
                foreach (var eachdepAttr in depAttrs)
                {
                    this._ioc.DoRegister(eachdepAttr, implType, intfType);
                }
            }
        }

        public T Resolve<T>(string objName = null)
        {
            var key = string.IsNullOrEmpty(objName) ? typeof(T).FullName : objName;
            if (!this._ioc.Contains(key))
            {
                lock (this._lockRegister)
                {
                    if (!this._ioc.Contains(key))
                    {
                        _registerImpl<T>(key);
                    }
                }
            }
            return this._ioc.Resolve<T>(key);
        }

        public void Register<T>(T impl)
        {
            var t = typeof(T);
            if (!t.IsInterface)
            {
                throw new Exception(
                    string.Format("Faild to Regiester type {0}. Only interface could be register into the DI Pool!",
                        t.FullName));
            }
            this._ioc.RegisterSingleton(t.FullName, impl);
        }
    }
}
