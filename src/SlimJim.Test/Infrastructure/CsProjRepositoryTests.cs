using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using SlimJim.Infrastructure;
using SlimJim.Model;
using SlimJim.Test.SampleFiles;

namespace SlimJim.Test.Infrastructure
{
	[TestFixture]
	public class CsProjRepositoryTests : TestBase
	{
		private string SearchPath1
		{
			get
			{
				return GetSamplePath("OtherProjects");
			}
		}

		private string SearchPath2
		{
			get
			{
				return GetSamplePath("MoreProjects");
			}
		}

		private readonly FileInfo file1 = SampleFileHelper.GetCsProjFile("Simple");
		private readonly FileInfo file2 = SampleFileHelper.GetCsProjFile("Simple");
		private readonly Proj proj1 = new Proj {AssemblyName = "Proj1"};
		private readonly Proj proj2 = new Proj {AssemblyName = "Proj1"};
		private ProjectFileFinder finder;
		private ProjReader reader;
		private CsProjRepository repository;
		private SlnGenerationOptions options;

		[SetUp]
		public void BeforeEach()
		{
			options = new SlnGenerationOptions(WorkingDirectory);
			finder = MockRepository.GenerateStrictMock<ProjectFileFinder>();
			reader = MockRepository.GenerateStrictMock<ProjReader>();
			repository = new CsProjRepository
			{
				Finder = finder,
				Reader = reader
			};
		}

		[TearDown]
		public void AfterEach()
		{
			finder.VerifyAllExpectations();
			reader.VerifyAllExpectations();
		}

		[Test]
		public void CreatesOwnInstancesOfFinderAndReader()
		{
			repository = new CsProjRepository();
			Assert.That(repository.Finder, Is.Not.Null, "Should have created instance of CsProjFinder.");
			Assert.That(repository.Reader, Is.Not.Null, "Should have created instance of ProjReader.");
		}

		[Test]
		public void GetsFilesFromFinderAndProcessesThemWithCsProjReader()
		{
			finder.Expect(f => f.FindAllCsProjectFiles(WorkingDirectory)).Return(new List<FileInfo>{file1, file2});
			reader.Expect(r => r.Read(file1)).Return(proj1);
			reader.Expect(r => r.Read(file2)).Return(proj2);

			List<Proj> projects = repository.LookupProjsFromDirectory(options);

			Assert.That(projects, Is.EqualTo(new[]{proj1, proj2}));
		}

		[Test]
		public void GracefullyHandlesNullsFromReader()
		{
			finder.Expect(f => f.FindAllCsProjectFiles(WorkingDirectory)).Return(new List<FileInfo> { file1, file2 });
			reader.Expect(r => r.Read(file1)).Return(proj1);
			reader.Expect(r => r.Read(file2)).Return(null);

			List<Proj> projects = repository.LookupProjsFromDirectory(options);

			Assert.That(projects, Is.EqualTo(new[] { proj1 }));
		}

		[Test]
		public void ReadsFilesFromAdditionalSearchPathsAsWell()
		{
			options.AddAdditionalSearchPaths(new[] { SearchPath1, SearchPath2 });
			finder.Expect(f => f.FindAllCsProjectFiles(WorkingDirectory)).Return(new List<FileInfo>());
			finder.Expect(f => f.FindAllCsProjectFiles(SearchPath1)).Return(new List<FileInfo>());
			finder.Expect(f => f.FindAllCsProjectFiles(SearchPath2)).Return(new List<FileInfo>());

			repository.LookupProjsFromDirectory(options);
		}

		[Test]
		public void IngoresDirectoryPatternsInOptions()
		{
			options.AddIgnoreDirectoryPatterns("Folder1", "Folder2");
			finder.Expect(f => f.IgnorePatterns(new[] {"Folder1", "Folder2"}));
			finder.Expect(f => f.FindAllCsProjectFiles(WorkingDirectory)).Return(new List<FileInfo>());

			repository.LookupProjsFromDirectory(options);
		}
	}
}