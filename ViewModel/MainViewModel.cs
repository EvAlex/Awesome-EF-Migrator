using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using GalaSoft.MvvmLight;
using PoliceSoft.Aquas.Data.Storage;
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
		private readonly IConnectionService connectionService;
		private readonly IDbAnalyzerService dbAnalyzer;
		private readonly IDbMigratorService dbMigrator;

		private readonly Type dbContextType = typeof(AquasDb);
		private readonly DbMigrationsConfiguration dbMigrationsConfiguration;

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel(IConnectionService connectionService, IDbAnalyzerService dbAnalyzer, IDbMigratorService dbMigrator)
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				// Code runs "for real"
			}

			this.connectionService = connectionService;
			this.dbAnalyzer = dbAnalyzer;
			this.dbMigrator = dbMigrator;

			InitializeConnections();

			OpenConnectDialogCommand = new RelayCommand(OpenConnectDialog);
			CopyCommand = new RelayCommand<string>(str => Clipboard.SetText(str), str => !string.IsNullOrWhiteSpace(str));
			UpdateCommand = new RelayCommand<Database>(UpdateDatabase, CanUpdateDatabase);
			CreateDatabaseCommand = new AsyncRelayCommand<string, Database>(
				CreateDatabase,
				OnDatabaseCreated,
                CanCreateDatabase);
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
			}
		}
		private string newDatabaseName;
		
		public RelayCommand OpenConnectDialogCommand { get; private set; }

		public RelayCommand<string> CopyCommand { get; private set; }

		public RelayCommand<Database> UpdateCommand { get; private set; }

		public AsyncRelayCommand<string, Database> CreateDatabaseCommand { get; private set; }

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
					foreach (var d in connection.DatabasesView)
					{
						(d as Database).IsSelected = true;
						break;
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void OnDatabaseSelected(TreeViewItemModel db)
		{
			SelectedConnection = null;
			SelectedDatabase = db as Database;
			AnalyzingDbState = true;

			Task.Factory.StartNew(() => dbMigrator.GetMigrations(dbMigrator.GetMigrationsConfiguration(dbContextType), SelectedDatabase))
				.ContinueWith(
					t =>
					{
						foreach (var m in t.Result)
						{
							SelectedDatabase.Migrations.Add(m);
						}
						UpdateCommand.RaiseCanExecuteChanged();
						AnalyzingDbState = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

		private bool ShouldShowDatabase(Database database)
		{
			return database.HasMigrationHistory &&
				(!database.MigrationHistoryRows.Any() ||
				 dbMigrator.DatabaseHasMigrationsFor(database, dbMigrator.GetMigrationsConfiguration(dbContextType)));
		}

		private void OpenConnectDialog()
		{
			var connectionDialog = SimpleIoc.Default.GetInstance<IConnectionDialog>(Guid.NewGuid().ToString());
			MessengerInstance.Register<DialogClosedMessage<IConnectionDialog>>(this, msg => connectionDialog.Close());
			connectionDialog.ShowDialog();
		}

		private void UpdateDatabase(Database database)
		{
			dbMigrator.UpdateDatabase(database, dbMigrator.GetMigrationsConfiguration(dbContextType));
		}

		private bool CanUpdateDatabase(Database database)
		{
			return database.HasPendingMigrations;
		}

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
			return SelectedConnection != null && !SelectedConnection.Databases.Any(d => d.Name == dbName) && !CreateDatabaseCommand.InProgress;
        }
	}
}