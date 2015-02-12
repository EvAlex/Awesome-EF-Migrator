using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceSoft.Aquas.Model.Initializer
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
