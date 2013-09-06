using System;
using System.Collections.Generic;

namespace PSycheTest.Runners.VisualStudio.Core.Utilities
{
	/// <summary>
	/// An <see cref="IEqualityComparer{T}"/> that compares strings in a non-case-sensitive way.
	/// </summary>
	public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
	{
		/// <see cref="IEqualityComparer{T}.Equals(T,T)"/>
		public bool Equals(string x, string y)
		{
			return String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Returns a file's hashcode.
		/// </summary>
		public int GetHashCode(string obj)
		{
			return obj.ToLowerInvariant().GetHashCode();
		}

		/// <summary>
		/// Gets a <see cref="CaseInsensitiveEqualityComparer"/>.
		/// </summary>
		public static IEqualityComparer<string> Instance
		{
			get { return instance; }
		}

		private static readonly IEqualityComparer<string> instance = new CaseInsensitiveEqualityComparer(); 
	}
}