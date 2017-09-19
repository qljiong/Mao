using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Common;
using Mao.Infrastructure.Db.Base;
using Mao.Infrastructure.Util;

namespace Mao.Infrastructure.Db.Impl
{
    // MsSql版本实现
    public sealed class DbUtilMsSql : AbstractDbUtil
    {
        public override string ParameterPattern
        {
            get
            {
                return @"\W+@(\w+)";
            }
        }

        public override DbDataAdapter CreateDbDataAdapter(DbCommand cmd)
        {
            return new SqlDataAdapter(cmd as SqlCommand);
        }

        public override IPageQuery CreatePagedQuery(IDataSetFactory dsf, int page, int pageSize, string sql, object[] sqlParams)
        {
            return new MsSqlPageQuery(dsf, page, pageSize, sql, sqlParams);
        }

        public override void AddParametersForDbCommand(DbCommand cmd, Dictionary<string, object> paraDict)
        {
            SqlCommand sqlCmd = cmd as SqlCommand;
            if (sqlCmd == null)
            {
                throw new Exception(string.Format("DbCommand is null while adding parameters."));
            }
            foreach (KeyValuePair<string, object> pair in paraDict)
            {
                if (pair.Value == null)
                    LogUtil.Default.WarnFormat("SqlCommand语句{0}参数{1}为null.", cmd.CommandText, pair.Key);
                var param = pair;
                var p = sqlCmd.Parameters.AddWithValue(param.Key, param.Value);
                if (param.Value is DateTime)
                {
                    p.DbType = DbType.Date;
                }
            }
        }

        public override string SysDateSql
        {
            get
            {
                return "select getdate() as sysdate";
            }
        }
    }
}
