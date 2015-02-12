using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public interface IEfConfigLoader
	{
		IEnumerable<AwesomeEfMigratorConfig> LoadConfigs();
	}
}
