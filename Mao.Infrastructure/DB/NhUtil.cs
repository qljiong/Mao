using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using Mao.Infrastructure.Util;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Configuration = NHibernate.Cfg.Configuration;

namespace Mao.Infrastructure.Db
{
    // NHibernate 初始化
    internal static class NhUtil
    {
        //TxObjectPool is the only invocker.
        //So insure threadsafe at TxObjectPool when call NhUtil.LoadSessionFactories().
        public static List<ISessionFactory> LoadSessionFactories(string dbName)
        {
            Spring.Data.Common.DbProviderFactory.DBPROVIDER_ADDITIONAL_RESOURCE_NAME
                = "file://DbConfig/additional_providers.config";
            List<ISessionFactory> sfList = new List<ISessionFactory>();
            IEnumerable<string> sfFiles = _getSessionFactoryFilePath(dbName);
            foreach (string sfFile in sfFiles)
            {
                string smartFile = FileUtil.CheckAndFillUpPath(sfFile);

                Configuration eachDbCfg = new Configuration().Configure(smartFile);
                string connStringOrigin = eachDbCfg.Properties["connection.connection_string"];
                string connStringPlainPwd = NhPasswordUtil.DecryptConnectionString(connStringOrigin);
                eachDbCfg.Properties["connection.connection_string"] = connStringPlainPwd;

                try
                {
                    FluentConfiguration fluentConfiguration = _buildFluentlyConfig(dbName, eachDbCfg);

                    //Build session factory，means create a db connection, so by load-on-demand. 
                    ISessionFactory sf = fluentConfiguration.BuildSessionFactory();
                    sfList.Add(sf);
                }
                catch (Exception ex)
                {
                    LogUtil.Default.ErrorFormat("Session factory failed:{0}{1}{0}{2}{0}Exception:{0}{3}.",
                        Environment.NewLine,
                        smartFile,
                        connStringPlainPwd,
                        ex.InnerMessage());
                    throw;//Comment it, if u don't want to be affected when any connection failed.
                }
            }
            return sfList;
        }

        //Return all sf_{dbName}_* config path, such as xhfoc : sf_xhfoc_test, sf_xhfoc_pro
        private static IEnumerable<string> _getSessionFactoryFilePath(string dbName)
        {
            string sessionFactoryFiles = AppConfigUtil.GetAppConfig("nh_session_factory_files");
            if (String.IsNullOrEmpty(sessionFactoryFiles))
                throw new Exception("Check app.config/web.config's appSettings section," +
                                    "add node: <add key= \"nh_session_factory_files\" value=\"session factory file path(separate by ;)\" />");

            var paths = sessionFactoryFiles.Split(new[] { ';' });

            var matchSfFiles = paths.Select(sfPath => sfPath.Trim()).Where(
                sfPath => !string.IsNullOrEmpty(sfPath.Trim()) &&
                    sfPath.Contains(string.Format("sf_{0}_", dbName))
                );

            List<string> matchSfList = matchSfFiles.ToList();

            if (matchSfList.Count == 0)
            {
                string msg = string.Format(
                    "Can't find any sf_{0}_*.config at appSettings(key='{1}') in app.config/web.config.",
                    dbName,
                    "nh_session_factory_files");
                throw new ConfigurationErrorsException(msg);
            }

            return matchSfList;
        }

        private static FluentConfiguration _buildFluentlyConfig(string dbName, Configuration nhCfg)
        {
            var mappingDll = Assembly.Load(_getMappingAssemblyName());

            FluentConfiguration fluentConfig = Fluently.Configure(nhCfg)
                .Mappings(m =>
                {
                    if (dbName == "ais")
                    {
                        // todo:
                        m.AutoMappings.Add(AutoMap.Assembly(mappingDll).Where(t => t.Namespace.Contains("naip")));
                    }
                    else
                    {
                        m.FluentMappings
                            .Conventions.Setup(x => x.Add(AutoImport.Never()))
                            .AddFromAssembly(mappingDll)
                            //.ExportTo(@"./TempHbmDir/") //If u need hbm files, create this dir first.
                            ;
                    }
                    ;

                    //if (dbName == "ais")
                    //{
                    //    m.AutoMappings.Add(AutoMap.Assembly(mappingDll).Where(t => t.Namespace.Contains("naip")));
                    //}
                    //else
                    //{
                    //    m.FluentMappings
                    //        .Conventions.Setup(x => x.Add(AutoImport.Never()))
                    //        .AddFromAssembly(mappingDll)
                    //        //.ExportTo(@"./TempHbmDir/") //If u need hbm files, create this dir first.
                    //        ;
                    //}
                    //var types = mappingDll.GetTypes().Where(t => ReflectionUtil.IsSubClassOfGeneric(t, typeof(ClassMap<>)));
                    //types.Foreach(t =>
                    //{
                    //    var attrs = t.GetCustomAttributes(typeof(TxAttribute), false);
                    //    if (attrs.Length > 0)
                    //    {
                    //        var iattrs = (attrs as TxAttribute[]).AsEnumerable();
                    //        var dbs = iattrs.Select(a => a.DbName);
                    //        if (!dbs.Contains(dbName)) return;
                    //    }
                    //    var bt = ReflectionUtil.GetBaseTypeOf(typeof (ClassMap<>));
                    //    var args = bt.GetGenericArguments();
                    //    if (args.Length > 0)
                    //        m.FluentMappings.Add(args[0]);
                    //});
                });
            return fluentConfig;
        }

        private static string _getMappingAssemblyName()
        {
            string assemblyName = AppConfigUtil.GetAppConfig("nh_mapping_assembly");

            if (String.IsNullOrEmpty(assemblyName))
                throw new Exception("Check app.config or web.config's appSettings section," +
                                    "add node <add key= \"nh_mapping_assembly\" value=\"dll file path\" />");
            else
                return assemblyName;
        }
    }
}
