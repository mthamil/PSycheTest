using System;
using System.Linq.Expressions;
using System.Management.Automation;
using PSycheTest.Core;
using PSycheTest.Core.Extensions;
using PSycheTest.Core.Utilities.Reflection;
using Tests.Support;
using Xunit;

namespace Tests.Integration.PSycheTest
{
	/// <summary>
	/// Base for classes which test cmdlets.
	/// </summary>
	public abstract class CmdletTestBase<TCmdlet> : IUseFixture<PowerShellFixture> where TCmdlet : PSCmdlet
	{
		public void SetFixture(PowerShellFixture data)
		{
			Current = data;
			Current.Shell.Commands.Clear();
			Current.AddCmdlets(typeof(TCmdlet));
		}

		protected string CmdletName
		{
			get { return typeof(TCmdlet).CmdletName(); }
		}

		protected string NameOf<TValue>(Expression<Func<TCmdlet, TValue>> propertyAccessor)
		{
			return Reflect.PropertyOf(propertyAccessor).Name;
		}

		protected PowerShell AddCmdlet()
		{
			return Current.Shell.AddCommand(CmdletName);
		}

		protected PowerShell AddParameter<TValue>(Expression<Func<TCmdlet, TValue>> propertyAccessor, TValue value)
		{
			return Current.Shell.AddParameter(NameOf(propertyAccessor), value);
		}

		protected PowerShellFixture Current { get; private set; }
	}
}