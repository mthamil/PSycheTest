using System;

namespace PSycheTest
{
	/// <summary>
	/// Attribute that identifies a test case.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class TestAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new <see cref="TestAttribute"/>.
		/// </summary>
		public TestAttribute()
		{
			Title = string.Empty;
		}

		/// <summary>
		/// A test case's optional title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Indicates that a test should be skipped and what the reason for that is.
		/// </summary>
		public string SkipBecause { get; set; }
	}
}