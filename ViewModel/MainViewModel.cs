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

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel(IConnectionService connectionService)
        {
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				// Code runs "for real"

				Database = new Database(typeof(AquasDb));
			}

			this.connectionService = connectionService;
			Connections = new ObservableCollection<Connection>(connectionService.ActiveConnections);
			this.connectionService.NewConnection += c => Connections.Add(c);

			OpenConnectDialogCommand = new RelayCommand(OpenConnectDialog);
        }

		public Database Database { get; private set; }

		public ObservableCollection<Connection> Connections { get; private set; }

		public RelayCommand OpenConnectDialogCommand { get; private set; }

		private void OpenConnectDialog()
		{
			var connectionDialog = SimpleIoc.Default.GetInstance<IConnectionDialog>(Guid.NewGuid().ToString());
			MessengerInstance.Register<DialogClosedMessage<IConnectionDialog>>(this, msg => connectionDialog.Close());
			connectionDialog.ShowDialog();
        }
	}
}