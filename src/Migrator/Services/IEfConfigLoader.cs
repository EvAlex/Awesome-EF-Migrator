using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvAlex.AwesomeEfMigrator.Services
{
	public interface IEfConfigLoader
	{
		IEnumerable<AwesomeEfMigratorConfig> LoadConfigs();
	}
}
