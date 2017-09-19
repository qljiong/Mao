using System.Collections.Generic;
using Mao.Infrastructure.Util;

namespace Mao.Infrastructure
{
    public class SysEnvSpecification
    {
        public const string DEVE = "DEVE";
        public const string TEST = "TEST";
        public const string PROD = "PROD";
        public const string PROV = "PROV";
        public const string DRVY = "DRVY";  // disaster recovery

        // description in chinese, key is the english version
        private static readonly Dictionary<string, string> _EvnDescriptions = new Dictionary<string, string>
        {
            {DEVE, "开发库"},
            {TEST, "测试库"},
            {PROD, "生产库"},
            {PROV, "生产验证"},
            {DRVY, "灾备库"}
        };

        static SysEnvSpecification()
        {
            Refresh();
        }

        public static void Refresh()
        {
            Current = AppConfigUtil.GetAppConfig(ConfigurationKeys.ENV_SPECIFICATION);
        }
        public static string Current { get; set; }

        public static string EnvDescription
        {
            get
            {
                {
                    if (string.IsNullOrEmpty(Current))
                    {
                        return "UNKNOWN";
                    }
                    return _EvnDescriptions.ContainsKey(Current) ? _EvnDescriptions[Current] : "UNKNOWN";
                }
            }
        }

        public static bool IsDeve()
        {
            return Current == DEVE;
        }

        public static bool IsTest()
        {
            return Current == TEST;
        }

        public static bool IsProd()
        {
            return Current == PROD;
        }

        public static bool IsInProdEnv()
        {
            return Current != DEVE && Current != TEST;
        }


    }


    public class ConfigurationKeys
    {
        public const string ENV_SPECIFICATION = "env_spacification";
        public const string WS_SERVER = "ws";
        public const string WS4SYS_SERVER = "ws4sys";

        public const string REDIS_CONFIG = "redis_config";

    }
}
