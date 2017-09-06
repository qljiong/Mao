using Mao.Infrastructure.Ioc;
using System.Reflection;

namespace Mao.Core.Util
{
    /*
     * this is an ioc for the inject without the Tx
     */
    public static class DependencyInjectUtil
    {
        private static readonly IDependencyHelper _DepHelper;
        static DependencyInjectUtil()
        {
            var dep = new DependencyHelper(new DepIocHook());
            dep.AddObjectClusterInAssemble("Mao.Infrastructure");
            dep.AddObjectClusterInAssemble(Assembly.GetExecutingAssembly());

            _DepHelper = dep;
        }

        public static T Resolve<T>(string objName = null)
        {
            return _DepHelper.Resolve<T>(objName);
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
