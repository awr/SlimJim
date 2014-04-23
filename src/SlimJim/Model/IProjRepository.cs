using System.Collections.Generic;

namespace SlimJim.Model
{
	public interface IProjRepository
	{
		List<Proj> LookupProjsFromDirectory(SlnGenerationOptions options);
	}
}