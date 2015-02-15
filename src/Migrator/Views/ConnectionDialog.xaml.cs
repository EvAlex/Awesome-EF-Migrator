using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EvAlex.AwesomeEfMigrator.ViewModel;

namespace EvAlex.AwesomeEfMigrator.Views
{
	/// <summary>
	/// Interaction logic for ConnectionDialog.xaml
	/// </summary>
	public partial class ConnectionDialog : Window, IConnectionDialog
	{
		public ConnectionDialog()
		{
			InitializeComponent();
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (DataContext is ConnectionDialogViewModel)
				(DataContext as ConnectionDialogViewModel).DialogClosingCommand.Execute(null);
		}
	}
}
