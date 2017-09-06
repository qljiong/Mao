using Mao.Infrastructure.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Core.Util
{
    /// <summary>
    /// Infrastructure 事务拦截对象池 简单封装
    /// </summary>
    public static class TransactionUtil
    {
        private static readonly IDependencyHelper _DepHelper;
        static TransactionUtil()
        {
            var dep = new DependencyHelper(new TxIocHook());
            dep.AddObjectClusterInAssemble("Mao.Infrastructure");
            dep.AddObjectClusterInAssemble(Assembly.GetExecutingAssembly());

            _DepHelper = dep;

        }
         

        public static T Resolve<T>(string objName = null)
        {
            return _DepHelper.Resolve<T>(objName);
        }

        /// <summary>
        /// 取用支持事务的对象
        /// </summary>
        /// <typeparam name="TInterface">对象接口</typeparam>
        /// <param name="objName">注册时使用的Name，默认为接口Type的FullName</param>
        /// <returns>支持事务的代理对象</returns>
        public static TInterface Get<TInterface>(string objName = null)
        {
            return _DepHelper.Resolve<TInterface>(objName);
        }

        public static void AddObjectClusterInAssemble(string ass)
        {
            _DepHelper.AddObjectClusterInAssemble(ass);
        }
        public static void AddObjectClusterInAssemble(Assembly ass)
        {
            _DepHelper.AddObjectClusterInAssemble(ass);
        }
    }
}
