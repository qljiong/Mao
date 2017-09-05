using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Excep
{
    public abstract class BaseException : Exception
    {
        public abstract int Code { get; }

        public abstract string Description { get; }

        public BaseException(string msg) : base(msg)
        {

        }

        public override string Message
        {
            get
            {
                return string.Format("#{0}#{1}#{2}", Code, Description, base.Message);
            }
        }
    }
}
