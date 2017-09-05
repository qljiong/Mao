using AopAlliance.Intercept;
using Mao.Infrastructure.Util;
using Spring.Transaction.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.DB
{
    //以 Spring.Transaction.Interceptor.TransactionInterceptor 为模板

    /// <summary>
    /// An AOP Alliance <see cref="AopAlliance.Intercept.IMethodInterceptor"/> providing
    /// declarative transaction management using the common Spring.NET transaction infrastructure.
    /// </summary>
    /// <remarks>
    /// <p>
    /// That class contains the necessary calls into Spring.NET's underlying
    /// transaction API: subclasses such as this are responsible for calling
    /// superclass methods such as
    /// <see cref="Spring.Transaction.Interceptor.TransactionAspectSupport.CreateTransactionIfNecessary(MethodInfo, Type) "/>
    /// in the correct order, in the event of normal invocation return or an exception. 
    /// </p>
    /// <p>
    /// <see cref="Spring.Transaction.Interceptor.TransactionInterceptor"/>s are thread-safe.
    /// </p>
    /// </remarks>
    /// <author>Rod Johnson</author>
    /// <author>Juergen Hoeller</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Mark Pollack (.NET)</author>
    [Serializable]
    public class FocTxInterceptor : TransactionAspectSupport, IMethodInterceptor
    {
        /// <summary>
        /// AOP Alliance invoke call that handles all transaction plumbing. 
        /// </summary>
        /// <param name="invocation">
        /// The method that is to execute in the context of a transaction.
        /// </param>
        /// <returns>The return value from the method invocation.</returns>
        public object Invoke(IMethodInvocation invocation)
        {
            // Work out the target class: may be <code>null</code>.
            // The TransactionAttributeSource should be passed the target class
            // as well as the method, which may be from an interface.
            Type targetType = (invocation.This != null) ? invocation.This.GetType() : null;

            // If the transaction attribute is null, the method is non-transactional.
            TransactionInfo txnInfo = CreateTransactionIfNecessary(invocation.Method, targetType);

            object returnValue;
            try
            {
                var method = invocation.Method;
                var interceptors = method.GetCustomAttributes(typeof(TxInterceptorAttribute), true);

                var txInterceptor = new InterceptContext
                {
                    Arguments = invocation.Arguments
                };

                // on excuting
                foreach (IInterceptor interceptor in interceptors)
                {
                    interceptor.Before(txInterceptor);
                }

                // This is an around advice.
                // Invoke the next interceptor in the chain.
                // This will normally result in a target object being invoked.
                returnValue = invocation.Proceed();

                // on executed
                foreach (IInterceptor interceptor in interceptors)
                {
                    interceptor.After(txInterceptor);
                }

                // 没有异常，但事务状态不正确，可能是底层DB Exception 被人为 swallow了.
                if (txnInfo.TransactionStatus.RollbackOnly)
                {
                    string msg = string.Format(
                        "[Tx] txInfo.RollbackOnly == true,FocTxInterceptor do tx rollback.maybe caller swallow the exception.");
                    var swallowDb = new DbErrorSwallowed(invocation.TargetType.FullName,
                        invocation.Method.Name, returnValue, msg);

                    CompleteTransactionAfterThrowing(txnInfo, swallowDb);
                    LogUtil.Default.WarnFormat(swallowDb.Message);
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                // target invocation exception
                CompleteTransactionAfterThrowing(txnInfo, ex);
                LogUtil.Default.ErrorFormat(
                "[Tx] CompleteTransactionAfterThrowing for {0} from type {1}",
                invocation.Method.Name,
                targetType.FullName);
                throw;
            }
            finally
            {
                CleanupTransactionInfo(txnInfo);
                LogUtil.Default.InfoFormat(
                "[Tx] CleanupTransactionInfo for {0} from type {1}",
                invocation.Method.Name,
                targetType.FullName);
            }
            CommitTransactionAfterReturning(txnInfo);

            LogUtil.Default.InfoFormat(
                "[Tx] CommitTransactionAfterReturning for {0} from type {1}",
                invocation.Method.Name,
                targetType.FullName);
            return returnValue;
        }
    }

    //不遵守Exception命名规范，其本身不是Exception，只是CompleteTransactionAfterThrowing操作需要一个异常。
    //用此异常来表明在可允许条件下，Db错误被人为吞噬了.
    internal class DbErrorSwallowed : Exception
    {
        public string TargetName { get; set; }
        public string MethodName { get; set; }

        public object ReturnValue { get; set; }

        public DbErrorSwallowed(string targetName, string methodName, object returnValue, string msg)
            : base(msg)
        {
            TargetName = targetName;
            MethodName = methodName;
            ReturnValue = returnValue;
        }

        public override string Message
        {
            get
            {
                return string.Format("{0}.{1}, return:{2}, {3}", TargetName, MethodName, ReturnValue, base.Message);
            }
        }
    }
}
