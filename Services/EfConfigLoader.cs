using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public class EfConfigLoader : IEfConfigLoader
	{
		private const string MigrationAssembliesDir = "MigrationAssemblies";

		//private readonly string[] targetAssembliesPaths = new string[]
		//	{
		//		Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Data.Storage\bin\debug\PoliceSoft.Aquas.Data.Storage.dll"),
		//	};

		public IEnumerable<AwesomeEfMigratorConfig> LoadConfigs()
		{
			string dir = Path.Combine(Directory.GetCurrentDirectory(), MigrationAssembliesDir);
			var targetAssembliesPaths = Directory.GetFiles(dir, "*.dll").Union(Directory.GetFiles(dir, "*.exe"));
            var assemblies = targetAssembliesPaths.Select(path => Assembly.LoadFile(path));
			foreach (var a in assemblies)
			{
				var types = a.GetTypes();
				var configTypes = types.Where(t => t.BaseType == typeof(AwesomeEfMigratorConfig));
				if (configTypes.Any())
				{
					var configs = configTypes.Select(t => Activator.CreateInstance(t)).OfType<AwesomeEfMigratorConfig>();
					foreach (var c in configs)
					{
						yield return c;
					}
				}
				else
				{
					var dbContextTypes = types.Where(t => t.BaseType == typeof(DbContext));
					foreach (var t in dbContextTypes)
					{
						yield return new ZeroConfig(t);
					}
				}
			}
			//return targetAssembliesPaths
			//	.Select(path => Assembly.LoadFile(path))
			//	.SelectMany(a => a.GetTypes())
			//	.Where(t => t.BaseType == typeof(AwesomeEfMigratorConfig))
			//	.Select(t => Activator.CreateInstance(t))
			//	.OfType<AwesomeEfMigratorConfig>();
		}
	}
}
