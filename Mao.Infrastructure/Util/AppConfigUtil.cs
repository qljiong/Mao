using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mao.Infrastructure.Util
{
    public class AppConfigUtil
    {
        public static string GetAppConfig(string key, string exeFile = null)
        {
            if (string.IsNullOrEmpty(exeFile))
            {
                return ConfigurationManager.AppSettings[key];
            }
            if (!File.Exists(exeFile)) throw new FileNotFoundException("未找到应用程序文件:" + exeFile, exeFile);
            Configuration config = ConfigurationManager.OpenExeConfiguration(exeFile);

            return config.AppSettings.Settings[key].Value;
        }

        /// <summary>  
        /// 在*.exe.config文件中appSettings配置节增加一对键值对  
        /// </summary>  
        /// <param name="key"></param>  
        /// <param name="val"></param>
        /// <param name="exeFile"></param>  
        public static void UpdateAppConfig(string key, string val, string exeFile = null)
        {
            string file = exeFile ?? Assembly.GetEntryAssembly().Location;

            if (!File.Exists(file)) throw new FileNotFoundException("未找到应用程序文件:" + exeFile, file);
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            bool exist = config.AppSettings.Settings.AllKeys.Contains(key);
            if (exist)
            {
                config.AppSettings.Settings.Remove(key);
            }
            config.AppSettings.Settings.Add(key, val);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
