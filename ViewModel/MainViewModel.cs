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

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel(IConnectionService connectionService, IDbAnalyzerService dbAnalyzer)
        {
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				// Code runs "for real"

				//Database = new Database(typeof(AquasDb));
			}

			this.connectionService = connectionService;
			this.dbAnalyzer = dbAnalyzer;

			InitializeConnections();

			OpenConnectDialogCommand = new RelayCommand(OpenConnectDialog);
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

		public RelayCommand OpenConnectDialogCommand { get; private set; }

		private void OnNewConnection(Connection connection)
		{
			connection.Databases = dbAnalyzer.GetDatabases(connection);
		}

		private void OpenConnectDialog()
		{
			var connectionDialog = SimpleIoc.Default.GetInstance<IConnectionDialog>(Guid.NewGuid().ToString());
			MessengerInstance.Register<DialogClosedMessage<IConnectionDialog>>(this, msg => connectionDialog.Close());
			connectionDialog.ShowDialog();
        }
	}
}