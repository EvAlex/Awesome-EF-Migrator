using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceSoft.Aquas.Model.Initializer
{
	internal sealed class ZeroConfig : AwesomeEfMigratorConfig
	{
		private readonly Type dbContextType;

        public ZeroConfig(Type dbContextType)
		{
			this.dbContextType = dbContextType;
        }

		protected override void Configure(AwesomeEfMigratorConfigBuilder configBuilder)
		{
			configBuilder.UseDbContext(dbContextType);
        }
	}
}
