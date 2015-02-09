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


		public Database SelectedDatabase
		{
			get { return selectedDatabse; }
			set { Set(ref selectedDatabse, value); }
		}
		private Database selectedDatabse;


		public RelayCommand OpenConnectDialogCommand { get; private set; }

		public RelayCommand<string> CopyCommand { get; private set; }

		private void OnNewConnection(Connection connection)
		{
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
			SelectedDatabase = db as Database;
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
	}
}