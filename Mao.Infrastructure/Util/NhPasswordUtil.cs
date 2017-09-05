using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Util
{
    public static class NhPasswordUtil
    {
        private const string _CONNE_STR_PASSWORD_PATTERN = @"{(\w+?)}";
        /// <summary>
        /// 对连接串中的Password加密串进行解密
        /// </summary>
        /// <param name="connectionString">可能含有密码加密串的ConnectionString</param>
        /// <returns>密码明文的ConnectionString</returns>
        public static string DecryptConnectionString(string connectionString)
        {
            int cipherPlaceHolderCount = RegexUtil.Matchs(connectionString, _CONNE_STR_PASSWORD_PATTERN).Count;

            if (cipherPlaceHolderCount == 0)
                return connectionString; //密码不加密，直接返回
            if (cipherPlaceHolderCount > 1)
                throw new Exception(
                    string.Format("连接串的密文占位符大括号只能有且只有一个，为密码配置，连接串:{0}。", connectionString)
                    );

            var mc = Regex.Match(connectionString, _CONNE_STR_PASSWORD_PATTERN);
            string plainPassword = AesUtil.Decrypt(mc.Groups[1].Value);
            string plainConntionString = Regex.Replace(connectionString, _CONNE_STR_PASSWORD_PATTERN, plainPassword);
            return plainConntionString;

        }

    }
}
