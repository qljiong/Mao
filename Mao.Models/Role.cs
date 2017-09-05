using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Models
{
    public class Role
    {
        public virtual Guid Id { get; protected set; }
        public virtual string RoleName { get; set; }
        public virtual bool IsRead { get; set; }
        public virtual bool IsWrite { get; set; }
    }
}
