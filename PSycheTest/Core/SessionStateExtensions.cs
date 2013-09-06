using System.Dynamic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSycheTest.Core
{
	/// <summary>
	/// Provides extension methods for operating on PowerShell session state.
	/// </summary>
	public static class SessionStateExtensions
	{
		/// <summary>
		/// Provides access to session variables.
		/// </summary>
		/// <param name="sessionState">The session state to access</param>
		/// <returns>An object providing access to session variables</returns>
		public static dynamic Variables(this SessionState sessionState)
		{
			return new SessionVariables(sessionState.PSVariable);
		}

		/// <summary>
		/// Provides access to session variables.
		/// </summary>
		/// <param name="sessionState">The session state to access</param>
		/// <returns>An object providing access to session variables</returns>
		public static dynamic Variables(this SessionStateProxy sessionState)
		{
			return new SessionVariables(sessionState.PSVariable);
		}
	}

	/// <summary>
	/// Provides access to PowerShell variables.
	/// </summary>
	public class SessionVariables : DynamicObject
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SessionVariables"/>.
		/// </summary>
		/// <param name="variables">The inner variables object</param>
		public SessionVariables(PSVariableIntrinsics variables)
		{
			_variables = variables;
		}

		/// <see cref="DynamicObject.TryGetMember"/>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			var value = _variables.GetValue(binder.Name);
			result = value;
			return true;
		}

		/// <see cref="DynamicObject.TrySetMember"/>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
 			 _variables.Set(binder.Name, value);
			return true;
		}

		private readonly PSVariableIntrinsics _variables;
	}
}