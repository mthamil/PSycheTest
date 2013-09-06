using System.ComponentModel.Composition;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using PSycheTest.Runners.VisualStudio.Core;

namespace PSycheTest.Runners.VisualStudio
{
	/// <summary>
	/// Provides serialization and deserialization of PSycheTest Visual Studio settings.
	/// </summary>
	[Export(typeof(ISettingsProvider))]
	[Export(typeof(IRunSettingsService))]
	[Export(typeof(IPSycheTestSettingsService))]
	[SettingsName(PSycheTestRunSettings.SettingsProviderName)]
	public class PSycheTestSettingsService : IRunSettingsService, ISettingsProvider, IPSycheTestSettingsService
	{
		/// <summary>
		/// Initializes a new <see cref="PSycheTestSettingsService"/>.
		/// </summary>
		public PSycheTestSettingsService()
        {
			Name = PSycheTestRunSettings.SettingsProviderName;
            Settings = new PSycheTestRunSettings();
			_serializer = new XmlSerializer(typeof(PSycheTestRunSettings));
        }

		/// <see cref="IPSycheTestSettingsService.Settings"/>
		public IPSycheTestRunSettings Settings { get; private set; }

		/// <see cref="IRunSettingsService.AddRunSettings"/>
		public IXPathNavigable AddRunSettings(IXPathNavigable inputRunSettingDocument, IRunSettingsConfigurationInfo configurationInfo, ILogger log)
		{
			ValidateArg.NotNull(inputRunSettingDocument, "inputRunSettingDocument");
			ValidateArg.NotNull(configurationInfo, "configurationInfo");

			var navigator = inputRunSettingDocument.CreateNavigator();
			navigator.MoveToRoot();
			return navigator;
		}

		/// <see cref="IRunSettingsService.Name"/>
		public string Name { get; private set; }

		/// <see cref="ISettingsProvider.Load"/>
		public void Load(XmlReader reader)
		{
			ValidateArg.NotNull(reader, "reader");

			if (reader.Read() && reader.Name.Equals(PSycheTestRunSettings.SettingsProviderName))
				Settings = _serializer.Deserialize(reader) as PSycheTestRunSettings;
		}

		private readonly XmlSerializer _serializer;
	}
}