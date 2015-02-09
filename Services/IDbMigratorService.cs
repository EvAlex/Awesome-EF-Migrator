using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceSoft.Aquas.Model.Initializer.Models;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public interface IDbMigratorService
	{
		DbMigrationsConfiguration GetMigrationsConfiguration(Type dbContextType);

		ICollection<Migration> GetMigrations(DbMigrationsConfiguration dbMigrationsConfig, Database database);

		bool DatabaseHasMigrationsFor(Database database, DbMigrationsConfiguration dbMigrationsConfig);

		void UpdateDatabase(Database database, DbMigrationsConfiguration dbMigrationsConfig);
	}
}
