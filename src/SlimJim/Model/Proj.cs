using System;
using System.Collections.Generic;

namespace SlimJim.Model
{
	public class Proj
	{
		public Proj()
		{
			ReferencedAssemblyNames = new List<string>();
            ReferencedProjectGuids = new List<Guid>();
		}

		public Guid Guid { get; set; }
		public string Path { get; set; }
		public string AssemblyName { get; set; }
		public string TargetFrameworkVersion { get; set; }
		public List<string> ReferencedAssemblyNames { get; set; }
		public List<Guid> ReferencedProjectGuids { get; set; }
		public bool UsesMSBuildPackageRestore { get; set; }

		public string ProjectName
		{
			get { return System.IO.Path.GetFileNameWithoutExtension(Path); }
		}

		public void ReferencesAssemblies(params Proj[] assemblyReferences)
		{
			foreach (Proj reference in assemblyReferences)
			{
				if (!ReferencedAssemblyNames.Contains(reference.AssemblyName))
				{
					ReferencedAssemblyNames.Add(reference.AssemblyName);
				}
			}
		}

		public void ReferencesProjects(params Proj[] projectReferences)
		{
			foreach (Proj reference in projectReferences)
			{
				if (!ReferencedProjectGuids.Contains(reference.Guid))
				{
					ReferencedProjectGuids.Add(reference.Guid);
				}
			}
		}

		public override string ToString()
		{
			return base.ToString() + 
				string.Format(@" {{AssemblyName=""{0}"", Guid=""{1:B}"", Path=""{2}""}}", AssemblyName, Guid, Path);
		}
	}
}
