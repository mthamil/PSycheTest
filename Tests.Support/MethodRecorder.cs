using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Tests.Support
{
	/// <summary>
	/// Proxy that records method invocations.
	/// </summary>
	public class MethodRecorder<T> : RealProxy
	{
		/// <summary>
		/// Creates a new interceptor that records method invocations.
		/// </summary>
		public MethodRecorder()
			: base(typeof(T))
		{
			_proxy = new Lazy<T>(() => (T)base.GetTransparentProxy());
		}

		/// <summary>
		/// The underlying proxy.
		/// </summary>
		public T Proxy
		{
			get { return _proxy.Value; }
		}

		/// <summary>
		/// The most recent invocation made on the proxy.
		/// </summary>
		public IMethodCallMessage LastInvocation { get; private set; }

		/// <see cref="RealProxy.Invoke"/>
		public override IMessage Invoke(IMessage msg)
		{
			var methodCall = msg as IMethodCallMessage;
			LastInvocation = methodCall;

			object returnValue = null;
			var method = methodCall.MethodBase as MethodInfo;
			if (method != null)
			{
				returnValue = GetDefaultValue(method.ReturnType);
			}

			return new ReturnMessage(returnValue, new object[0], 0, methodCall.LogicalCallContext, methodCall);
		}

		/// <summary>
		/// If a type is a primitive such as int, returns its default, otherwise
		/// null is returned.
		/// </summary>
		private static object GetDefaultValue(Type type)
		{
			if (type.IsValueType && type != _voidType)	// can't create an instance of Void
				return Activator.CreateInstance(type);

			return null;
		}

		private readonly Lazy<T> _proxy;

		private static readonly Type _voidType = typeof(void);
	}
}
