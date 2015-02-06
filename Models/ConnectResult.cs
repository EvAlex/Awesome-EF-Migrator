using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	public class ConnectResult
	{
		public ConnectResult(Exception exception)
		{
			this.exception = exception;
			this.success = exception == null;
        }

		public Exception Exception
		{
			get { return exception; }
		}
		private readonly Exception exception;

		public bool Success
		{
			get { return success; }
		}
		private readonly bool success;
	}
}
