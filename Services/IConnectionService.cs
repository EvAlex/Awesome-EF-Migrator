using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceSoft.Aquas.Model.Initializer.Models;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public interface IConnectionService
	{
		/// <summary>
		/// Raised everytime connection is opened
		/// </summary>
		event Action<Connection> Connected;

		/// <summary>
		/// Raised when new connection is opened
		/// </summary>
		event Action<Connection> NewConnection;

		IReadOnlyCollection<Connection> ActiveConnections { get; }

		void Connect(string dataSource);

		void Connect(string dataSource, string username, string password);
	}
}
