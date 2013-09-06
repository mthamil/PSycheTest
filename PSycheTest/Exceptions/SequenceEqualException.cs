using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace PSycheTest.Exceptions
{
	/// <summary>
	/// Exceptionsthrown when two sequences are not equivalent.
	/// </summary>
	[Serializable]
	public class SequenceEqualException : AssertionException
	{
		/// <summary>
		/// Creates a new instance of the <see cref="SequenceEqualException"/> class.
		/// </summary>
		/// <param name="expected">The expected sequence</param>
		/// <param name="expectedFullyDrained">Whether the entirety of the expected sequence was evaluated</param>
		/// <param name="actual">The actual sequence</param>
		/// <param name="actualFullyDrained">Whether the entirety of the actual sequence was evaluated</param>
		public SequenceEqualException(IEnumerable expected, bool expectedFullyDrained, IEnumerable actual, bool actualFullyDrained)
			: base(typeof(SequenceEqualException).Name + " : SequenceEqual Assertion Failure")
		{
			_expected = expected;
			_actual = actual;
			_expectedFullyDrained = expectedFullyDrained;
			_actualFullyDrained = actualFullyDrained;

			Actual = String.Join(",", _actual.Cast<object>());
			Expected = String.Join(",", _expected.Cast<object>());
		}

		/// <summary>
		/// Gets the actual sequence.
		/// </summary>
		public string Actual { get; private set; }

		/// <summary>
		/// Gets the expected sequence.
		/// </summary>
		public string Expected { get; private set; }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
		public override string Message
		{
			get
			{
				return String.Format("{0}{1}Expected: {2}{3}{1}Actual: {4}{5}",
				                     base.Message,
				                     Environment.NewLine,
				                     Expected == string.Empty ? EMPTY_COLLECTION_MESSAGE : Expected,
				                     _expectedFullyDrained ? string.Empty : ",...",
				                     Actual == string.Empty ? EMPTY_COLLECTION_MESSAGE : Actual,
				                     _actualFullyDrained ? string.Empty : ",...");
			}
		}

		/// <see cref="System.Exception(SerializationInfo, StreamingContext)"/>
		protected SequenceEqualException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{

		}

		/// <see cref="ISerializable.GetObjectData"/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			base.GetObjectData(info, context);
		}

		private readonly IEnumerable _expected;
		private readonly IEnumerable _actual;
		private readonly bool _expectedFullyDrained;
		private readonly bool _actualFullyDrained;

		private const string EMPTY_COLLECTION_MESSAGE = "Empty Sequence";
	}
}