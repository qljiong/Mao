using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using NHibernate;
using Spring.Data.NHibernate.Generic.Support;
using Mao.Infrastructure.Util;
using NHibernate.Impl;

namespace Mao.Infrastructure.Db.Base
{
    public abstract class AbstractDataSetFactory : HibernateDaoSupport, IDataSetFactory
    {
        public abstract AbstractDbUtil DbUtil { get; }

        public new ISession Session // ef and dsf use the same session in a thread
        {
            get
            {
                ISession baseSession = base.Session;
                baseSession.FlushMode = FlushMode.Never; //Never, in case of commited by transaction proxy object.
                return baseSession;
            }
        }

        #region NH Session Execute

        /// <summary>
        /// 执行Sql
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="args">参数数组，按Sql中形参的顺序指定</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNoneQuery(string sql, params object[] args)
        {
            return this.ExcuteWithLog(() =>
            {
                DbCommand cmd = DbUtil.CreateDbCommand((DbConnection)this.Session.Connection, sql, args);
                return cmd.ExecuteNonQuery();
            }, sql);
        }

        /// <summary>
        /// 多套参数重复执行Sql
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="argsList">参数数组列表，按列表的长度，重复执行Sql。每个列表中的数组，按Sql中形参的顺序指定。</param>
        /// <returns>返回受影响的总行数</returns>
        public int ExecuteNoneQueryLoop(string sql, List<object[]> argsList)
        {
            return this.ExcuteWithLog(() =>
            {
                int count = 0;
                foreach (object[] argValues in argsList)
                {
                    count += ExecuteNoneQuery(sql, argValues);
                }
                return count;
            }, sql);

        }

        /// <summary>
        /// 获取XAL数据库的系统时间
        /// </summary>
        /// <returns>返回XAL数据库的系统时间</returns>
        public DateTime GetSysDate()
        {
            return Convert.ToDateTime(ExecuteScalar(DbUtil.SysDateSql));
        }

        public IPageQuery LoadPagedDataTable(string sql, int pageNum, int pageSize, params object[] args)
        {
            return this.DbUtil.CreatePagedQuery(this, pageNum, pageSize, sql, args);
        }

        /// <summary>
        /// 查找符合条件的第一行值，若结果集为空，则返回null
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="argValues">实参数组</param>
        /// <returns>返回结果</returns>
        public object ExecuteScalar(string sql, params object[] argValues)
        {
            return this.ExcuteWithLog(() =>
            {
                DbCommand cmd = DbUtil.CreateDbCommand((DbConnection)this.Session.Connection, sql, argValues);
                return cmd.ExecuteScalar();
            }, sql);
        }

        #endregion

        #region DataSet

        /// <summary>
        /// 提取DataSet
        /// AddParametersForDataAdapter的唯一调用方,在此处理GetDataSet异常和sql日志
        /// </summary>
        /// <param name="sql">Sql,参数请使用 :p0,:p1,:p2...，与argValues顺序一致，byName=True</param>
        /// <param name="argValues">实参数组</param>
        /// <returns>结果数据集</returns>
        public DataSet GetDataSet(string sql, params object[] argValues)
        {
            return this.ExcuteWithLog(() =>
            {
                DbCommand cmd = DbUtil.CreateDbCommand((DbConnection)this.Session.Connection, sql, argValues);
                return DataSetDynamic(sql, cmd);
            }, sql);
        }

        public DataSet GetDataSetDynamic(string sql, object argValues)
        {
            return this.ExcuteWithLog(() =>
            {
                DbCommand cmd = DbUtil.CreateDbCommandDynamic((DbConnection)this.Session.Connection, sql, argValues);
                return DataSetDynamic(sql, cmd);
            }, sql);
        }

        private DataSet DataSetDynamic(string sql, DbCommand cmd)
        {
            LogUtil.Default.DebugFormat("session id:{0}", Session.GetHashCode());

            try
            {
                DbDataAdapter da = DbUtil.CreateDbDataAdapter(cmd);

                _logSqlToText(sql, da);

                DataSet ds = new DataSet();
                Stopwatch sqlStopWatch = new Stopwatch();
                sqlStopWatch.Start();
                da.Fill(ds);
                sqlStopWatch.Stop();

                _logSqlTimeToNHProfiler(sqlStopWatch, da.SelectCommand.CommandText, da, ds);
                return ds;
            }
            catch (Exception ex)
            {
                SessionImpl nhSessionImpl = (SessionImpl)Session;
                LogUtil.SqlLogger.Error(nhSessionImpl.SessionId + ":" + sql, ex);
                throw;
            }
        }

        private T ExcuteWithLog<T>(Func<T> act, string sql)
        {
            if (SysEnvSpecification.IsProd())
            {
                return act();
            }
            Stopwatch wc = new Stopwatch();
            wc.Start();
            T x = act();
            wc.Stop();
            Console.WriteLine("[{0}ms] elapsed by sql: [{1}]", wc.ElapsedMilliseconds, sql);
            return x;
        }


        //前 记sql，参数
        private void _logSqlToText(string sqlOrigin, DbDataAdapter da)
        {
            // 生产环境不记录select日志
            if (SysEnvSpecification.IsProd()) return;
            string sqlReal = da.SelectCommand.CommandText;
            string paraString = string.Empty;
            for (int i = 0; i < da.SelectCommand.Parameters.Count; i++)
            {
                string pname = da.SelectCommand.Parameters[i].ParameterName;
                string pvalue = da.SelectCommand.Parameters[i].Value == null
                    ? "null"
                    : da.SelectCommand.Parameters[i].Value.ToString();
                paraString += string.Format("{0}:{1}\r\n", pname, pvalue);
            }
            if (sqlOrigin == sqlReal)
                LogUtil.SqlLogger.InfoFormat("Sql{0}{1}{0}Parameters{0}{2}",
                                             "\r\n", sqlOrigin, paraString);
            else
                LogUtil.SqlLogger.InfoFormat("Sql origin{0}{1}{0}Sql converted{0}{2}{0}Parameters{0}{3}",
                                             "\r\n", sqlOrigin, sqlReal, paraString);
        }

        //后 记执行时间
        private void _logSqlTimeToNHProfiler(Stopwatch sqlStopWatch, string sqlReal, DbDataAdapter da, DataSet ds)
        {
            // 生产环境不记录select日志
            if (SysEnvSpecification.IsProd()) return;
            List<IDataParameter> dbParas = new List<IDataParameter>(da.SelectCommand.Parameters.Count);
            dbParas.AddRange(da.SelectCommand.Parameters.Cast<IDataParameter>());
            int sqlTime = (int)sqlStopWatch.ElapsedMilliseconds;

            int rowCount = 0;
            if (ds.Tables.Count > 0)
            {
                rowCount = ds.Tables[0].Rows.Count;
            }

            SessionImpl nhSessionImpl = (SessionImpl)Session;
            LogUtil.NhProfilerSql(nhSessionImpl.SessionId.ToString(),
                sqlReal, dbParas, sqlTime, rowCount);
        }
        #endregion
    }
}
