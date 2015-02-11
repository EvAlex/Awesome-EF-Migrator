using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceSoft.Aquas.Model.Initializer.Models;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public class DbMigratorService : IDbMigratorService
	{
		private Dictionary<Type, DbMigrationsConfiguration> dbMigrationsConfigs = new Dictionary<Type, DbMigrationsConfiguration>();

		public DbMigrationsConfiguration GetMigrationsConfiguration(Type dbContextType)
		{
			if (dbMigrationsConfigs.ContainsKey(dbContextType))
				return dbMigrationsConfigs[dbContextType];

			var dbMigrationsConfigType = dbContextType.Assembly
				.GetTypes()
				.Where(t => t.BaseType != null && t.BaseType.GenericTypeArguments.Any(gt => gt == dbContextType))
				.ToList()[0];	//	TODO what if there are more than one DbMigrationsConfiguration for the given DbContext?
			var dbMigrationsConfig = Activator.CreateInstance(dbMigrationsConfigType) as DbMigrationsConfiguration;
			dbMigrationsConfigs[dbContextType] = dbMigrationsConfig;

			return dbMigrationsConfig;
		}

		public ICollection<Migration> GetMigrations(DbMigrationsConfiguration dbMigrationsConfig, Database database)
		{
            dbMigrationsConfig.TargetDatabase = new DbConnectionInfo(database.ConnectionString, "System.Data.SqlClient");	//	TODO providername
			var dbMigrator = new DbMigrator(dbMigrationsConfig);
			var dbMigrations = dbMigrator.GetDatabaseMigrations();
			var localMigrations = dbMigrator.GetLocalMigrations();
			var pendingMigrations = dbMigrator.GetPendingMigrations();

			return
				localMigrations.Except(pendingMigrations)
					.Select(m => new Migration(m, MigrationState.Applied))
				.Union(pendingMigrations
					.Select(m => new Migration(m, MigrationState.Pending)))
				.ToList();
		}

		public bool DatabaseHasMigrationsFor(Database database, DbMigrationsConfiguration dbMigrationsConfig)
		{
			return database.HasMigrationHistory &&
				   database.MigrationHistoryRows.Any(r => r.ContextKey == dbMigrationsConfig.GetType().FullName);
		}

		public void UpdateDatabase(Database database, DbMigrationsConfiguration dbMigrationsConfig)
		{
			UpdateDatabase(database, dbMigrationsConfig, (string)null);
		}

		public void UpdateDatabase(Database database, DbMigrationsConfiguration dbMigrationsConfig, Migration targetMigration)
		{
			UpdateDatabase(database, dbMigrationsConfig, targetMigration.Name);
		}

		public void RollbackAllMigrations(Database database, DbMigrationsConfiguration dbMigrationsConfig)
		{
			UpdateDatabase(database, dbMigrationsConfig, "0");
		}

		private void UpdateDatabase(Database database, DbMigrationsConfiguration dbMigrationsConfig, string targetMigration)
		{
			dbMigrationsConfig.TargetDatabase = new DbConnectionInfo(database.ConnectionString, "System.Data.SqlClient");	//	TODO providername
			var migrator = new DbMigrator(dbMigrationsConfig);
			if (targetMigration == null)
				migrator.Update();
			else
				migrator.Update(targetMigration);

		}
    }
}
