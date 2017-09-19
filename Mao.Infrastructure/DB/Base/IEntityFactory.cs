using NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Db.Base
{
    public interface IEntityFactory
    {
        /// <summary>
        /// 来自IOC注入的Dsf，所以使用get;set;
        /// </summary>
        IDataSetFactory Dsf { get; set; }

        /// <summary>
        /// 主键查找唯一值，若结果!=1抛出异常
        /// </summary>
        /// <typeparam name="T">Domain类型</typeparam>
        /// <param name="id">主键值</param>
        /// <returns>Domain对象</returns>
        T LoadEntityById<T>(object id);

        /// <summary>
        ///  主键查找唯一值，若无则返回Null
        /// </summary>
        /// <typeparam name="T">Domain类型</typeparam>
        /// <param name="id">主键值</param>
        /// <returns>Domain对象或Null</returns>
        T GetEntityById<T>(object id);

        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exist<T>(object id);

        #region Hql 取Domain对象、对象列表
        /// <summary>
        /// 取Domain唯一对象,若结果!=1抛出异常
        /// </summary>
        /// <typeparam name="T">Domain类型</typeparam>
        /// <param name="hql">Hql语句</param>
        /// <param name="args">参数数组</param>
        /// <returns>Domain唯一对象</returns>
        /// <exception cref="HibernateException">结果不等于1,抛出异常</exception>
        T LoadEntityByHql<T>(string hql, params object[] args);

        /// <summary>
        /// 取Domain唯一对象,若无则返回Null
        /// </summary>
        /// <typeparam name="T">Domain类型</typeparam>
        /// <param name="hql">Hql语句</param>
        /// <param name="args">参数数组</param>
        /// <returns>Domain唯一对象,若无则返回Null</returns>
        /// <exception cref="HibernateException">结果>1,抛出异常</exception>
        T GetEntityByHql<T>(string hql, params object[] args);

        /// <summary>
        /// 取Domain对象列表,使用参数数组
        /// </summary>
        /// <typeparam name="T">Domain类型</typeparam>
        /// <param name="hql">Hql语句</param>
        /// <param name="args">参数数组</param>
        /// <returns>Domain对象列表</returns>
        IList<T> LoadEntitiesByHql<T>(string hql, params object[] args);

        NhPagedQuery<T> LoadPagedEntities<T>(int pageNum, int pageSize);

        #endregion

        #region Sql 取Domain对象、字段值

        #endregion

        #region Insert
        /// <summary>
        /// 新增对象
        /// </summary>

        void Insert(object entity);

        /// <summary>
        /// 新增对象列表
        /// </summary>
        /// <param name="entities"></param>
        void InsertMulti(IEnumerable entities);

        /// <summary>
        /// 新增对象，但不立刻执行sql语句
        /// </summary>
        void InsertWithoutFlush(object entity);

        #endregion

        #region Update

        void Update(object entity);

        void UpdateMulti(IEnumerable entities);

        void UpdateWithoutFlush(object entity);

        #endregion 

        #region InsertOrUpdate

        /// <summary>
        /// 新增 或 更新对象
        /// </summary>
        void InsertOrUpdate(object entity);


        /// <summary>
        /// 新增 或 更新对象，但不立刻执行sql语句
        /// </summary>
        void InsertOrUpdateWithoutFlush(object entity);
        #endregion

        #region Delete

        //todo 04783 session 有对象时的case
        //session 无对象时的case

        /// <summary>
        /// 根据ID删除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        void DeleteById<T>(object id);

        /// <summary>
        /// 删除对象
        /// </summary>
        void Delete(object entity);

        /// <summary>
        /// 删除对象，但不立刻执行sql语句
        /// </summary>
        /// <param name="entity"></param>
        void DeleteWithoutFlush(object entity);

        #endregion

        ISession CurrentSession();

        /// <summary>
        /// 使用SaveOrUpdateWithoutFlush后，需要的手工Flush
        /// </summary>
        void FlushSession();

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="hql">HQL语句</param>
        /// <param name="args">参数</param>
        /// <returns>更新条数</returns>
        int ExecuteUpdateOrDelete(string hql, params object[] args);

        NhQuery<T> LoadEntities<T>();
    }
}
