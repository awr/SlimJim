using System;
using SlimJim.Model;

namespace SlimJim.Test.Model
{
	public class ProjectPrototypes
	{
		public readonly Proj TheirProject3 = new Proj
			{
				AssemblyName = "TheirProject3",
				Guid = Guid.Parse("{499372E5-5DBF-4DB4-BB1A-9072395C9017}"),
				Path = @"C:\Projects\TheirProject3\TheirProject3.csproj"
			};

		public readonly Proj TheirProject2 = new Proj
			{
				AssemblyName = "TheirProject2",
				Guid = Guid.Parse("{58E0EE99-9DCA-45C4-AB04-48D67316F71D}"),
				Path = @"C:\Projects\TheirProject2\TheirProject2.csproj"
			};

		public readonly Proj TheirProject1 = new Proj
			{
				AssemblyName = "TheirProject1",
				Guid = Guid.Parse("{74CBCCEE-C805-49C3-9EB8-10B48CCC3A6F}"),
				Path = @"C:\Projects\TheirProject1\TheirProject1.csproj"
			};

		public readonly Proj MyProject = new Proj
			{
				AssemblyName = "MyProject",
				Guid = Guid.Parse("{E75347BE-2125-4325-818D-0ECC760F11BA}"),
				Path = @"C:\Projects\MyProject\MyProject.csproj",
				TargetFrameworkVersion = "v3.5"
			};

		public readonly Proj MyMultiFrameworkProject35 = new Proj
			{
				AssemblyName = "MyMultiFrameworkProject",
				Guid = Guid.Parse("{fba80161-8315-4a8b-91a4-bff7d2f0a968}"),
				Path = @"C:\Projects\MyMultiFrameworkProject\MyMultiFrameworkProject-net35.csproj",
				TargetFrameworkVersion = "v3.5"
			};

		public readonly Proj MyMultiFrameworkProject40 = new Proj
			{
				AssemblyName = "MyMultiFrameworkProject",
				Guid = Guid.Parse("{b8da3366-a3d6-4580-9b99-60aed5b01d5e}"),
				Path = @"C:\Projects\MyMultiFrameworkProject\MyMultiFrameworkProject-net40.csproj",
				TargetFrameworkVersion = "v4.0"
			};

		public readonly Proj Unrelated1 = new Proj
			{
				AssemblyName = "Unrelated1",
				Guid = Guid.NewGuid(),
				Path = @"C:\Projects\Unrelated1\Unrelated1.csproj"
			};

		public readonly Proj Unrelated2 = new Proj
			{
				AssemblyName = "Unrelated2",
				Guid = Guid.NewGuid(),
				Path = @"C:\Projects\Unrelated2\Unrelated2.csproj"
			};

		public Proj OurProject1 = new Proj
		{
			AssemblyName = "OurProject1",
			Guid = Guid.Parse("{021CD387-7FE9-4BAD-B57F-6D8ABFE73562}"),
			Path = @"C:\Projects\OurProject1\OurProject1.csproj"
		};

		public Proj OurProject2 = new Proj
		{
			AssemblyName = "OurProject2",
			Guid = Guid.Parse("{4A0CC937-5131-4B9F-AD9E-D6844BDD8EC3}"),
			Path = @"C:\Projects\OurProject2\OurProject2.csproj"
		};
	}
}