using Mao.Infrastructure.DB;
using Mao.Infrastructure.DB.Base;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Util
{
    /// <summary>
    /// 事务对象池
    /// (所有需数据库事务支持的Context，Repository，EntityFactory， DataSetFactory都在此注册、取用)
    /// </summary>
    public static class TxObjectPool
    {
        static TxObjectPool()
        {

        }

        #region Buz object - register ioc object by code

        /// <summary>
        /// 指定Name进行事务注册
        /// </summary>
        /// <typeparam name="TInterface">对象接口</typeparam>
        /// <param name="target">对象实例</param>
        /// <param name="dbName">数据库名:dsp、svc、flt ...</param>
        /// <param name="objName">对象Name</param>
        public static void Register<TInterface>(TInterface target, string dbName, string objName)
        {
            _insureSfIsInitialized(dbName);
            FocTxProxyFactoryObject txProxyFactoryObj = CreateTxProxyFactoryObject(target, dbName);
            TInterface tOrigin;
            try
            {
                tOrigin = (TInterface)txProxyFactoryObj.GetObject();
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException(
                    string.Format("Register transaction object {0}，use this way:IFoo foo = new Foo().",
                    typeof(TInterface)),
                     "target",
                     e);
            }
            if (tOrigin == null)
            {
                throw new AssertUtilException(
                    string.Format("Db {0} can't proxy a tx object {1}", dbName, target));
            }
            LogUtil.Default.InfoFormat("To register singleton target:{0}-{1}.", target, objName);
            IocUtil.XmlAppContext.ObjectFactory.RegisterSingleton(objName, tOrigin);

        }

        //注册代理工厂
        public static FocTxProxyFactoryObject CreateTxProxyFactoryObject<TInterface>
            (TInterface target, string dbName)
        {
            FocTxProxyFactoryObject txProxyFactory = new FocTxProxyFactoryObject();
            txProxyFactory.Target = target;
            string proxyMgrId = "proxy_mgr_" + dbName;

            var existProxyMgr = IocUtil.XmlAppContext.ContainsObject(proxyMgrId);

            if (!existProxyMgr)
            {
                string msg =
                    string.Format("Spring ioc object {0} does not exist, check ef_{1}.config.",
                    proxyMgrId, dbName);
                throw new ConfigurationErrorsException(msg);
            }

            //ef_{dbName}.config: defined the proxy_mgr_{dbName}
            //以 ef_xhfoc.config 里的 tx_proxy_xhfoc 配置为模板，生成 txProxyFactory 对象
            IocUtil.XmlAppContext.ConfigureObject(txProxyFactory, "proxy_mgr_" + dbName);

            return txProxyFactory;
        }


        /// <summary>
        /// 取用支持事务的对象
        /// </summary>
        /// <typeparam name="TInterface">对象接口（不可为类型）</typeparam>
        /// <param name="objName">对象名，默认为接口Type的FullName</param>
        /// <returns>支持事务的代理对象</returns>
        public static TInterface Get<TInterface>(string objName = null)
        {
            if (String.IsNullOrEmpty(objName))
            {
                objName = typeof(TInterface).FullName;
            }

            if (!IocUtil.XmlAppContext.ContainsObject(objName))
            {
                string msg = string.Format("未定义{0},请注意Get<T>签名中，T为接口,并使用[Tx]特性对{0}的实现类进行标注。",
                    objName);
                throw new Exception(msg);
            }

            TInterface proxyObjectWithTxSupport = IocUtil.XmlAppContext.GetObject<TInterface>(objName);
            return proxyObjectWithTxSupport;
        }

        #endregion


        #region Ef、Dsf - register ioc object by xml configuration
        private static readonly object _LockInitialSf = new object();

        public static IEntityFactory GetEf(string dbName)
        {
            _insureSfIsInitialized(dbName);
            return IocUtil.Get<IEntityFactory>("ef_" + dbName);
        }

        public static IDataSetFactory GetDsf(string dbName)
        {
            _insureSfIsInitialized(dbName);
            return IocUtil.Get<IDataSetFactory>("dsf_" + dbName);
        }

        private static void _insureSfIsInitialized(string dbName)
        {
            //ioc object 'sf_xhfoc' doesn't exist means that xhfoc db connection need to established.
            if (!IocUtil.Contains("sf_" + dbName))
            {
                lock (_LockInitialSf)
                {
                    if (!IocUtil.Contains("sf_" + dbName))
                    {
                        _initializeSf(dbName);
                    }
                }
            }
        }

        #endregion

        private static void _initializeSf(string dbName)
        {
            var sessionFactories = NhUtil.LoadSessionFactories(dbName);
            _logConnectionInfo(sessionFactories);
            foreach (var sf in sessionFactories)
            {
                _registerSessionFactory(sf);
            }
        }

        private static void _registerSessionFactory(ISessionFactory sf)
        {
            string sfId = ((NHibernate.Impl.SessionFactoryImpl)sf).Name;
            IocUtil.XmlAppContext.ObjectFactory.RegisterSingleton(sfId, sf);
        }

        private static void _logConnectionInfo(IList<ISessionFactory> sessionFactories)
        {
            string sfNames = string.Empty;
            foreach (var sessionFactory in sessionFactories)
            {
                var nhsf = (NHibernate.Impl.SessionFactoryImpl)sessionFactory;
                sfNames += nhsf.Name + "\r\n";
            }
            string msg = string.Format("Successful connect with {0} db:\r\n{1}",
                                       sessionFactories.Count(),
                                       sfNames);
            LogUtil.Default.InfoFormat(msg);
            Console.WriteLine(msg);
        }

    }
}
