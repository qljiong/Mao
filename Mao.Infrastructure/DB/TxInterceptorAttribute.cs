using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.DB
{
    public class InterceptContext
    {
        /// <summary>
        /// Arguments of the action in the target
        /// </summary>
        public object[] Arguments { get; set; }
        //Abstract

    }

    public interface IInterceptor
    {
        void Before(InterceptContext interceptContext);
        void After(InterceptContext interceptContext);
    }

    /// <summary>
    /// context执行拦截器
    /// </summary>
    public abstract class TxInterceptorAttribute : Attribute, IInterceptor
    {
        // action 执行之前执行Before
        public abstract void Before(InterceptContext interceptContext);

        // action 执行之后执行After
        public abstract void After(InterceptContext interceptContext);

    }
}
