using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using PSycheTest.Runners.VisualStudio.Core;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.VisualStudio.Core
{
	public class PSycheTestRunSettingsTests
	{
		[Fact]
		public void Test_ReadXml()
		{
			// Arrange.
			var xmlReader = XmlReader.Create(new StringReader(
				@"<?xml version='1.0' encoding='utf-8' ?>
				  <RunSettings>
				    <PSycheTest>
					  <Modules>
						  <Module>TestModule1.psd1</Module>
						  <Module>TestModule2.dll</Module>
					  </Modules>
				    </PSycheTest>
				  </RunSettings>"));

			// Act.
			settings.ReadXml(xmlReader);

			// Assert.
			Assert.Equal(new[] { "TestModule1.psd1", "TestModule2.dll" }, settings.Modules.ToArray());
		}

		[Fact]
		public void Test_ReadXml_With_No_PSycheTest_Element()
		{
			// Arrange.
			var xmlReader = XmlReader.Create(new StringReader(
				@"<?xml version='1.0' encoding='utf-8' ?>
				  <RunSettings>
				  </RunSettings>"));

			// Act.
			settings.ReadXml(xmlReader);

			// Assert.
			Assert.Empty(settings.Modules);
		}

		[Fact]
		public void Test_ReadXml_With_No_Modules()
		{
			// Arrange.
			var xmlReader = XmlReader.Create(new StringReader(
				@"<?xml version='1.0' encoding='utf-8' ?>
				  <RunSettings>
				  <PSycheTest>
				  </PSycheTest>
				  </RunSettings>"));

			// Act.
			settings.ReadXml(xmlReader);

			// Assert.
			Assert.Empty(settings.Modules);
		}

		[Fact]
		public void Test_WriteXml()
		{
			// Arrange.
			var sink = new StringBuilder();
			using (var xmlWriter = XmlWriter.Create(sink, writerSettings))
			{
				settings.Modules.Add("TestModule1");
				settings.Modules.Add(@".\TestModule2.psd1");

				// Act.
				settings.WriteXml(xmlWriter);
			}

			// Assert.
			Assert.Equal(
				@"<Modules><Module>TestModule1</Module><Module>.\TestModule2.psd1</Module></Modules>", 
				sink.ToString());
		}

		[Fact]
		public void Test_WriteXml_With_No_Modules()
		{
			// Arrange.
			var sink = new StringBuilder();
			using (var xmlWriter = XmlWriter.Create(sink, writerSettings))
			{
				// Act.
				settings.WriteXml(xmlWriter);
			}

			// Assert.
			Assert.Equal(@"<Modules />", sink.ToString());
		}

		private readonly PSycheTestRunSettings settings = new PSycheTestRunSettings();

		private static readonly XmlWriterSettings writerSettings = new XmlWriterSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment,
			Encoding = Encoding.UTF8,
			OmitXmlDeclaration = true
		};
	}
}