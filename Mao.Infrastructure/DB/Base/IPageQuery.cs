using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.DB.Base
{
    public interface IPageQuery
    {
        Pager Query();
        PagedEntities<T> QueryEntities<T>();
        IPageQuery OrderBy(string orderWith);
    }
}
