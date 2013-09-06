using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PSycheTest.Exceptions
{
	/// <summary>
	/// Assertion exception for when a certain value was expected to be in a collection
	/// and was not found.
	/// </summary>
	[Serializable]
	public class ContainsException : AssertionException
	{
		/// <summary>
		/// The expected value.
		/// </summary>
		public object Expected { get; private set; }

		/// <see cref="Exception.Message"/>
		public override string Message
		{
			get
			{
				return String.Format("{0}{2}Not found: {1}", 
				                     base.Message,
				                     Expected ?? "(null)", 
				                     Environment.NewLine);
			}
		}

		/// <summary>
		/// Initializes a new <see cref="ContainsException"/>.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="userMessage">A user provided message</param>
		public ContainsException(object expected, string userMessage)
		{
			Expected = expected;
			UserMessage = userMessage;
		}

		/// <see cref="Exception(SerializationInfo,StreamingContext)"/>
		protected ContainsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Expected = info.GetValue("Expected", typeof(object));
		}

		/// <see cref="Exception.GetObjectData"/>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			info.AddValue("Expected", Expected);
			base.GetObjectData(info, context);
		}
	}
}