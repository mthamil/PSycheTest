using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PSycheTest.Exceptions
{
	/// <summary>
	/// Base exception for all assertions.
	/// </summary>
	[Serializable]
	public class AssertionException : Exception
	{
		/// <summary>
		/// A user provided message
		/// </summary>
		public string UserMessage { get; protected set; }

		/// <see cref="Exception.Message"/>
		public override string Message
		{
			get
			{
				return UserMessage;
			}
		}

		/// <summary>
		/// Initializes a new <see cref="AssertionException"/>.
		/// </summary>
		public AssertionException() { }

		/// <summary>
		/// Initializes a new <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="userMessage">A user provided message</param>
		public AssertionException(string userMessage)
		  : base(userMessage)
		{
			UserMessage = userMessage;
		}

		/// <summary>
		/// Initializes a new <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="userMessage">A user provided message</param>
		/// <param name="innerException">The exception that caused this exception</param>
		protected AssertionException(string userMessage, Exception innerException)
		  : base(userMessage, innerException)
		{
		}

		/// <see cref="Exception(SerializationInfo,StreamingContext)"/>
		protected AssertionException(SerializationInfo info, StreamingContext context)
		  : base(info, context)
		{
			UserMessage = info.GetString("UserMessage");
		}

		/// <see cref="Exception.GetObjectData"/>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			info.AddValue("UserMessage", UserMessage);
			base.GetObjectData(info, context);
		}
	}
}