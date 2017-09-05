using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Util
{
    public static class RegexUtil
    {
        /// <summary>
        /// 分解sql语句，取得形参数组
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="pattern">sql参数的正则表达式</param>
        /// <returns></returns>
        public static List<string> ParseParameterNames(string sql, string pattern)
        {
            MatchCollection mc = Regex.Matches(sql, pattern);
            List<string> paraList = new List<string>();

            foreach (Match match in mc)
            {
                string para = match.Groups[1].Value;

                if (!paraList.Contains(para))
                {
                    paraList.Add(para);
                }
            }
            return paraList;
        }

        /// <summary>
        /// 是否符合某个指定的正则
        /// </summary>
        /// <param name="input">目标文本</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>是否符合正则</returns>
        public static bool IsMatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 查找符合正则表达式的集合
        /// </summary>
        /// <param name="input">目标文本</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>符合的集合</returns>
        public static MatchCollection Matchs(string input, string pattern)
        {
            return Regex.Matches(input, pattern);
        }
    }
}
