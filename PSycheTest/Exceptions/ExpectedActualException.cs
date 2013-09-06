using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PSycheTest.Exceptions
{
	/// <summary>
	/// Assertion exception for when a certain value was expected and the actual value did
	/// not match.
	/// </summary>
	[Serializable]
	public class ExpectedActualException : AssertionException
	{
		/// <summary>
		/// The expected value.
		/// </summary>
		public object Expected { get; private set; }

		/// <summary>
		/// The actual value.
		/// </summary>
		public object Actual { get; private set; }

		/// <see cref="Exception.Message"/>
		public override string Message
		{
			get
			{
				return String.Format("{0}{3}Expected: {1}{3}Actual: {2}", 
				                     base.Message,
				                     Expected ?? "(null)", 
				                     Actual ?? "(null)", 
				                     Environment.NewLine);
			}
		}

		/// <summary>
		/// Initializes a new <see cref="ExpectedActualException"/>.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="userMessage">A user provided message</param>
		public ExpectedActualException(object expected, object actual, string userMessage)
			: base(userMessage)
		{
			Expected = expected;
			Actual = actual;
		}

		/// <see cref="Exception(SerializationInfo,StreamingContext)"/>
		protected ExpectedActualException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Expected = info.GetValue("Expected", typeof(object));
			Actual = info.GetValue("Actual", typeof(object));
		}

		/// <see cref="Exception.GetObjectData"/>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			info.AddValue("Expected", Expected);
			info.AddValue("Actual", Actual);
			base.GetObjectData(info, context);
		}
	}
}