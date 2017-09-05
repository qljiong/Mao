using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.DB.Base
{
    public class NhPagedQuery<T> : NhQueryBase<T, NhPagedQuery<T>>
    {
        private readonly int _page;
        private readonly int _pageSize;

        public NhPagedQuery(IEntityFactory ef, int page, int pageSize)
            : base(ef)
        {
            this._page = page;
            this._pageSize = pageSize;
        }

        public PagedEntities<T> QueryEntities()
        {
            var page = this.Query();
            return new PagedEntities<T>(page.Total, (IList<T>)page.Result);
        }

        public Pager Query()
        {
            return this.LoadPagedEntities();
        }

        private Pager LoadPagedEntities()
        {
            int pageNum = this._page;
            int pageSize = this._pageSize;
            ICriteria criteria = this.Ef.CurrentSession().CreateCriteria(typeof(T));

            this.PrepareCriteria(criteria);

            ICriteria pageCriteria = CriteriaTransformer.Clone(criteria);
            criteria.ClearOrders();
            Pager pager = new Pager
            {
                Total = Convert.ToInt32(criteria.SetProjection(Projections.RowCount()).UniqueResult()),
                Result = pageCriteria
                    .SetMaxResults(pageSize)
                    .SetFirstResult(pageSize * (pageNum - 1))
                    .List<T>()
            };
            return pager;
        }

    }
}
