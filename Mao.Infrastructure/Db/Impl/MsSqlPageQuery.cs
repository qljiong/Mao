using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mao.Infrastructure.Db.Base;
using Mao.Infrastructure.Util;

namespace Mao.Infrastructure.Db.Impl
{
    public class MsSqlPageQuery : PagedQueryBase
    {
        public MsSqlPageQuery(IDataSetFactory dsf, int page, int pageSize, string sql, object[] sqlParams)
            : base(dsf, page, pageSize, sql, sqlParams)
        {
        }

        protected override Pager LoadDataTable()
        {
            Pager result = new Pager();

            var start = this.PageSize * (this.Page - 1);

            var sql = this.Sql;

            var selectRegex = new Regex(SelectReg, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var fieldSql = selectRegex.Match(sql).Value;
            var fromSql = sql.Replace(fieldSql, " ").ReplaceFirst("select", " ").Trim();

            var oderby = " order by " + (this.OrderByPart ?? "CURRENT_TIMESTAMP") + " ";
            var innerSelect = Regex.Replace(fieldSql.Trim() + ",", AliasReg, ",");

            innerSelect += "row_number() over (" + oderby + ") as _sort_row ";
            innerSelect = "select " + innerSelect + fromSql;

            const string TABLE_NAME = " tx_______";
            var sqlB = Regex.Replace("select top(" + this.PageSize + ") " + fieldSql, TbReg, TABLE_NAME) + " from ({0}) {1}";
            sqlB = this.InsertWhere(sqlB, string.Format(" {0}._sort_row > " + start, TABLE_NAME));
            sqlB = string.Format(sqlB, innerSelect, TABLE_NAME);
            sqlB += string.Format(" order by {0}._sort_row", TABLE_NAME);

            var countSql = sql.Replace(fieldSql, " count(*) ");
            result.Total = Convert.ToInt32(this.Dsf.ExecuteScalar(countSql, argValues: this.SqlParams));

            List<object> paramList = this.SqlParams.ToList();
            result.Result = this.Dsf.GetDataSet(sqlB, args: paramList.ToArray()).Tables[0];

            return result;
        }
    }
}
