using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.DB.Base
{
    public interface IDataSetFactory
    {
        /// <summary>
        /// 来自代码示例化的数据库Util，非IOC注入，故使用get
        /// </summary>
        AbstractDbUtil DbUtil { get; }

        /// <summary>
        /// 执行Sql
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="args">参数数组，按Sql中形参的顺序指定</param>
        /// <returns>返回受影响的行数</returns>
        int ExecuteNoneQuery(string sql, params object[] args);

        /// <summary>
        /// 多套参数重复执行Sql
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="argsList">参数数组列表，按列表的长度，重复执行Sql。每个列表中的数组，按Sql中形参的顺序指定。</param>
        /// <returns>返回受影响的总行数</returns>
        int ExecuteNoneQueryLoop(string sql, List<object[]> argsList);

        /// <summary>
        /// 获取XAL数据库的系统时间
        /// </summary>
        /// <returns>返回XAL数据库的系统时间</returns>
        DateTime GetSysDate();

        IPageQuery LoadPagedDataTable(string sql, int pageNum, int pageSize, params object[] args);

        /// <summary>
        /// 查找符合条件的第一行值，若结果集为空，则返回null
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="argValues">实参数组</param>
        /// <returns>返回结果</returns>
        object ExecuteScalar(string sql, params object[] argValues);

        /// <summary>
        /// 提取DataSet
        /// </summary>
        /// <param name="sql">Sql,参数请使用 ?p0,?p1,?p2...，与argValues顺序一致，byName=True</param>
        /// <param name="args">实参数组</param>
        /// <returns>结果数据集</returns>
        DataSet GetDataSet(string sql, params object[] args);

        DataSet GetDataSetDynamic(string sql, object argValues);
    }
}
