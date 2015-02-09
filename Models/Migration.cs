using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	public class Migration : ObservableObject
	{
		/// <summary>
		/// Copy-pasted from EntityFramework System.Data.Entity.Migrations.Utilities.UtcNowGenerator
		/// </summary>
		public const string MigrationIdFormat = "yyyyMMddHHmmssf";

		public Migration(string migrationName, MigrationState migrationState)
		{
			Id = migrationName.Split('_').First();
			Name = migrationName.Substring(Id.Length + 1);
			Timestamp = DateTime.ParseExact(Id, MigrationIdFormat, CultureInfo.InvariantCulture);
			State = migrationState;
        }

		public string Id { get; private set; }

		public string Name { get; private set; }

		public DateTime Timestamp { get; private set; }

		public MigrationState State
		{
			get { return state; }
			set { Set(ref state, value); }
		}
		private MigrationState state;
	}
}
