using Spring.Aop.Framework;
using Spring.Aop.Support;
using Spring.Transaction.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.DB
{
    /// <summary>
    /// Advisor driven by a <see cref="Spring.Transaction.Interceptor.ITransactionAttributeSource"/>, used to include
    /// a <see cref="Spring.Transaction.Interceptor.TransactionInterceptor"/> for methods that
    /// are transactional.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Because the AOP framework caches advice calculations, this is normally
    /// faster than just letting the <see cref="Spring.Transaction.Interceptor.TransactionInterceptor"/> 
    /// run and find out itself that it has no work to do.
    /// </p>
    /// </remarks>
    /// <author>Rod Johnson</author>
    /// <author>Griffin Caprio (.NET)</author>
    [Serializable]
    public class FocTxAttributeSourceAdvisor : StaticMethodMatcherPointcutAdvisor
    {
        #region Fields

        private ITransactionAttributeSource _transactionAttributeSource;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionAttributeSourceAdvisor"/> class.
        /// </summary>
        /// <remarks>
        /// 	<p>
        /// This is an abstract class, and as such has no publicly
        /// visible constructors.
        /// </p>
        /// </remarks>
        public FocTxAttributeSourceAdvisor()
        {
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="Spring.Transaction.Interceptor.TransactionAttributeSourceAdvisor"/> class.
        /// </summary>
        /// <param name="transactionInterceptor">
        /// The pre-configured transaction interceptor.
        /// </param>
        public FocTxAttributeSourceAdvisor(FocTxInterceptor transactionInterceptor)
            : base(transactionInterceptor)
        {
            SetTxAttributeSource(transactionInterceptor);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the transaction interceptor.
        /// </summary>
        /// <value>The transaction interceptor.</value>
        public FocTxInterceptor TransactionInterceptor
        {
            set
            {
                //TODO refactor
                Advice = value;
                SetTxAttributeSource(value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tests the input method to see if it's covered by the advisor.
        /// </summary>
        /// <param name="method">The method to match.</param>
        /// <param name="targetClass">The <see cref="System.Type"/> to match against.</param>
        /// <returns>
        /// <b>True</b> if the supplied <paramref name="method"/> is covered by the advisor.
        /// </returns>
        public override bool Matches(MethodInfo method, Type targetClass)
        {
            return (_transactionAttributeSource.ReturnTransactionAttribute(method, targetClass) != null);
        }

        /// <summary>
        /// Sets the tx attribute source.
        /// </summary>
        /// <param name="transactionInterceptor">The transaction interceptor.</param>
        protected void SetTxAttributeSource(FocTxInterceptor transactionInterceptor)
        {
            if (transactionInterceptor.TransactionAttributeSource == null)
            {
                throw new AopConfigException("Cannot construct a FocTxAttributeSourceAdvisor using a " +
                                             "FocTxInterceptor that has no TransactionAttributeSource configured.");
            }
            _transactionAttributeSource = transactionInterceptor.TransactionAttributeSource;
        }

        #endregion
    }
}
