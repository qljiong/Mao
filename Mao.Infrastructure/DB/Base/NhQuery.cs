using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Db.Base
{
    public class NhQuery<T> : NhQueryBase<T, NhQuery<T>>
    {
        public NhQuery(IEntityFactory ef)
            : base(ef)
        {
        }

        public IList<T> QueryMaxOn(Expression<Func<T, object>> expression)
        {
            this.AddOrder(expression, false);

            ICriteria criteria = this.Ef.CurrentSession().CreateCriteria(typeof(T));

            this.PrepareCriteria(criteria);

            return criteria.SetMaxResults(1)
                .SetFirstResult(0)
                .List<T>();
        }

        public IList<T> Query()
        {
            ICriteria criteria = this.Ef.CurrentSession().CreateCriteria(typeof(T));

            this.PrepareCriteria(criteria);

            return criteria.List<T>();
        }
    }
}
