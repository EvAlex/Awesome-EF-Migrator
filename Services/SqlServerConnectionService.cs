using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PoliceSoft.Aquas.Model.Initializer.Models;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public class SqlServerConnectionService : IConnectionService
	{
		private readonly Connection[] defaultConnections = new Connection[]
			{
				new Connection(PrepareDbConnection("(local)"), DataSourcePriority.AutoAddedHigh),
				new Connection(PrepareDbConnection(@".\SQLEXPRESS"), DataSourcePriority.AutoAddedNormal),
				new Connection(PrepareDbConnection(@"(LocalDB)\v11.0"), DataSourcePriority.AutoAddedLow),
				new Connection(PrepareDbConnection(@"(LocalDB)\v12.0"), DataSourcePriority.AutoAddedLow),
			};

		private List<Connection> activeConnections = new List<Connection>();

		private static DbConnection PrepareDbConnection(string dataSource)
		{
			return new SqlConnection(string.Format("Data Source = {0}; Integrated Security = True;", dataSource));
		}

		private static DbConnection PrepareDbConnection(string dataSource, string username, string password)
		{
			return new SqlConnection(string.Format("Data Source = {0}; User Id = {1}; Password = {2}",
				dataSource,
				username,
				password));
		}

		public SqlServerConnectionService()
		{
			Task.WhenAll(defaultConnections.Select(conn => Task.Factory.StartNew(
				() => TryConnect(conn))));
		}

		private void TryConnect(Connection connection)
		{
			try
			{
				connection.DbConnection.Open();
				if (ConnectionIsNew(connection))
				{
					activeConnections.Add(connection);
					RaiseNewConnection(connection);
				}
				RaiseConnected(connection);
			}
			catch (Exception)
			{
			}
		}

		private bool ConnectionIsNew(Connection connection)
		{
			return !activeConnections.Any(c => c.DbConnection.ConnectionString == connection.DbConnection.ConnectionString);
        }

		private void RaiseConnected(Connection connection)
		{
			var temp = Connected;
			if (temp != null)
			{
				Application.Current.Dispatcher.BeginInvoke((Action)(() => temp(connection)));
			}
		}

		private void RaiseNewConnection(Connection connection)
		{
			var temp = NewConnection;
			if (temp != null)
			{
				Application.Current.Dispatcher.BeginInvoke((Action)(() => temp(connection)));
			}
		}

		public void Connect(string dataSource)
		{
			var connection = new Connection(PrepareDbConnection(dataSource), DataSourcePriority.UserAdded);
			TryConnect(connection);
		}

		public void Connect(string dataSource, string username, string password)
		{
			var connection = new Connection(PrepareDbConnection(dataSource, username, password), DataSourcePriority.UserAdded);
			TryConnect(connection);
		}

		public IReadOnlyCollection<Connection> ActiveConnections
		{
			get { return activeConnections.AsReadOnly(); }
		}

		public event Action<Connection> Connected;
		public event Action<Connection> NewConnection;
	}
}
