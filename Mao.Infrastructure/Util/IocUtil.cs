using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Util
{
    public static class IocUtil
    {
        public static readonly XmlApplicationContext XmlAppContext;
        static IocUtil()
        {
            XmlAppContext = (XmlApplicationContext)ContextRegistry.GetContext();
        }

        /// <summary>
        /// 取容器中的对象
        /// </summary>
        /// <param name="objId">容器中配置的对象ID</param>
        /// <param name="constructorArguments">构造函数参数</param>
        /// <returns></returns>
        public static T Get<T>(string objId, params object[] constructorArguments)
        {
            if (!Contains(objId))
            {
                throw new Exception("未找到对象配置:" + objId);
            }
            else
            {
                T obj = _get<T>(objId, constructorArguments);
                return obj; //objects.xml文件里的objName 对象
            }
        }

        /// <summary>
        /// 对象是否存在于容器
        /// </summary>
        /// <param name="objId">容器中配置的对象ID</param>
        public static bool Contains(string objId)
        {
            return XmlAppContext.ContainsObject(objId);
        }

        // 列出已注入的ioc对象名称
        public static string GetObjectDefinitionNames()
        {
            return XmlAppContext.GetObjectDefinitionNames()
                .Aggregate("", (current, objName) => current + "," + objName);
        }

        private static T _get<T>(string objId, params object[] constructorArguments)
        {
            return (constructorArguments != null && constructorArguments.Any()) ?
                XmlAppContext.GetObject<T>(objId, constructorArguments) :
                XmlAppContext.GetObject<T>(objId);
        }

        //注入单例对象
        public static void RegisterSingleton(string name, object singleton)
        {
            XmlAppContext.ObjectFactory.RegisterSingleton(name, singleton);
        }
    }
}
