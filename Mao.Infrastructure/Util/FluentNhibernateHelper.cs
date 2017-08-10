using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Mao.Models;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Configuration;
using System.IO;

namespace Mao.Infrastructure.Util
{
    /// <summary>
    /// Nhibernate辅助类
    /// </summary>
    public class FluentNhibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static ISession _session;
        private static object _objLock = new object();
        private FluentNhibernateHelper()
        {

        }

        private static string dbfile = ConfigurationManager.AppSettings["dbfile"];

        /// <summary>
        /// 创建ISessionFactory
        /// </summary>
        /// <returns></returns>
        public static ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory == null)
            {
                lock (_objLock)
                {
                    if (_sessionFactory == null)
                    {
                        //配置ISessionFactory
                        _sessionFactory = Fluently.Configure()
                            .Database(SQLiteConfiguration.Standard
                            .UsingFile(dbfile))
                            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Employee>())
                            .ExposeConfiguration(BuildSchema)
                            .BuildSessionFactory();
                    }
                }
            }
            return _sessionFactory;
        }

        private static void BuildSchema(NHibernate.Cfg.Configuration obj)
        {
            //delete the existing db on each run
            if (File.Exists(dbfile))
                File.Delete(dbfile);

            new SchemaExport(obj).Create(false, true);
        }

        /// <summary>
        /// 重置Session
        /// </summary>
        /// <returns></returns>
        public static ISession ResetSession()
        {
            if (_session.IsOpen)
                _session.Close();
            _session = _sessionFactory.OpenSession();
            return _session;
        }
        /// <summary>
        /// 打开ISession
        /// </summary>
        /// <returns></returns>
        public static ISession GetSession()
        {
            GetSessionFactory();
            if (_session == null)
            {
                lock (_objLock)
                {
                    if (_session == null)
                    {
                        _session = _sessionFactory.OpenSession();
                    }
                }
            }
            return _session;
        }

    }
}
