﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.History;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	public class Database : TreeViewItemModel
	{
		public Database(string name, DbConnection dbConnection, ICollection<DatabaseTable> tables)
		{
			Name = name;
			Tables = tables;
			DbConnection = dbConnection;

			if (HasMigrationHistory)
				MigrationHistoryRows = new ObservableCollection<HistoryRow>(GetMigrationHistory());

			Migrations = new ObservableCollection<Migration>();
		}

		public string Name { get; private set; }

		public ICollection<DatabaseTable> Tables { get; private set; }

		public DbConnection DbConnection { get; private set; }


		//public Database(Type dbContextType)
		//{
		//	var dbMigrationsConfigType = dbContextType.Assembly
		//		.GetTypes()
		//		.Where(t => t.BaseType != null && t.BaseType.GenericTypeArguments.Any(gt => gt == dbContextType))
		//		.ToList()[0];
		//	var dbMigrationsConfig = Activator.CreateInstance(dbMigrationsConfigType) as DbMigrationsConfiguration;
		//	var dbMigrator = new DbMigrator(dbMigrationsConfig);
		//	var dbMigrations = dbMigrator.GetDatabaseMigrations();
		//	var localMigrations = dbMigrator.GetLocalMigrations();
		//	var pendingMigrations = dbMigrator.GetPendingMigrations();

		//	Migrations = new ObservableCollection<Migration>(
		//		localMigrations.Except(pendingMigrations)
		//			.Select(m => new Migration(m, MigrationState.Applied))
		//		.Union(pendingMigrations
		//			.Select(m => new Migration(m, MigrationState.Pending))));
		//}

		public ObservableCollection<Migration> Migrations { get; private set; }

		/// <summary>
		/// __MigrationHistory table rows in database.
		/// </summary>
		public ObservableCollection<HistoryRow> MigrationHistoryRows { get; private set; }

		public bool HasMigrationHistory
		{
			get
			{
				var historyTable = Tables.SingleOrDefault(t => t.Name == HistoryContext.DefaultTableName);
				return historyTable != null &&
					   historyTable.Columns.Any(c => c.Name == "MigrationId") &&
					   historyTable.Columns.Any(c => c.Name == "ContextKey") &&
					   historyTable.Columns.Any(c => c.Name == "Model") &&
					   historyTable.Columns.Any(c => c.Name == "ProductVersion");
			}
		}

		private ICollection<HistoryRow> GetMigrationHistory()
		{
			var res = new List<HistoryRow>();

			//return new HistoryContext(DbConnection, "dbo").History.ToList();
			using (var command = DbConnection.CreateCommand())
			{
				command.CommandText = string.Format("SELECT [MigrationId], [ContextKey], [Model], [ProductVersion] FROM [{0}].[dbo].[{1}]",
					Name,
					HistoryContext.DefaultTableName);
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						res.Add(new HistoryRow
						{
							MigrationId = reader.GetString(0),
							ContextKey = reader.GetString(1),
							Model = (byte[])reader[2],
							ProductVersion = reader.GetString(3)
						});
					}
				}
			}

			return res;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
