using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceSoft.Aquas.Model.Initializer.Models;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public class SqlServerDbAnalyzerService : IDbAnalyzerService
	{
		public List<Database> GetDatabases(Connection connection)
		{
			if (connection.DbConnection == null)
				throw new ArgumentNullException("connection");
			if (connection.DbConnection.State != ConnectionState.Open)
				throw new ArgumentException("Connection state should be Open.", "connection");

			return GetDatabases(connection.DbConnection);
		}

		private List<Database> GetDatabases(DbConnection dbConnection)
		{
			return GetDatabaseNames(dbConnection)
				.Select(n => new Database(n, GetDatabaseTables(dbConnection, n)))
				.ToList();
		}

		private ICollection<string> GetDatabaseNames(DbConnection dbConnection)
		{
			return Query(
				dbConnection, 
				"SELECT name FROM sys.databases WHERE state = 0", 
				r => r.GetString(0));
		}

		private ICollection<DatabaseTable> GetDatabaseTables(DbConnection dbConnection, string dbName)
		{
			var res = new List<DatabaseTable>();

            var sql = string.Format(@"
				USE [{0}];
				SELECT TABLE_SCHEMA,
					   TABLE_NAME,
					   COLUMN_NAME,
					   DATA_TYPE,
					   IS_NULLABLE,
					   CHARACTER_MAXIMUM_LENGTH 
				FROM INFORMATION_SCHEMA.COLUMNS", dbName);
			using (var reader = Query(dbConnection, sql))
			{
				while (reader.Read())
				{
					string tableSchema = reader.GetString(0);
					string tableName = reader.GetString(1);
					var table = res.SingleOrDefault(t => t.Name == tableName && t.Schema == tableSchema);
					if (table == null)
						res.Add(table = new DatabaseTable(tableName, tableSchema));
					table.Columns.Add(
						new TableColumn(
							reader.GetString(2),
							new Models.DbType(
								reader.GetString(3),
								reader.GetString(4) == "YES",
								reader.IsDBNull(5) ? null : (int?)reader.GetInt32(5))));
                }
			}

			return res;
        }

		private DbDataReader Query(DbConnection dbConnection, string sql)
		{
			using (var command = dbConnection.CreateCommand())
			{
				command.CommandText = sql;
				return command.ExecuteReader();
            }
		}

		private ICollection<TResult> Query<TResult>(DbConnection dbConnection, string sql, Func<DbDataReader, TResult> itemReader)
		{
			var res = new List<TResult>();

			using (var command = dbConnection.CreateCommand())
			{
				command.CommandText = sql;
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						res.Add(itemReader(reader));
					}
				}
			}

			return res;
		}
	}
}
