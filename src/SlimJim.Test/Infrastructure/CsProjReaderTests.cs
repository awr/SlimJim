using System.IO;
using NUnit.Framework;
using SlimJim.Infrastructure;
using SlimJim.Model;

namespace SlimJim.Test.Infrastructure
{
	[TestFixture]
	public class CsProjReaderTests
	{
		private FileInfo file;

		[Test]
		public void ReadsFileContentsIntoObject()
		{
			Proj project = GetProject("Simple");

			Assert.That(project.Guid, Is.EqualTo("{4A37C916-5AA3-4C12-B7A8-E5F878A5CDBA}"));
			Assert.That(project.AssemblyName, Is.EqualTo("MyProject"));
			Assert.That(project.Path, Is.EqualTo(file.FullName));
			Assert.That(project.TargetFrameworkVersion, Is.EqualTo("v4.0"));
			Assert.That(project.ReferencedAssemblyNames, Is.EqualTo(new[]
																   	{
																							"System",
																							"System.Core",
																							"System.Xml.Linq",
																							"System.Data.DataSetExtensions",
																							"Microsoft.CSharp",
																							"System.Data",
																							"System.Xml"
																   	}));
			Assert.That(project.ReferencedProjectGuids, Is.EqualTo(new[]
																  	{
																							"{99036BB6-4F97-4FCC-AF6C-0345A5089099}",
																							"{69036BB3-4F97-4F9C-AF2C-0349A5049060}"
																  	}));
		}

		[Test]
		public void IgnoresNestedReferences()
		{
			Proj project = GetProject("ConvertedReference");

			Assert.That(project.ReferencedAssemblyNames, Is.Not.Contains("log4net"));
		}

		[Test]
		public void TakesOnlyNameOfFullyQualifiedAssemblyName()
		{
			Proj project = GetProject("FQAssemblyName");

			Assert.That(project.ReferencedAssemblyNames, Contains.Item("NHibernate"));
		}

		[Test]
		public void NoProjectReferencesDoesNotCauseNRE()
		{
			Proj project = GetProject("NoProjectReferences");

			Assert.That(project.ReferencedProjectGuids, Is.Empty);
		}

		[Test]
		public void NoAssemblyName_ReturnsNull()
		{
			Proj project = GetProject("BreaksThings");

			Assert.That(project, Is.Null);
		}

		private Proj GetProject(string fileName)
		{
			file = SampleFiles.SampleFileHelper.GetCsProjFile(fileName);
			var reader = new ProjReader();
			return reader.Read(file);
		}
	}
}