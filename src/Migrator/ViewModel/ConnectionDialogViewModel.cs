using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using PoliceSoft.Aquas.Model.Initializer.Models;
using PoliceSoft.Aquas.Model.Initializer.Services;
using System.Windows.Controls;
using GalaSoft.MvvmLight.CommandWpf;
using PoliceSoft.Aquas.Model.Initializer.Views;
using GalaSoft.MvvmLight.Messaging;
using PoliceSoft.Aquas.Model.Initializer.Messages;
using GalaSoft.MvvmLight.Ioc;
using System.Threading;

namespace PoliceSoft.Aquas.Model.Initializer.ViewModel
{
	public class ConnectionDialogViewModel : ViewModelBase
	{
		private readonly IConnectionService connectionService;

		public ConnectionDialogViewModel(IConnectionService connectionService)
		{
			this.connectionService = connectionService;
			availableConnections = new ObservableCollection<Connection>(connectionService.ActiveConnections);
			SelectConnectionWithHighestPriority();
			this.connectionService.NewConnection += OnNewConnection;

			AvailableConnections = CollectionViewSource.GetDefaultView(availableConnections);
			AvailableConnections.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Descending));

			AuthenticationModes = new AuthenticationMode[]
				{
					new AuthenticationMode(AuthenticationType.WindowsAuthentication, "Windows Authentication"),
					new AuthenticationMode(AuthenticationType.SqlServerAuthentication, "SQL Server Authentication")
				};
			selectedAuthenticationMode = AuthenticationModes.First();

			ConnectCommand = new RelayCommand<PasswordBox>(Connect, CanConnect);
			CancelCommand = new RelayCommand(Cancel, CanCancel);
			DialogClosingCommand = new RelayCommand(() => { if (ConnectInProgress) Cancel(); });
        }

		private void OnNewConnection(Connection connection)
		{
			availableConnections.Add(connection);
			if (!userSelectedConnection)
			{
				SelectConnectionWithHighestPriority();
			}
		}

		private void SelectConnectionWithHighestPriority()
		{
			if (availableConnections.Any())
			{
				selectedConnection = availableConnections.OrderByDescending(i => i.Priority).First();
				RaisePropertyChanged(() => SelectedConnection);
				NewDataSource = selectedConnection.DbConnection.DataSource;
			}
		}

		public ICollectionView AvailableConnections { get; private set; }
		public ObservableCollection<Connection> availableConnections;


		public Connection SelectedConnection
		{
			get { return selectedConnection; }
			set
			{
				userSelectedConnection = true;
				Set(ref selectedConnection, value);
			}
		}
		private Connection selectedConnection;
		private bool userSelectedConnection = false;

		public AuthenticationMode[] AuthenticationModes { get; private set; }


		public AuthenticationMode SelectedAuthenticationMode
		{
			get { return selectedAuthenticationMode; }
			set
			{
				Set(ref selectedAuthenticationMode, value);
				RaisePropertyChanged("SqlServerAuthenticationModeSelected");
			}
		}
		private AuthenticationMode selectedAuthenticationMode;

		public string NewDataSource
		{
			get { return newDataSource; }
			set { Set(ref newDataSource, value); }
		}
		public string newDataSource;


		public bool SqlServerAuthenticationModeSelected
		{
			get { return selectedAuthenticationMode.Type == AuthenticationType.SqlServerAuthentication; }
		}

		public string Username { get; set; }

		public bool ConnectInProgress
		{
			get { return connectInProgress; }
			set { Set(ref connectInProgress, value); }
		}
		private bool connectInProgress;

		private CancellationTokenSource taskCancellationTokenSource;

		public RelayCommand<PasswordBox> ConnectCommand { get; private set; }

		public RelayCommand CancelCommand { get; private set; }

		public RelayCommand DialogClosingCommand { get; private set; }

		private bool CanConnect(PasswordBox passwordBox)
		{
			return !ConnectInProgress;
		}

		private void Connect(PasswordBox passwordBox)
		{
			string dataSource = SelectedConnection == null ? NewDataSource : SelectedConnection.DbConnection.DataSource;
			Task<ConnectResult> task;
			taskCancellationTokenSource = new CancellationTokenSource();
			var token = taskCancellationTokenSource.Token;

			if (SelectedAuthenticationMode.Type == AuthenticationType.SqlServerAuthentication)
				task = Task.Factory.StartNew(() => connectionService.Connect(dataSource, Username, passwordBox.Password), token);
			else
				task = Task.Factory.StartNew(() => connectionService.Connect(dataSource), token);
			
			ConnectInProgress = true;
            task.ContinueWith(
				t =>
				{
					if (token.IsCancellationRequested)
						return;
					if (t.Result.Success)
						MessengerInstance.Send(new DialogClosedMessage<IConnectionDialog>());
					else
					{
						var errorDialog = SimpleIoc.Default.GetInstance<IErrorDialog>(Guid.NewGuid().ToString());
						errorDialog.Show();
						MessengerInstance.Send(new ErrorMessage("Failed to connect to database. See exception details below.", t.Result.Exception));
					}
					ConnectInProgress = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());			
		}

		private bool CanCancel()
		{
			return ConnectInProgress;
		}

		private void Cancel()
		{
			if (taskCancellationTokenSource != null)
				taskCancellationTokenSource.Cancel();
			ConnectInProgress = false;
        }
	}
}
