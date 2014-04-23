using System.Collections.Generic;
using System.IO;
using SlimJim.Model;

namespace SlimJim.Infrastructure
{
    public class CsProjRepository : ProjRepository
	{
		protected override List<FileInfo> FindAllProjectFiles(SlnGenerationOptions options)
		{
			List<FileInfo> files = Finder.FindAllCsProjectFiles(options.ProjectsRootDirectory);

			foreach (string path in options.AdditionalSearchPaths)
			{
				files.AddRange(Finder.FindAllCsProjectFiles(path));
			}

			return files;
		}
	}
}