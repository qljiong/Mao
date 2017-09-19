using System;
using System.Data;
using Mao.Infrastructure.Util;

namespace Mao.Infrastructure.Db.Base
{
    public abstract class PagedQueryBase : IPageQuery
    {
        protected readonly IDataSetFactory Dsf;
        protected readonly string Sql;
        protected readonly int Page;
        protected readonly int PageSize;
        protected readonly object[] SqlParams;
        protected string OrderByPart;

        protected PagedQueryBase(IDataSetFactory dsf, int page, int pageSize, string sql, object[] sqlParams)
        {
            this.Dsf = dsf;
            this.Sql = sql;
            this.Page = page;
            this.PageSize = pageSize;
            this.SqlParams = sqlParams;
        }

        public PagedEntities<T> QueryEntities<T>()
        {
            var page = this.Query();
            return new PagedEntities<T>(page.Total, DbUtil.LoadEntities<T>((DataTable)page.Result));
        }

        public Pager Query()
        {
            return this.LoadDataTable();
        }

        public IPageQuery OrderBy(string orderWith)
        {
            this.OrderByPart = orderWith;
            return this;
        }

        protected abstract Pager LoadDataTable();

        protected virtual string InsertWhere(string sql, string where)
        {
            var sqlL = sql.ToLower();
            var whereCause = " where " + where + " ";
            if (sqlL.Contains("where"))
            {
                var wIdx = sqlL.IndexOf("where", StringComparison.Ordinal);
                return sql.Insert(wIdx + 5, " " + @where + " and ");
            }
            // TODO:
            return sql + whereCause;
        }

        /*
         * ()中为匹配项
         * \S+(?=\.\S+)                         =>      (t).xx
         * (?<=select)(.*?)(?=from)             =>      select (xxx)  from
         * (?<=\w+\s+)[\S]+\s*,                 =>      t.res_id(                Id,)
         */
        protected static readonly string SelectReg = "(?<=select)(.*?)(?=from)";
        protected static readonly string AliasReg = @"(?<=\w+\s+)[\S]+\s*,";
        protected static readonly string TbReg = @"\S+(?=\.\S+)";
    }
}