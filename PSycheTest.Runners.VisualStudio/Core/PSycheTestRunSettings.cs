using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Represents test run settings.
	/// </summary>
	[XmlRoot(SettingsProviderName)]
	public class PSycheTestRunSettings : TestRunSettings, IXmlSerializable, IPSycheTestRunSettings
	{
		/// <summary>
		/// Initializes a new <see cref="PSycheTestRunSettings"/> instance.
		/// </summary>
		public PSycheTestRunSettings()
			: base(SettingsProviderName)
		{
		}

		/// <summary>
		/// Any modules to initially import specified in the settings file.
		/// </summary>
		[XmlIgnore]
		public ICollection<string> Modules { get { return _modules; } }

		/// <see cref="TestRunSettings.ToXml"/>
		public override XmlElement ToXml()
		{
			var stringWriter = new StringWriter();
			_serializer.Serialize(stringWriter, this);
			var xml = stringWriter.ToString();
			var document = new XmlDocument();
			document.LoadXml(xml);
			return document.DocumentElement;
		}

		/// <see cref="IXmlSerializable.GetSchema"/>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <see cref="IXmlSerializable.ReadXml"/>
		public void ReadXml(XmlReader reader)
		{
			var xml = XDocument.Load(reader);
			var psycheTestSettings = xml.Descendants(XName.Get(SettingsProviderName)).FirstOrDefault();
			if (psycheTestSettings != null)
			{
				var modulesElement = psycheTestSettings.Element(XName.Get("Modules"));
				if (modulesElement != null)
				{
					var moduleElements = modulesElement.Elements(XName.Get("Module"));
					foreach (var moduleElement in moduleElements)
						_modules.Add(moduleElement.Value);
				}
			}
		}

		/// <see cref="IXmlSerializable.WriteXml"/>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Modules");
			foreach (var module in _modules)
				writer.WriteElementString("Module", module);
			writer.WriteEndElement();
		}

		/// <summary>
		/// The name of the settings provider.
		/// </summary>
		public const string SettingsProviderName = "PSycheTest";

		private readonly IList<string> _modules = new List<string>(); 
		private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(PSycheTestRunSettings));
	}
}