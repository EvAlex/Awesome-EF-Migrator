using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.History;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvAlex.AwesomeEfMigrator.Models;

namespace EvAlex.AwesomeEfMigrator.Services
{
	public interface IDbAnalyzerService
	{
		List<Database> GetDatabases(Connection connection);

		Database GetDatabase(Connection connection, string dbName);
	}
}
