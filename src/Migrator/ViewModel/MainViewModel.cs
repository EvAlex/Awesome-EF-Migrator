using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using GalaSoft.MvvmLight;
using System.Globalization;
using PoliceSoft.Aquas.Model.Initializer.Models;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using PoliceSoft.Aquas.Model.Initializer.Views;
using PoliceSoft.Aquas.Model.Initializer.Services;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Ioc;
using PoliceSoft.Aquas.Model.Initializer.Messages;
using System.Windows.Data;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace PoliceSoft.Aquas.Model.Initializer.ViewModel
{
	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// <para>
	/// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
	/// </para>
	/// <para>
	/// You can also use Blend to data bind with the tool's support.
	/// </para>
	/// <para>
	/// See http://www.galasoft.ch/mvvm
	/// </para>
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		private readonly IEfConfigLoader efConfigLoader;
		private readonly IConnectionService connectionService;
		private readonly IDbAnalyzerService dbAnalyzer;
		private readonly IDbMigratorService dbMigrator;

		private readonly Type dbContextType;
		private readonly DbMigrationsConfiguration dbMigrationsConfiguration;

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel(IEfConfigLoader efConfigLoader, IConnectionService connectionService, IDbAnalyzerService dbAnalyzer, IDbMigratorService dbMigrator)
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				// Code runs "for real"
			}


			this.efConfigLoader = efConfigLoader;
			this.connectionService = connectionService;
			this.dbAnalyzer = dbAnalyzer;
			this.dbMigrator = dbMigrator;

			dbContextType = LoadDbContextType();

			InitializeConnections();

			InitializeCommands();
		}

		private Type LoadDbContextType()
		{
			Type res;

			var configs = efConfigLoader.LoadConfigs();
			var configBuilder = new AwesomeEfMigratorConfigBuilder();
			foreach (var c in configs)
			{
				c.CallConfigure(configBuilder);
			}

			if (configBuilder.DbContextTypes.Count == 1)
				res = configBuilder.DbContextTypes.First();
			else
				res = SelectDbContextType(configBuilder.DbContextTypes);

			return res;
		}

		private Type SelectDbContextType(IReadOnlyCollection<Type> dbContextTypes)
		{
			throw new NotImplementedException();
		}

		private void InitializeConnections()
		{
			Connections = new ObservableCollection<Connection>(connectionService.ActiveConnections);
			foreach (var c in Connections)
			{
				OnNewConnection(c);
			}
			connectionService.NewConnection +=
				c =>
				{
					OnNewConnection(c);
					Connections.Add(c);
				};

			ConnectionsView = CollectionViewSource.GetDefaultView(Connections);
			ConnectionsView.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Descending));
		}

		private void InitializeCommands()
		{
			OpenConnectDialogCommand = new RelayCommand(OpenConnectDialog);
			CopyCommand = new RelayCommand<string>(str => Clipboard.SetText(str), str => !string.IsNullOrWhiteSpace(str));
			UpdateCommand = new AsyncRelayCommand<Database, Database>(
				UpdateDatabase,
				OnDatabaseUpdated,
				CanUpdateDatabase);
			CreateDatabaseCommand = new AsyncRelayCommand<string, Database>(
				CreateDatabase,
				OnDatabaseCreated,
				CanCreateDatabase);

			UpdateToMigrationCommand = new AsyncRelayCommand<Migration, Database>(
				UpdateDatabase,
				OnDatabaseUpdated,
				CanUpdateDatabase);

			RollbackAllMigrationsCommand = new AsyncRelayCommand<Database, Database>(
				RollbackAllMigrations,
				OnRolledBackAllMigrations,
				CanRollbackAllMigrations);
		}

		public Database Database { get; private set; }

		public ObservableCollection<Connection> Connections { get; private set; }

		public ICollectionView ConnectionsView { get; private set; }


		public Connection SelectedConnection
		{
			get { return selectedConnection; }
			set { Set(ref selectedConnection, value); }
		}
		private Connection selectedConnection;

		public Database SelectedDatabase
		{
			get { return selectedDatabse; }
			set { Set(ref selectedDatabse, value); }
		}
		private Database selectedDatabse;

		public string NewDatabaseName
		{
			get { return newDatabaseName; }
			set
			{
				Set(ref newDatabaseName, value);
				CreateDatabaseCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged(() => NewDbNameInUse);
            }
		}
		private string newDatabaseName;


		public RelayCommand<string> CopyCommand { get; private set; }





		/// <summary>
		/// Connection that is currently used to create database
		/// </summary>
		private Connection createDbConnection;

		/// <summary>
		/// Indicates that satate is being analyzed for <see cref="SelectedDatabase"/>
		/// </summary>
		public bool AnalyzingDbState
		{
			get { return analyzingState; }
			set { Set(ref analyzingState, value); }
		}
		private bool analyzingState;


		private void OnNewConnection(Connection connection)
		{
			connection.Selected +=
				c =>
				{
					SelectedConnection = c as Connection;
					SelectedDatabase = null;
					NewDatabaseName = dbContextType.Name;
				};
			connection.DatabasesView.Filter = obj => ShouldShowDatabase(obj as Database);

			Task.Factory.StartNew(() => dbAnalyzer.GetDatabases(connection))
				.ContinueWith(
				t =>
				{
					foreach (var d in t.Result)
					{
						connection.Databases.Add(d);
						d.Selected += OnDatabaseSelected;
					}

					connection.IsExpanded = true;

					if (connection.Priority == DataSourcePriority.UserAdded || 
					    SelectedDatabase == null ||
						GetParentConnection(SelectedDatabase).Priority < connection.Priority)
					{
						foreach (var d in connection.DatabasesView)
						{
							(d as Database).IsSelected = true;
							break;
						}
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private Connection GetParentConnection(Database database)
		{
			return Connections.Single(c => c.Databases.Any(d => d == database));
		}

		private void OnDatabaseSelected(TreeViewItemModel db)
		{
			SelectedConnection = null;
			SelectedDatabase = db as Database;
			RefreshDatabaseDetails(SelectedDatabase);
		}

		//private void RefreshDatabaseDetails(Database database)
		//{
		//	AnalyzingDbState = true;
		//	Task.Factory.StartNew(
		//			() => 
		//				dbMigrator.GetMigrations(dbMigrator.GetMigrationsConfiguration(dbContextType), database))
		//		.ContinueWith(
		//			t =>
		//			{
		//				database.Migrations.Clear();
		//				foreach (var m in t.Result)
		//				{
		//					database.Migrations.Add(m);
		//				}
		//				UpdateCommand.RaiseCanExecuteChanged();
		//				UpdateToMigrationCommand.RaiseCanExecuteChanged();
		//				RollbackAllMigrationsCommand.RaiseCanExecuteChanged();
		//				AnalyzingDbState = false;
		//			}, TaskScheduler.FromCurrentSynchronizationContext());
		//}

		private async Task RefreshDatabaseDetails(Database database)
		{
			AnalyzingDbState = true;
			var migrations = await GetDbMigrations(database);

			database.Migrations.Clear();
			foreach (var m in migrations)
			{
				database.Migrations.Add(m);
			}
			UpdateCommand.RaiseCanExecuteChanged();
			UpdateToMigrationCommand.RaiseCanExecuteChanged();
			RollbackAllMigrationsCommand.RaiseCanExecuteChanged();
			AnalyzingDbState = false;
		}

		private async Task<ICollection<Migration>> GetDbMigrations(Database database)
		{
			return await Task.Run(() => dbMigrator.GetMigrations(dbMigrator.GetMigrationsConfiguration(dbContextType), database));
        }

		private bool ShouldShowDatabase(Database database)
		{
			return database.HasMigrationHistory &&
				(!database.MigrationHistoryRows.Any() ||
				 dbMigrator.DatabaseHasMigrationsFor(database, dbMigrator.GetMigrationsConfiguration(dbContextType)));
		}

		#region OpenConnectDialogCommand

		public RelayCommand OpenConnectDialogCommand { get; private set; }

		private void OpenConnectDialog()
		{
			var connectionDialog = SimpleIoc.Default.GetInstance<IConnectionDialog>(Guid.NewGuid().ToString());
			MessengerInstance.Register<DialogClosedMessage<IConnectionDialog>>(this, msg => connectionDialog.Close());
			connectionDialog.ShowDialog();
		}

		#endregion

		#region UpdateCommand

		public AsyncRelayCommand<Database, Database> UpdateCommand { get; private set; }

		private Database UpdateDatabase(Database database)
		{
			dbMigrator.UpdateDatabase(database, dbMigrator.GetMigrationsConfiguration(dbContextType));
			return database;
        }

		private bool CanUpdateDatabase(Database database)
		{
			return database != null && database.HasPendingMigrations;
		}

		#endregion

		#region CreateDatabaseCommand

		public AsyncRelayCommand<string, Database> CreateDatabaseCommand { get; private set; }

		private Database CreateDatabase(string dbName)
		{
			createDbConnection = SelectedConnection;
			UpdateDatabase(new Database(dbName, createDbConnection.DbConnection, null));
			return dbAnalyzer.GetDatabase(createDbConnection, dbName);
		}

		private void OnDatabaseCreated(Database database)
		{
			createDbConnection.Databases.Add(database);
			database.Selected += OnDatabaseSelected;
			database.IsSelected = true;
			createDbConnection = null;
		}

		private bool CanCreateDatabase(string dbName)
		{
			return !DbNameInUse(selectedConnection, dbName) && !CreateDatabaseCommand.InProgress;
		}

		public bool NewDbNameInUse
		{
			get { return DbNameInUse(selectedConnection, newDatabaseName); }
		}

		private bool DbNameInUse(Connection connection, string dbName)
		{
			return connection != null && connection.Databases.Any(d => d.Name == dbName);
        }


		#endregion

		#region UpdateToMigrationCommand

		public Migration SelectedMigration
		{
			get { return selectedMigration; }
			set
			{
				Set(ref selectedMigration, value);
				UpdateToMigrationCommand.RaiseCanExecuteChanged();
            }
		}
		private Migration selectedMigration;

		public AsyncRelayCommand<Migration, Database> UpdateToMigrationCommand { get; private set; }

		private Database UpdateDatabase(Migration targetMigration)
		{
			var database = SelectedDatabase;
			dbMigrator.UpdateDatabase(database, dbMigrator.GetMigrationsConfiguration(dbContextType), targetMigration);
			return database;
		}

		private void OnDatabaseUpdated(Database database)
		{
			RefreshDatabaseDetails(database);
		}

		private bool CanUpdateDatabase(Migration targetMigration)
		{
			return SelectedDatabase != null && 
				targetMigration != null && 
				!UpdateToMigrationCommand.InProgress && !RollbackAllMigrationsCommand.InProgress;
		}

		#endregion

		#region RollbackAllMigrationsCommand

		public AsyncRelayCommand<Database, Database> RollbackAllMigrationsCommand { get; private set; }

		private Database RollbackAllMigrations(Database database)
		{
			dbMigrator.RollbackAllMigrations(database, dbMigrator.GetMigrationsConfiguration(dbContextType));
			return database;
		}

		private void OnRolledBackAllMigrations(Database database)
		{
			RefreshDatabaseDetails(database);
		}

		private bool CanRollbackAllMigrations(Database database)
		{
			return database != null && database.Migrations.Any(m => m.State == MigrationState.Applied) &&
				!UpdateToMigrationCommand.InProgress && !RollbackAllMigrationsCommand.InProgress;
		}

		#endregion
	}
}