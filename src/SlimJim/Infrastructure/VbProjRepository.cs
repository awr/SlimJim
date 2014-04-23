using System.Collections.Generic;
using System.IO;
using log4net;
using SlimJim.Model;

namespace SlimJim.Infrastructure
{
    public class VbProjRepository : ProjRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VbProjRepository));

        protected override List<FileInfo> FindAllProjectFiles(SlnGenerationOptions options)
        {
            List<FileInfo> files = Finder.FindAllVbProjectFiles(options.ProjectsRootDirectory);

            foreach (string path in options.AdditionalSearchPaths) {
                files.AddRange(Finder.FindAllVbProjectFiles(path));
            }

            return files;
        }
    }
}