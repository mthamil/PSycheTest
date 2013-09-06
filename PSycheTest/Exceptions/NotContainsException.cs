using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PSycheTest.Exceptions
{
	/// <summary>
	/// Assertion exception for when a certain value was not expected to be in a collection
	/// but it was found.
	/// </summary>
	[Serializable]
	public class NotContainsException : AssertionException
	{
		/// <summary>
		/// The value that should not be found.
		/// </summary>
		public object Unexpected { get; private set; }

		/// <see cref="Exception.Message"/>
		public override string Message
		{
			get
			{
				return String.Format("{0}{2}Found: {1}", 
				                     base.Message,
				                     Unexpected ?? "(null)", 
				                     Environment.NewLine);
			}
		}

		/// <summary>
		/// Initializes a new <see cref="NotContainsException"/>.
		/// </summary>
		/// <param name="unexpected">The value that should not be found</param>
		/// <param name="userMessage">A user provided message</param>
		public NotContainsException(object unexpected, string userMessage)
		{
			Unexpected = unexpected;
			UserMessage = userMessage;
		}

		/// <see cref="Exception(SerializationInfo,StreamingContext)"/>
		protected NotContainsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Unexpected = info.GetValue("Unexpected", typeof(object));
		}

		/// <see cref="Exception.GetObjectData"/>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			info.AddValue("Unexpected", Unexpected);
			base.GetObjectData(info, context);
		}
	}
}