using System;
using System.Collections.Generic;
using NUnit.Framework;
using SlimJim.Model;

namespace SlimJim.Test.Model.SlnBuilder
{
	public class SlnBuilderTestFixture : TestBase
	{
		protected string targetProjectName;
		protected Sln solution;
		protected ProjectPrototypes projects;
		protected SlnGenerationOptions options;

		[SetUp]
		public void BeforeEach()
		{
			projects = new ProjectPrototypes();
			targetProjectName = projects.MyProject.AssemblyName;
			options = new SlnGenerationOptions(GetSamplePath("Projects"));
		}

		protected void GeneratePartialGraphSolution(string[] targetProjectNames, params Proj[] projectsList)
		{
			var generator = new SlimJim.Model.SlnBuilder(new List<Proj>(projectsList));
			options.AddTargetProjectNames(targetProjectNames);
			solution = generator.BuildSln(options);
		}
	}
}