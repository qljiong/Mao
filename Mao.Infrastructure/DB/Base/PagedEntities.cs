using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.DB.Base
{
    public class PagedEntities<T>
    {
        public int Total { get; set; }
        public IList<T> Result { get; set; }

        public PagedEntities()
            : this(0, null)
        {
        }

        public PagedEntities(int total, IList<T> entites)
        {
            this.Total = total;
            this.Result = entites;
        }
    }
}
