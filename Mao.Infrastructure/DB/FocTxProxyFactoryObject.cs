﻿using Spring.Aop;
using Spring.Aop.Framework;
using Spring.Aop.Framework.Adapter;
using Spring.Aop.Support;
using Spring.Aop.Target;
using Spring.Core.TypeResolution;
using Spring.Objects.Factory;
using Spring.Transaction;
using Spring.Transaction.Interceptor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Db
{
    /// <summary>
    /// Proxy factory object for simplified declarative transaction handling.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Alternative to the standard AOP <see cref="Spring.Aop.Framework.ProxyFactoryObject"/>
    /// with a <see cref="Spring.Transaction.Interceptor.TransactionInterceptor"/>.
    /// </p>
    /// <p>
    /// This class is intended to cover the <i>typical</i> case of declarative
    /// transaction demarcation: namely, wrapping a (singleton) target object with a
    /// transactional proxy, proxying all the interfaces that the target implements.
    /// </p>
    /// <p>
    /// Internally, a <see cref="Spring.Transaction.Interceptor.TransactionInterceptor"/> 
    /// instance is used, but the user of this class does not have to care. Optionally, an
    /// <see cref="Spring.Aop.IPointcut"/> can be specified to cause conditional invocation of
    /// the underlying <see cref="Spring.Transaction.Interceptor.TransactionInterceptor"/>.
    /// </p> 
    /// <p>
    /// The
    /// <see cref="Spring.Transaction.Interceptor.TransactionProxyFactoryObject.PreInterceptors"/>
    /// and
    /// <see cref="Spring.Transaction.Interceptor.TransactionProxyFactoryObject.PostInterceptors"/>
    /// properties can be set to add additional interceptors to the mix.
    /// </p>
    /// </remarks>
    /// <author>Juergen Hoeller</author>
    /// <author>Dmitriy Kopylenko</author>
    /// <author>Rod Johnson</author>
    /// <author>Griffin Caprio (.NET)</author>
    public class FocTxProxyFactoryObject : ProxyConfig, IFactoryObject, IInitializingObject
    {
        private FocTxInterceptor _transactionInterceptor;
        private object _target;
        private IList<Type> _proxyInterfaces;
        private TruePointcut _pointcut;
        private object[] _preInterceptors;
        private object[] _postInterceptors;
        private IAdvisorAdapterRegistry _advisorAdapterRegistry = GlobalAdvisorAdapterRegistry.Instance;
        private object _proxy;

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="Spring.Transaction.Interceptor.TransactionProxyFactoryObject"/> class.
        /// </summary>
        public FocTxProxyFactoryObject()
        {
            _transactionInterceptor = new FocTxInterceptor();
        }

        /// <summary>
        /// Set the transaction manager for this factory.
        /// </summary>
        /// <remarks>
        /// It is this instance that will perform actual transaction management: this class is
        /// just a way of invoking it.
        /// </remarks>
        public IPlatformTransactionManager PlatformTransactionManager
        {
            set { _transactionInterceptor.TransactionManager = value; }
        }

        /// <summary>
        /// Set the target object, i.e. the object to be wrapped with a transactional proxy.
        /// </summary>
        /// <remarks>
        /// <p>
        /// The target may be any object, in which case a
        /// <see cref="Spring.Aop.Target.SingletonTargetSource"/> will
        /// be created. If it is a <see cref="EmptyTargetSource"/>, no wrapper
        /// <see cref="EmptyTargetSource"/> is created: this enables the use of a pooling
        /// or prototype <see cref="EmptyTargetSource"/>.
        /// </p>
        /// </remarks>
        public object Target
        {
            set { _target = value; }
        }

        /// <summary>
        /// Specify the set of interfaces being proxied.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If left null (the default), the AOP infrastructure works
        /// out which interfaces need proxying by analyzing the target,
        /// proxying <b>all</b> of the interfaces that the target object implements.
        /// </p>
        /// </remarks>
        public string[] ProxyInterfaces
        {
            set { _proxyInterfaces = TypeResolutionUtils.ResolveInterfaceArray(value); }
        }

        /// <summary>
        /// Set properties with method names as keys and transaction attribute
        /// descriptors as values.
        /// </summary>
        /// <remarks>
        /// <p>
        /// The various transaction attribute descriptors are parsed via
        /// an instance of the
        /// <see cref="Spring.Transaction.Interceptor.TransactionAttributeEditor"/> class.
        /// </p>
        /// <note>
        /// Method names are always applied to the target class, no matter if defined in an
        /// interface or the class itself.
        /// </note>
        /// <p>
        /// Internally, a
        /// <see cref="Spring.Transaction.Interceptor.NameMatchTransactionAttributeSource"/>
        /// will be created from the given properties.
        /// </p>
        /// </remarks>
        /// <example>
        /// <p>
        /// An example string (method name and transaction attributes) might be:
        /// </p>
        /// <p>
        /// key = "myMethod", value = "PROPAGATION_REQUIRED,readOnly".
        /// </p>
        /// </example>
        public NameValueCollection TransactionAttributes
        {
            set { _transactionInterceptor.TransactionAttributes = value; }
        }

        /// <summary>
        /// Set the transaction attribute source which is used to find transaction
        /// attributes.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If specifying a <see cref="System.String"/> property value, an
        /// appropriate <see cref="System.ComponentModel.TypeConverter"/>
        /// implementation will create a
        /// <see cref="Spring.Transaction.Interceptor.MethodMapTransactionAttributeSource"/>
        /// from the value.
        /// </p>
        /// </remarks>
        public ITransactionAttributeSource TransactionAttributeSource
        {
            set { _transactionInterceptor.TransactionAttributeSource = value; }
        }

        /// <summary>
        /// Set a pointcut, i.e an object that can cause conditional invocation
        /// of the <see cref="Spring.Transaction.Interceptor.TransactionInterceptor"/>
        /// depending on method and attributes passed.
        /// </summary>
        /// <remarks>
        /// <note>
        /// Additional interceptors are always invoked.
        /// </note>
        /// </remarks>
        public TruePointcut TruePointcut
        {
            set { _pointcut = value; }
        }

        /// <summary>
        /// Set additional interceptors (or advisors) to be applied before the
        /// implicit transaction interceptor.
        /// </summary>
        public object[] PreInterceptors
        {
            set { _preInterceptors = value; }
        }

        /// <summary>
        /// Set additional interceptors (or advisors) to be applied after the
        /// implicit transaction interceptor.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Note that this is just necessary if you rely on those interceptors in general.
        /// </p>
        /// </remarks>
        public object[] PostInterceptors
        {
            set { _postInterceptors = value; }
        }

        /// <summary>
        /// Specify the <see cref="Spring.Aop.Framework.Adapter.IAdvisorAdapterRegistry"/> to use.
        /// </summary>
        /// <remarks>The default instance is the global AdvisorAdapterRegistry.</remarks>
        public IAdvisorAdapterRegistry AdvisorAdapterRegistry
        {
            set { _advisorAdapterRegistry = value; }
        }

        #region IFactoryObject Members
        /// <summary>
        /// Returns the object <see cref="System.Type"/> for this proxy factory.
        /// </summary>
        public Type ObjectType
        {
            get
            {
                if (_proxy != null)
                {
                    return _proxy.GetType();
                }
                if (_target != null && _target is ITargetSource)
                {
                    return _target.GetType();
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Returns the object wrapped by this proxy factory.
        /// </summary>
        /// <returns>The target object proxy.</returns>
        public object GetObject()
        {
            return _proxy;
        }

        /// <summary>
        /// Is this object a singleton?  Always returns <b>true</b> in this implementation.
        /// </summary>
        public bool IsSingleton
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region IInitializingObject Members
        /// <summary>
        /// Method run after all the properties have been set for this object.
        /// Responsible for actual proxy creation.
        /// </summary>
        public void AfterPropertiesSet()
        {
            _transactionInterceptor.AfterPropertiesSet();

            if (_target == null)
            {
                throw new ArgumentException("'target' is required.");
            }
            ProxyFactory proxyFactory = new ProxyFactory();

            if (_preInterceptors != null)
            {
                for (int i = 0; i < _preInterceptors.Length; i++)
                {
                    proxyFactory.AddAdvisor(_advisorAdapterRegistry.Wrap(_preInterceptors[i]));
                }
            }
            if (_pointcut != null)
            {
                IAdvisor advice = new DefaultPointcutAdvisor(_pointcut, _transactionInterceptor);
                proxyFactory.AddAdvisor(advice);
            }
            else
            {
                proxyFactory.AddAdvisor(new FocTxAttributeSourceAdvisor(_transactionInterceptor));
            }
            if (_postInterceptors != null)
            {
                for (int i = 0; i < _postInterceptors.Length; i++)
                {
                    proxyFactory.AddAdvisor(_advisorAdapterRegistry.Wrap(_postInterceptors[i]));
                }
            }
            proxyFactory.CopyFrom(this);
            proxyFactory.TargetSource = createTargetSource(_target);
            if (_proxyInterfaces != null)
            {
                proxyFactory.Interfaces = _proxyInterfaces;
            }
            else if (!ProxyTargetType)
            {
                if (_target is ITargetSource)
                {
                    throw new AopConfigException("Either 'ProxyInterfaces' or 'ProxyTargetType' is required " +
                        "when using an ITargetSource as 'target'");
                }
                proxyFactory.Interfaces = AopUtils.GetAllInterfaces(_target);
            }
            _proxy = proxyFactory.GetProxy();
        }
        #endregion

        /// <summary>
        /// Set the target or <see cref="Spring.Aop.ITargetSource"/>.
        /// </summary>
        /// <param name="target">
        /// The target. If this is an implementation of the <see cref="Spring.Aop.ITargetSource"/>
        /// interface, it is used as our <see cref="Spring.Aop.ITargetSource"/>; otherwise it is
        /// wrapped in a <see cref="Spring.Aop.Target.SingletonTargetSource"/>.
        /// </param>
        /// <returns>An <see cref="Spring.Aop.ITargetSource"/> for this object.</returns>
        protected ITargetSource createTargetSource(object target)
        {
            if (target is ITargetSource)
            {
                return (ITargetSource)target;
            }
            else
            {
                return new SingletonTargetSource(target);
            }
        }
    }
}
