using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceSoft.Aquas.Model.Initializer
{
	public class AwesomeEfMigratorConfigBuilder
	{
		internal AwesomeEfMigratorConfigBuilder()
		{
		}

		private readonly List<Type> dbContextTypes = new List<Type>();
		internal IReadOnlyCollection<Type> DbContextTypes
		{
			get { return dbContextTypes.AsReadOnly(); }
		}

		public void UseDbContext<TDbContext>()
			where TDbContext : DbContext
		{
			UseDbContext(typeof(TDbContext));
		}

		internal void UseDbContext(Type dbContextType)
		{
			dbContextTypes.Add(dbContextType);
		}
	}
}
