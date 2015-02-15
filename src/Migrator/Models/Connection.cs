using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;

namespace EvAlex.AwesomeEfMigrator.Models
{
	/// <summary>
	/// Represents Data Source in connection string
	/// </summary>
	public class Connection : TreeViewItemModel
	{
		public Connection(DbConnection dbConnection, DataSourcePriority priority)
		{
			DbConnection = dbConnection;
			Priority = priority;
			Databases = new ObservableCollection<Database>();
			DatabasesView = CollectionViewSource.GetDefaultView(Databases);
        }

		public DbConnection DbConnection { get; private set; }

		public DataSourcePriority Priority { get; private set; }

		public ObservableCollection<Database> Databases { get; private set; }

		public ICollectionView DatabasesView { get; private set; }
	}
}
