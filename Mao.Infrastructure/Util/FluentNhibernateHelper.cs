using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Mao.Infrastructure.Mapping;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Data;
using System.Data.SqlClient;
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
        private const string exportFilePath = @"d:\abc.sql";
        private static string dbfile = System.Configuration.ConfigurationManager.AppSettings["dbfile"];
        private FluentNhibernateHelper()
        {

        }

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
                                           .Database(MsSqlConfiguration
                                           .MsSql2008
                                           .ConnectionString(c => c.FromConnectionStringWithKey("DefaultConnection")))
                                           .Mappings(m => m.FluentMappings
                                           .AddFromAssemblyOf<EmployeeMap>()).ExposeConfiguration(BuildSchema)
                                           .BuildSessionFactory();
                    }
                }
            }
            return _sessionFactory;
        }

        private static void BuildSchema(Configuration cfg)
        {
            var schemaExport = new SchemaExport(cfg);
            var str = cfg.Properties["connection.connection_string"].ToString();
            bool isNew = IsNewDb(str);
            if (isNew)
            {
                if (File.Exists(exportFilePath))
                    File.Delete(exportFilePath);
                schemaExport.SetOutputFile(exportFilePath);
            }
            schemaExport.Create(false, isNew);
        }

        private static bool IsNewDb(string connectString)
        {
            bool isNew = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    conn.Open();
                    string sql = "select * FROM Item";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

            }
            catch
            {
                isNew = true;
            }
            return isNew;
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
