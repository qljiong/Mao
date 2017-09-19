using Mao.Infrastructure.Util;
using NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Db.Base
{
    /// <summary>
    /// 抽象数据库工具类
    /// </summary>
    public abstract class AbstractDbUtil
    {
        #region Hibernate IQuery参数， 可统一处理

        /// <summary>
        /// 为NH的query准备参数
        /// </summary>
        /// <param name="query">NH的query</param>
        /// <param name="args">实参数组</param>
        public void SetParametersForNhQuery(IQuery query, params object[] args)
        {
            Dictionary<string, object> paraDict =
                AssembleParameterDict(query.QueryString, args);
            foreach (KeyValuePair<string, object> pair in paraDict)
            {
                if (pair.Value == null)
                {
                    LogUtil.Default.WarnFormat("IQuery query {0} parameter '{1}' is null; Use type <string> for default.", query.QueryString, pair.Key);
                    query.SetParameter(pair.Key, (string)pair.Value);
                }
                else
                {
                    if (pair.Value is IList)//列表类型
                        query.SetParameterList(pair.Key, (IList)pair.Value);
                    else
                        query.SetParameter(pair.Key, pair.Value);
                }

            }
        }

        // 解析sql语句，组合实参，得到参数词典
        public Dictionary<string, object> AssembleParameterDict(string sql, params object[] args)
        {
            List<string> paraList = RegexUtil.ParseParameterNames(sql, this.ParameterPattern);
            if (paraList.Count != args.Count())
            {
                string argNames =
                    paraList.Aggregate(string.Empty, (current, variable) => current + (variable + ","));
                string argValues =
                    args.Aggregate(string.Empty, (current, variable) => current + (variable + ","));

                string errorMsg = string.Format("Sql {0} parameters count unmatch:\r\nparameter:{1}:{2}\r\nargument:{3}:{4}",
                    sql, paraList.Count, argNames, args.Count(), argValues);
                LogUtil.SqlLogger.Error(errorMsg);
                throw new Exception(errorMsg);
            }

            Dictionary<string, object> parameterDict = new Dictionary<string, object>();
            int i = 0;
            foreach (var arg in args)
            {
                parameterDict.Add(paraList[i], arg);
                i++;
            }
            return parameterDict;
        }

        public Dictionary<string, object> AssembleParameterDictDynamic(string sql, object args)
        {
            List<string> paraList = RegexUtil.ParseParameterNames(sql, this.ParameterPattern);

            Type t = args.GetType();
            var ps = t.GetProperties();
            Dictionary<string, object> argsDict = ps.ToDictionary(p => p.Name.ToLower(), p => p.GetValue(args, null));
            return paraList.ToDictionary(pa => pa, pa => argsDict[pa.ToLower()]);
        }


        public DbCommand CreateDbCommand(DbConnection connection, string sql, object[] args)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            this.AddParametersForDbCommand(cmd, this.AssembleParameterDict(sql, args));
            return cmd;
        }

        public DbCommand CreateDbCommandDynamic(DbConnection connection, string sql, object args)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            this.AddParametersForDbCommand(cmd, this.AssembleParameterDictDynamic(sql, args));
            return cmd;
        }


        #endregion


        // 解析参数的正则表达式，书写标准为冒号，故此处固定为使用冒号提取参数
        public abstract string ParameterPattern { get; }

        #region Sql语句参数，无法统一处理，需数据库个性化实现[abstract]
        public abstract DbDataAdapter CreateDbDataAdapter(DbCommand cmd);

        public abstract IPageQuery CreatePagedQuery(IDataSetFactory dsf, int page, int pageSize, string sql, object[] sqlParams);

        public abstract void AddParametersForDbCommand(DbCommand cmd, Dictionary<string, object> paraDict);

        public abstract string SysDateSql { get; }

        #endregion
    }
}
