using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace SlimJim.Model
{
	public class SlnBuilder
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(SlnFileGenerator));

		private readonly List<Proj> projectsList;
		private Sln builtSln;
		private static SlnBuilder overriddenBuilder;
		private SlnGenerationOptions options;

		public SlnBuilder(List<Proj> projectsList)
		{
			this.projectsList = projectsList;
		}

		public virtual Sln BuildSln(SlnGenerationOptions options)
		{
			this.options = options;
			builtSln = new Sln(options.SolutionName)
				{
					Version = options.VisualStudioVersion,
					ProjectsRootDirectory = options.ProjectsRootDirectory
				};

			AddProjectsToSln(options);

			return builtSln;
		}

		private void AddProjectsToSln(SlnGenerationOptions options)
		{
			if (options.Mode == SlnGenerationMode.PartialGraph)
			{
				AddPartialProjectGraphToSln(options);
			}
			else
			{
				AddAllProjectsToSln();
			}
		}

		private void AddPartialProjectGraphToSln(SlnGenerationOptions options)
		{
			Log.Info("Building partial graph solution for target projects: " + string.Join(", ", options.TargetProjectNames));

			foreach (string targetProjectName in options.TargetProjectNames)
			{
				Proj rootProject = AddAssemblySubtree(targetProjectName);

				if (rootProject == null)
				{
					Log.WarnFormat("Project {0} not found.", targetProjectName);
				}

				AddAfferentReferencesToProject(rootProject);
			}
		}

		private void AddAllProjectsToSln()
		{
			Log.Info("Building full graph solution.");

			projectsList.ForEach(AddProject);
		}

		private Proj AddAssemblySubtree(string assemblyName, string targetFrameworkVersion = "")
		{
			Proj project = FindProjectByAssemblyName(assemblyName, targetFrameworkVersion);

			AddProjectAndReferences(project);

			return project;
		}

		private Proj FindProjectByAssemblyName(string assemblyName, string targetFrameworkVersion)
		{
			var matches = projectsList.Where(p => p.AssemblyName == assemblyName).ToList();


			if (matches.Count <= 1)
			{
				var single = matches.SingleOrDefault();
				if (single != null) Log.InfoFormat("Found projects with AssemblyName {0}: {1}", assemblyName, single.Path);
				return single;
			}

			//TODO: filter projects that don't specify version
			if (string.IsNullOrEmpty(targetFrameworkVersion))
			{
				Log.WarnFormat("Found multiple projects with AssemblyName {0} and no target framework version is specified: {1}", assemblyName, string.Join(", ", matches.Select(m => m.Path)));
				return matches.First();
			}

			var myVersion = new Version(targetFrameworkVersion.Substring(1));
			var versions = matches
							.Where(m => m.TargetFrameworkVersion != null && m.TargetFrameworkVersion.StartsWith("v"))
							.ToDictionary(m => new Version(m.TargetFrameworkVersion.Substring(1)));

			var closest = versions.Where(v => v.Key <= myVersion).OrderByDescending(v => v.Key).FirstOrDefault();

		    if (closest.Value != null)
		    {
                Log.InfoFormat("Found multiple projects with AssemblyName {0}: {1} and chose {2}", assemblyName, string.Join(", ", matches.Select(m => m.Path)), closest.Value.Path);
                return closest.Value;
		    }

            Log.WarnFormat("Found multiple projects with AssemblyName {0}: {1} and none have compatible TargetFrameworkVersion property. Choosing {2}", assemblyName, string.Join(", ", matches.Select(m => m.Path)), matches.First());
		    return matches.First();
		}

		private void AddProjectAndReferences(Proj project)
		{
			if (project != null)
			{
				AddProject(project);

				IncludeEfferentProjectReferences(project);

				if (options.IncludeEfferentAssemblyReferences)
				{
					IncludeEfferentAssemblyReferences(project);
				}
			}
		}

		private void AddProject(Proj project)
		{
			builtSln.AddProjects(project);
		}

		private void IncludeEfferentProjectReferences(Proj project)
		{
			foreach (var projectGuid in project.ReferencedProjectGuids)
			{
				AddProjectSubtree(projectGuid);
			}
		}

		private void IncludeEfferentAssemblyReferences(Proj project)
		{
			foreach (string assemblyName in project.ReferencedAssemblyNames)
			{
				AddAssemblySubtree(assemblyName, project.TargetFrameworkVersion);
			}
		}

		private void AddProjectSubtree(string projectGuid)
		{
			Proj referencedProject = FindProjectByProjectGuid(projectGuid);

			AddProjectAndReferences(referencedProject);
		}

		private void AddAfferentReferencesToProject(Proj project)
		{
			if (project != null)
			{
				List<Proj> afferentAssemblyReferences = projectsList.FindAll(
					csp => csp.ReferencedAssemblyNames.Contains(project.AssemblyName));

				AddAfferentReferences(afferentAssemblyReferences);

				List<Proj> afferentProjectReferences =
					projectsList.FindAll(csp => csp.ReferencedProjectGuids.Contains(project.Guid));

				AddAfferentReferences(afferentProjectReferences);
			}
		}

		private void AddAfferentReferences(List<Proj> afferentReferences)
		{
			foreach (Proj assemblyReference in afferentReferences)
			{
				AddProjectAndReferences(assemblyReference);
			}
		}

		private Proj FindProjectByProjectGuid(string projectGuid)
		{
			return projectsList.Find(csp => csp.Guid == projectGuid);
		}

		public static SlnBuilder GetSlnBuilder(List<Proj> projects)
		{
			return overriddenBuilder ?? new SlnBuilder(projects);
		}

		public static void OverrideDefaultBuilder(SlnBuilder slnBuilder)
		{
			overriddenBuilder = slnBuilder;
		}
	}
}
