using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Models
{
    public class SysUser
    {
        public virtual Guid Id { get; protected set; }
        public virtual string UsrName { get; set; }
        public virtual string UsrMail { get; set; }
        public virtual string UsrGender { get; set; }
    }
}
