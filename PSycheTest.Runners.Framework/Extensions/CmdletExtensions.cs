using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace PSycheTest.Runners.Framework.Extensions
{
	/// <summary>
	/// Class that helps build collections of cmdlets.
	/// </summary>
	public static class CmdletExtensions
	{
		/// <summary>
		/// Retrieves a collection of cmdlets defined in an assembly.
		/// </summary>
		/// <param name="assembly">An assembly containing the cmdlet classes</param>
		/// <returns>A new list of cmdlet definitions</returns>
		public static IEnumerable<SessionStateCmdletEntry> GetCmdlets(this Assembly assembly)
		{
			var cmdlets = from type in assembly.GetTypes()
						  where type.IsClass
						  from cmdletAttribute in type.GetCustomAttributes<CmdletAttribute>()
						  select CreateCmdletConfig(type);

			return cmdlets.ToList();
		}

		/// <summary>
		/// Creates a <see cref="CmdletConfigurationEntry"/> from a <see cref="PSCmdlet"/>.
		/// </summary>
		/// <param name="cmdletType">The type of cmdlet to use</param>
		/// <returns>A new <see cref="CmdletConfigurationEntry"/> for the given cmdlet</returns>
		public static SessionStateCmdletEntry FromCmdletType(this Type cmdletType)
		{
			return CreateCmdletConfig(cmdletType);
		}

		/// <summary>
		/// Gets the name of a cmdlet from its implementing type.
		/// </summary>
		/// <param name="cmdletType">The type of cmdlet</param>
		/// <returns>The name of the cmdlet</returns>
		public static string CmdletName(this Type cmdletType)
		{
			var cmdletAttribute = cmdletType.GetCustomAttribute<CmdletAttribute>();
			if (cmdletAttribute == null)
				throw new ArgumentException(cmdletType.Name + " does not have a " + typeof(CmdletAttribute).Name, "cmdletType");

			return cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName;
		}

		private static SessionStateCmdletEntry CreateCmdletConfig(Type cmdletType)
		{
			return new SessionStateCmdletEntry(cmdletType.CmdletName(), cmdletType, null);
		}
	}
}