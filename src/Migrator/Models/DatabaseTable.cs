using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvAlex.AwesomeEfMigrator.Models
{
	public class DatabaseTable : TreeViewItemModel
	{
		public DatabaseTable(string name, string schemaName)
			: this(schemaName + "." + name)
		{
		}

		public DatabaseTable(string fullName)
		{
			FullName = fullName;
			if (fullName.Contains("."))
			{
				Schema = fullName.Split('.').First();
				Name = fullName.Substring(Schema.Length + 1);
			}
			else
				Name = fullName;

			Columns = new List<TableColumn>();
        }

		public string Schema { get; private set; }

		public string Name { get; private set; }

		public string FullName { get; private set; }

		public ICollection<TableColumn> Columns { get; private set; }

		public override string ToString()
		{
			return FullName;
		}
	}
}
