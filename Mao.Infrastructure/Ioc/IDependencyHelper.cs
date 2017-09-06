using System.Reflection;

namespace Mao.Infrastructure.Ioc
{
    public interface IDependencyHelper
    {
        T Resolve<T>(string objName);
        void Register<T>(T impl);
        void AddObjectClusterInAssemble(string assembly);
        void AddObjectClusterInAssemble(Assembly assembly);
    }
}
