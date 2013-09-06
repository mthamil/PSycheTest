using System;

namespace PSycheTest
{
	/// <summary>
	/// Indicates that a function should be run before each test in a script.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class TestSetupAttribute : Attribute { }
}