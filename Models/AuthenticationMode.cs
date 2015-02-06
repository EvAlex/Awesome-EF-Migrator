using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	public class AuthenticationMode
	{
		public AuthenticationMode(AuthenticationType authType, string name)
		{
			Type = authType;
			Name = name; 
        }

		public AuthenticationType Type { get; private set; }

		public string Name { get; private set; }
	}
}
