using System.Runtime.Remoting.Channels;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Contains methods to help with channel cleanup.
	/// </summary>
	internal static class Channels
	{
		/// <summary>
		/// Unregisters any currently registered remoting channels.
		/// </summary>
		/// <remarks>
		/// After looking at other Visual Studio test runners, apparently remoting channels are opened but 
		/// not cleaned up by Visual Studio and so this is necessary.
		/// </remarks>
		internal static void UnregisterAll()
		{
			foreach (IChannel chan in ChannelServices.RegisteredChannels)
				ChannelServices.UnregisterChannel(chan);
		}
	}
}