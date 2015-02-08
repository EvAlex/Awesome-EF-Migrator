namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	public class TableColumn
	{
		public TableColumn(string name, DbType type)
		{
			Name = name;
			Type = type;
		}

		public string Name { get; private set; }

		public DbType Type { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} {1}", Name, Type);
		}
	}
}