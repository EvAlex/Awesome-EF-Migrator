using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.History;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceSoft.Aquas.Model.Initializer.Models;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public interface IDbAnalyzerService
	{
		List<Database> GetDatabases(Connection connection);

		Database GetDatabase(Connection connection, string dbName);
	}
}
