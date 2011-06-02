using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using SlimJim.Infrastructure;
using SlimJim.Model;

namespace SlimJim
{
	public class SlnFileGenerator
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(SlnFileGenerator));
		public CsProjRepository ProjectRepository { get; set; }
		public SlnFileWriter SlnWriter { get; set; }

		public SlnFileGenerator()
		{
			ProjectRepository = new CsProjRepository();
			SlnWriter = new SlnFileWriter();
		}

		public void GenerateSolutionFile(SlnGenerationOptions options)
		{
			Log.InfoFormat("Generating solution file. Targets: {0}; Destination: {1}", 
					string.Join(", ", options.TargetProjectNames),
					Path.Combine(options.SlnOutputPath, options.SolutionName));

			List<CsProj> projects = ProjectRepository.LookupCsProjsFromDirectory(options);
			Sln solution = SlnBuilder.GetSlnBuilder(projects).BuildSln(options);
			SlnWriter.WriteSlnFile(solution, options.SlnOutputPath);
		}
	}
}