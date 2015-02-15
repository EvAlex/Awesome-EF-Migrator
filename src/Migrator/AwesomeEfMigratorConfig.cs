using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvAlex.AwesomeEfMigrator
{
	public abstract class AwesomeEfMigratorConfig
	{
		internal void CallConfigure(AwesomeEfMigratorConfigBuilder configBuilder)
		{
			Configure(configBuilder);
        }

		protected abstract void Configure(AwesomeEfMigratorConfigBuilder configBuilder);
	}
}
