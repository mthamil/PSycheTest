using System;
using System.Collections.Generic;
using System.Linq;

namespace PSycheTest.Runners.Framework.Utilities.Text
{
	/// <summary>
	/// Contains general purpose <see cref="string"/> extension methods.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Splits a string into trimmed lines.
		/// </summary>
		/// <param name="string">The string to split</param>
		/// <returns>An enumerable over the trimmed lines in a string</returns>
		public static IEnumerable<string> Lines(this string @string)
		{
			if (String.IsNullOrEmpty(@string))
				return Enumerable.Empty<string>();

			return @string.Split('\n').Select(line => line.Trim());
		}
	}
}