using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	/// <summary>
	/// Represents Data Source in connection string
	/// </summary>
	public class Connection : ObservableObject
	{
		public Connection(DbConnection dbConnection, DataSourcePriority priority)
		{
			DbConnection = dbConnection;
			Priority = priority;
		}

		public DbConnection DbConnection { get; private set; }

		public DataSourcePriority Priority { get; private set; }
	}
}
