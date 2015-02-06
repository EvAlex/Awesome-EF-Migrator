using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	public class Database : ObservableObject
	{
		public Database(Type dbContextType)
		{
			var dbMigrationsConfigType = dbContextType.Assembly
				.GetTypes()
				.Where(t => t.BaseType != null && t.BaseType.GenericTypeArguments.Any(gt => gt == dbContextType))
				.ToList()[0];
			var dbMigrationsConfig = Activator.CreateInstance(dbMigrationsConfigType) as DbMigrationsConfiguration;
			var dbMigrator = new DbMigrator(dbMigrationsConfig);
			var dbMigrations = dbMigrator.GetDatabaseMigrations();
			var localMigrations = dbMigrator.GetLocalMigrations();
			var pendingMigrations = dbMigrator.GetPendingMigrations();
			
			Migrations = new ObservableCollection<Migration>(
				localMigrations.Except(pendingMigrations)
					.Select(m => new Migration(m, MigrationState.Applied))
				.Union(pendingMigrations
					.Select(m => new Migration(m, MigrationState.Pending))));
		}

		public ObservableCollection<Migration> Migrations { get; private set; }
	}
}
