using FluentNHibernate.Conventions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Db.Base
{
    public class NhQueryBase<T, TQ> where TQ : NhQueryBase<T, TQ>
    {
        protected readonly IEntityFactory Ef;
        protected readonly List<ICriterion> CriterionList;
        protected List<Order> CriterionOrders;
        private ExpressionProcessor.ProjectionInfo _tmpProjectionInfo;
        private Dictionary<string, FetchMode> _fetchModes;

        public NhQueryBase(IEntityFactory ef)
        {
            Ef = ef;
            this.CriterionList = new List<ICriterion>();
            this.CriterionOrders = new List<Order>();
            this._fetchModes = new Dictionary<string, FetchMode>();
        }

        /// <summary>
        /// Where
        /// </summary>
        /// <param name="expression">lambda where expression</param>
        /// <returns></returns>
        public TQ Where(Expression<Func<T, bool>> expression)
        {
            ICriterion criterion = ExpressionProcessor.ProcessExpression(expression);
            this.CriterionList.Add(criterion);
            return (TQ)this;
        }
        public TQ WhereNot(Expression<Func<T, bool>> expression)
        {
            ICriterion criterion = ExpressionProcessor.ProcessExpression(expression);
            this.CriterionList.Add(new NotExpression(criterion));
            return (TQ)this;
        }

        // TODO: 先写在这里
        private static TP JoinByFunc<TP>(IList<TP> lst, Func<TP, TP, TP> func) where TP : class
        {
            if (lst.IsEmpty()) return null;
            lst = lst.Reverse().ToList();
            var ret = lst[0];
            for (var i = 1; i < lst.Count; ++i)
            {
                ret = func(lst[i], ret);
            }
            return ret;
        }

        public TQ WhereOr(params Action<NhQueryDummy<T>>[] funcs)
        {
            var orCriterionList = new List<ICriterion>();
            foreach (var func in funcs)
            {
                var tq = new NhQueryDummy<T>();
                func(tq);
                var andCriterion = JoinByFunc(tq.CriterionList, Restrictions.And);
                if (andCriterion == null) continue;
                orCriterionList.Add(andCriterion);
            }
            var orCriterion = JoinByFunc(orCriterionList, Restrictions.Or);
            if (orCriterion != null) CriterionList.Add(orCriterion);
            return (TQ)this;
        }

        /// <summary>
        /// Where 
        /// </summary>
        /// <param name="expression">lambda field expression</param>
        /// <returns></returns>
        public TQ Where(Expression<Func<T, object>> expression)
        {
            return this.And(expression);
        }

        public TQ And(Expression<Func<T, object>> expression)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this._tmpProjectionInfo = projection;
            return (TQ)this;
        }

        /// <summary>
        /// Equal to value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TQ Eq(object value)
        {
            this.CriterionList.Add(Restrictions.Eq(this._tmpProjectionInfo.AsProjection(), value));
            return (TQ)this;
        }
        /// <summary>
        /// Equal to property
        /// </summary>
        /// <param name="expression">property lambda expression</param>
        /// <returns></returns>
        public TQ EqWith(Expression<Func<T, object>> expression)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this.CriterionList.Add(Restrictions.EqProperty(this._tmpProjectionInfo.AsProjection(), projection.AsProjection()));
            return (TQ)this;
        }

        public TQ NotEq(object value)
        {
            this.CriterionList.Add(Restrictions.Not(Restrictions.Eq(this._tmpProjectionInfo.AsProjection(), value)));
            return (TQ)this;
        }
        public TQ NotEqWith(Expression<Func<T, object>> expression)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this.CriterionList.Add(Restrictions.Not(Restrictions.EqProperty(this._tmpProjectionInfo.AsProjection(), projection.AsProjection())));
            return (TQ)this;
        }

        public TQ Gt(object value)
        {
            this.CriterionList.Add(Restrictions.Gt(this._tmpProjectionInfo.AsProjection(), value));
            return (TQ)this;
        }
        public TQ GtWith(Expression<Func<T, object>> expression)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this.CriterionList.Add(Restrictions.GtProperty(this._tmpProjectionInfo.AsProjection(), projection.AsProjection()));
            return (TQ)this;
        }

        public TQ Ge(object value)
        {
            this.CriterionList.Add(Restrictions.Ge(this._tmpProjectionInfo.AsProjection(), value));
            return (TQ)this;
        }
        public TQ GeWith(Expression<Func<T, object>> expression)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this.CriterionList.Add(Restrictions.GeProperty(this._tmpProjectionInfo.AsProjection(), projection.AsProjection()));
            return (TQ)this;
        }

        public TQ Lt(object value)
        {
            this.CriterionList.Add(Restrictions.Lt(this._tmpProjectionInfo.AsProjection(), value));
            return (TQ)this;
        }
        public TQ LtWith(Expression<Func<T, object>> expression)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this.CriterionList.Add(Restrictions.LtProperty(this._tmpProjectionInfo.AsProjection(), projection.AsProjection()));
            return (TQ)this;
        }

        public TQ Le(object value)
        {
            this.CriterionList.Add(Restrictions.Le(this._tmpProjectionInfo.AsProjection(), value));
            return (TQ)this;
        }
        public TQ LeWith(Expression<Func<T, object>> expression)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this.CriterionList.Add(Restrictions.LeProperty(this._tmpProjectionInfo.AsProjection(), projection.AsProjection()));
            return (TQ)this;
        }

        public TQ Between(object l, object h)
        {
            this.CriterionList.Add(Restrictions.Between(this._tmpProjectionInfo.AsProjection(), l, h));
            return (TQ)this;
        }

        /// <summary>
        /// like sql
        /// </summary>
        /// <param name="cond">like condition</param>
        /// <param name="insensitive">case insensitive, default is false</param>
        /// <returns></returns>
        public TQ Like(object cond, bool insensitive = false)
        {
            this.CriterionList.Add(insensitive
                ? Restrictions.InsensitiveLike(this._tmpProjectionInfo.AsProjection(), cond)
                : Restrictions.Like(this._tmpProjectionInfo.AsProjection(), cond));
            return (TQ)this;
        }

        public TQ EqIgnoreCase(string value)
        {
            CriterionList.Add(Restrictions.Eq(
                Projections.SqlFunction("lower", NHibernateUtil.String, _tmpProjectionInfo.AsProjection()),
                value.ToLower()));
            return (TQ)this;
        }

        public TQ IsNotNull()
        {
            this.CriterionList.Add(Restrictions.IsNotNull(this._tmpProjectionInfo.AsProjection()));
            return (TQ)this;
        }

        public TQ IsNull()
        {
            this.CriterionList.Add(Restrictions.IsNull(this._tmpProjectionInfo.AsProjection()));
            return (TQ)this;
        }

        public TQ In(ICollection inExp)
        {
            this.CriterionList.Add(Restrictions.In(this._tmpProjectionInfo.AsProjection(), inExp));
            return (TQ)this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">linq expression</param>
        /// <param name="asc">升（默认）/降</param>
        /// <returns></returns>       
        public TQ AddOrder(Expression<Func<T, object>> expression, bool asc = true)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this.CriterionOrders.Add(new Order(projection.AsProjection(), asc));
            return (TQ)this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">linq expression</param>
        /// <param name="asc">升（默认）/降</param>
        /// <returns></returns>
        public TQ OrderBy(Expression<Func<T, object>> expression, bool asc = true)
        {
            return this.AddOrder(expression, asc);
        }

        public TQ SetFetchMode(Expression<Func<T, object>> expression, FetchMode fetchMode)
        {
            ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            this._fetchModes.Add(projection.AsProperty(), fetchMode);
            return (TQ)this;
        }

        protected void PrepareCriteria(ICriteria criteria)
        {
            foreach (var criterion in this.CriterionList)
            {
                criteria.Add(criterion);
            }
            foreach (var fetchMode in _fetchModes)
            {
                criteria.SetFetchMode(fetchMode.Key, fetchMode.Value);
            }
            foreach (var order in this.CriterionOrders)
            {
                criteria.AddOrder(order);
            }
        }
    }

    public class NhQueryDummy<TP> : NhQueryBase<TP, NhQueryDummy<TP>>
    {
        public NhQueryDummy() : base(null) { }
    }
}
