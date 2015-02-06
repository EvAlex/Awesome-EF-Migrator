using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.ServiceLocation;

namespace PoliceSoft.Aquas.Model.Initializer.Services
{
	public class DialogService
	{
		public TDialogWindow CreateDialog<TDialogWindow>()
			where TDialogWindow : Window
		{
			return null;
			//return ServiceLocator.Current.
		}
	}
}
