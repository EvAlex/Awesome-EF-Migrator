using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;

namespace PoliceSoft.Aquas.Model.Initializer
{
	public class AsyncRelayCommand : RelayCommand, INotifyPropertyChanged
	{
		private readonly Action execute;
		private readonly Func<bool> canExecute;

		public AsyncRelayCommand(Action execute)
			: base(execute)
		{
			this.execute = execute;
		}

		public AsyncRelayCommand(Action execute, Func<bool> canExecute)
			: base(execute, canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public override void Execute(object parameter)
		{
			InProgress = true;
			RaiseCanExecuteChanged();
			Task.Factory.StartNew(() => execute())
				.ContinueWith(
					t =>
					{
						InProgress = false;
						RaiseCanExecuteChanged();
					}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private bool inProgress;

		public bool InProgress
		{
			get { return inProgress; }
			set
			{
				inProgress = value;
				RaisePropertyChanged("InProgress");
			}
		}

		private void RaisePropertyChanged(string propertyName)
		{
			var temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

	}
}
