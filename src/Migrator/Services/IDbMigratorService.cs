using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvAlex.AwesomeEfMigrator.Models;

namespace EvAlex.AwesomeEfMigrator.Services
{
	public interface IDbMigratorService
	{
		DbMigrationsConfiguration GetMigrationsConfiguration(Type dbContextType);

		ICollection<Migration> GetMigrations(DbMigrationsConfiguration dbMigrationsConfig, Database database);

		bool DatabaseHasMigrationsFor(Database database, DbMigrationsConfiguration dbMigrationsConfig);

		void UpdateDatabase(Database database, DbMigrationsConfiguration dbMigrationsConfig);

		void UpdateDatabase(Database database, DbMigrationsConfiguration dbMigrationsConfiguration, Migration targetMigration);

		void RollbackAllMigrations(Database database, DbMigrationsConfiguration dbMigrationsConfig);
	}
}
