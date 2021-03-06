﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SlimJim.Model;

namespace SlimJim.Infrastructure
{
    public class ProjReader
	{
		private static readonly XNamespace Ns = "http://schemas.microsoft.com/developer/msbuild/2003";

		public virtual Proj Read(FileInfo csProjFile)
		{
			var xml = LoadXml(csProjFile);
			var properties = (from p in xml.Elements(Ns + "PropertyGroup")
                                  where !p.HasAttributes
                                  select p).FirstOrDefault();

			if (properties == null) return null;

            var assemblyNameElement = properties.Element(Ns + "AssemblyName");

		    var assemblyName = (assemblyNameElement == null
		        ? csProjFile.Name
		        : assemblyNameElement.Value);

			return new Proj
			{
				Path = csProjFile.FullName,
				AssemblyName = assemblyName,
				Guid = Guid.Parse(properties.Element(Ns + "ProjectGuid").ValueOrDefault()).ToString("B"),
				TargetFrameworkVersion = properties.Element(Ns + "TargetFrameworkVersion").ValueOrDefault(),
				ReferencedAssemblyNames = ReadReferencedAssemblyNames(xml),
				ReferencedProjectGuids = ReadReferencedProjectGuids(xml),
				UsesMSBuildPackageRestore = FindImportedNuGetTargets(xml)
			};
		}

		private XElement LoadXml(FileInfo csProjFile)
		{
			XElement xml;
			using (var reader = csProjFile.OpenText())
			{
				xml = XElement.Load(reader);
			}
			return xml;
		}

		private List<string> ReadReferencedAssemblyNames(XElement xml)
		{
			var rawAssemblyNames = (from r in xml.DescendantsAndSelf(Ns + "Reference")
											 where r.Parent.Name == Ns + "ItemGroup"
											 select r.Attribute("Include").Value).ToList();
			var unQualifiedAssemblyNames = rawAssemblyNames.ConvertAll(UnQualify);
			return unQualifiedAssemblyNames;
		}

		private string UnQualify(string name)
		{
			if (!name.Contains(",")) return name;

			return name.Substring(0, name.IndexOf(","));
		}

		private List<string> ReadReferencedProjectGuids(XElement xml)
		{
			return (
                from pr in xml.DescendantsAndSelf(Ns + "ProjectReference")
					  select Guid.Parse(pr.Element(Ns + "Project").Value).ToString("B")).ToList();
		}

		private bool FindImportedNuGetTargets(XElement xml)
		{
			var importPaths = (
                from import in xml.DescendantsAndSelf(Ns + "Import")
                where import.Attribute("Project") != null
				select import.Attribute("Project").Value);
			return importPaths.Any(p => p.EndsWith(@"\.nuget\nuget.targets", StringComparison.InvariantCultureIgnoreCase));
		}
	}
}