using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;

namespace SlimJim.Infrastructure
{
	public class ProjectFileFinder
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly List<Regex> ignorePatterns;

		public ProjectFileFinder()
		{
			ignorePatterns = new List<Regex>();
			IgnorePatterns(@"^\.svn$", @"^\.hg$", @"^\.git$", "^bin$", "^obj$", "ReSharper");
		}

		public virtual List<FileInfo> FindAllCsProjectFiles(string startPath)
		{
			Log.InfoFormat("Searching for .csproj files at {0}", startPath);

			var root = new DirectoryInfo(startPath);
			var projectFiles = GetProjectFiles(root, "*.csproj");

			return projectFiles;
		}

        public virtual List<FileInfo> FindAllVbProjectFiles(string startPath)
        {
            Log.InfoFormat("Searching for .vbproj files at {0}", startPath);

            var root = new DirectoryInfo(startPath);
            var projectFiles = GetProjectFiles(root, "*.vbproj");

            return projectFiles;
        }

        private List<FileInfo> GetProjectFiles(DirectoryInfo directory, string searchPattern)
		{
			var files = new List<FileInfo>();

			if (!PathIsIgnored(directory.Name))
			{
				SearchDirectoryForProjects(directory, searchPattern, files);
			}

			return files;
		}

		private void SearchDirectoryForProjects(DirectoryInfo directory, string searchPattern, List<FileInfo> files)
		{
			FileInfo[] projects = directory
                                    .GetFiles(searchPattern)
									.Where(f => !PathIsIgnored(f.Name))
									.ToArray();

			if (projects.Length > 0)
			{
				AddProjectFile(projects, files);
			}
			else
			{
				RecurseChildDirectories(directory, searchPattern, files);
			}
		}

        private void RecurseChildDirectories(DirectoryInfo directory, string searchPattern, List<FileInfo> files)
		{
			foreach (DirectoryInfo dir in directory.EnumerateDirectories())
			{
				files.AddRange(GetProjectFiles(dir, searchPattern));
			}
		}

		private void AddProjectFile(IEnumerable<FileInfo> projects, List<FileInfo> files)
		{
			foreach (var project in projects)
			{
				files.Add(project);
				Log.Debug(project);
			}
		}

		public bool PathIsIgnored(DirectoryInfo dir)
		{
			return PathIsIgnored(dir.Name);
		}

		public bool PathIsIgnored(string name)
		{
			return ignorePatterns.Exists(p => p.IsMatch(name));
		}

		public virtual void IgnorePatterns(params string[] patterns)
		{
			var regexes = new List<string>(patterns).ConvertAll(p => new Regex(p, RegexOptions.IgnoreCase));
			ignorePatterns.AddRange(regexes);
		}
	}
}
