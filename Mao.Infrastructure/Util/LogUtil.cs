using HibernatingRhinos.Profiler.Appender;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Util
{
    public static class LogUtil
    {
        //生产环境一般开启 FATAL > ERROR > WARN 这三个级别 ,        
        //(高) OFF > FATAL > ERROR > WARN > INFO > DEBUG > ALL (低)-->

        const string _CONFIG_RESOURCE = "Xal.Op.Foc3.Infrastructure.InfrasConfig.log4net_embedded.config";

        private static readonly List<string> _LoadedFileNames = new List<string>();
        private const string _ROOTLOOGER_THRIDPARTY_NAME = "ROOT"; //默认INFO级别，可以设定，专门给Hibernate等第三方使用的
        private static readonly Dictionary<string, ILog> _Loggers = new Dictionary<string, ILog>();
        private static readonly object _LockerForAddLogger = null;

        static LogUtil()
        {
            _LockerForAddLogger = new object();
            _loadFromFile();
        }

        #region 取logger

        // 使用程序入口自定义的log4net配置文件
        private static void _loadFromFile()
        {
            string log4NetConfigPath = AppConfigUtil.GetAppConfig("log4net_config");
            // app.config or web.config file has a "log4net_config" configure value
            if (!string.IsNullOrEmpty(log4NetConfigPath))
            {
                log4NetConfigPath = FileUtil.CheckAndFillUpPath(log4NetConfigPath);
                XmlConfigurator.ConfigureAndWatch(new FileInfo(log4NetConfigPath));
                _LoadedFileNames.Add("file:" + log4NetConfigPath);
            }
            else
            {
                // app.config or web.config file doesn't have a "log4net_config" configure value, load default config from resource
                var configStream = ResourceUtil.GetResourceStream(_CONFIG_RESOURCE);
                XmlConfigurator.Configure(configStream);
                _LoadedFileNames.Add("assembly:" + _CONFIG_RESOURCE);
            }
        }

        private static ILog _initialLogger(string logName)
        {
            ILog logger;
            if (logName == _ROOTLOOGER_THRIDPARTY_NAME)
            {
                Type tp = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType; //根据类型来新生成logger，如果没有就为默认的root配置
                logger = LogManager.GetLogger(tp); //用tp，只是为了得到root，因为用string不知道怎么取到root
            }
            else
            {
                logger = LogManager.Exists(logName); //判断有无，兼取log
            }
            if (logger == null)
            {
                throw new ConfigurationErrorsException(logName + " does not exist.");
            }

            return logger;
        }
        #endregion

        // 取得 "log4net.config" 文件里的ROOT logger
        public static ILog GetLogger(string loggerName)
        {
            if (!_Loggers.ContainsKey(loggerName))
            {
                lock (_LockerForAddLogger)
                {
                    if (!_Loggers.ContainsKey(loggerName))
                    {
                        ILog log = _initialLogger(loggerName);
                        _Loggers.Add(loggerName, log);
                        _writeLevel(log, loggerName);
                    }
                }
            }

            return _Loggers[loggerName];
        }

        private static void _writeLevel(ILog logger, string loggerName)
        {
            logger.FatalFormat("[Begin {0} in {1}]", loggerName, AppDomain.CurrentDomain.BaseDirectory);
            logger.Fatal("【Log4NetDefault.config】开启的级别:\r\nFATAL:"
                + logger.IsFatalEnabled +
                "\r\nERROR:" + logger.IsErrorEnabled +
                "\r\nWARN:" + logger.IsWarnEnabled +
                "\r\nINFO:" + logger.IsInfoEnabled +
                "\r\nDEBUG:" + logger.IsDebugEnabled);
        }

        // 默认ILog，文件为Main.log
        public static ILog Default
        {
            get { return GetLogger(_ROOTLOOGER_THRIDPARTY_NAME); }
        }

        // Sql语句 ILog, 文件为SQL.log，与执行语句相关的日志，错误，都使用此Logger
        public static ILog SqlLogger
        {
            get { return GetLogger("SQL_logger"); }
        }

        // 记录sql执行日志到NHibernate Profile, nhSessionId:NHibernate使用的当前Session id, sql:执行的sql语句
        public static void NhProfilerSql
            (string nhSessionId, string sql,
            IEnumerable<IDataParameter> dbParas, int sqlTime, int rowCount)
        {
            CustomQueryReporting.ReportQuery(
                nhSessionId, "/* Log by LogUtil. */\r\n" + sql,
                dbParas, sqlTime, sqlTime, rowCount);

        }

        public static void NhProfilerError
            (string nhSessionId, string sql, string errorMsg)
        {
            CustomQueryReporting.ReportError(nhSessionId,
                string.Format("{0}\r\n{1}\r\n{2}", "/* Log by LogUtil */", sql, errorMsg));
        }


    }
}
