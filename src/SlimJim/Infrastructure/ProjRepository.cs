using System.Collections.Generic;
using System.IO;
using log4net;
using SlimJim.Model;

namespace SlimJim.Infrastructure
{
    public abstract class ProjRepository : IProjRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProjRepository));

        protected ProjRepository()
        {
            Finder = new ProjectFileFinder();
            Reader = new ProjReader();
        }

        public virtual List<Proj> LookupProjsFromDirectory(SlnGenerationOptions options)
        {
            IgnoreConfiguredDirectoryPatterns(options);

            List<FileInfo> files = FindAllProjectFiles(options);
            List<Proj> projects = ReadProjectFilesIntoProjObjects(files);

            return projects;
        }

        private void IgnoreConfiguredDirectoryPatterns(SlnGenerationOptions options)
        {
            if (options.IgnoreDirectoryPatterns.Count > 0) {
                Finder.IgnorePatterns(options.IgnoreDirectoryPatterns.ToArray());
            }
        }

        protected abstract List<FileInfo> FindAllProjectFiles(SlnGenerationOptions options);

        private List<Proj> ReadProjectFilesIntoProjObjects(List<FileInfo> files)
        {
            List<Proj> projects = files.ConvertAll(f => Reader.Read(f));
            projects.RemoveAll(p => p == null);
            return projects;
        }

        public ProjectFileFinder Finder { get; set; }
        public ProjReader Reader { get; set; }
    }
}