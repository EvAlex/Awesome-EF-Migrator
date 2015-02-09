using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;

namespace PoliceSoft.Aquas.Model.Initializer
{
	public class AsyncRelayCommand<T> : RelayCommand<T>, INotifyPropertyChanged
	{
		private readonly Action<T> execute;
		private readonly Func<T, bool> canExecute;

		public AsyncRelayCommand(Action<T> execute)
			: base(execute)
		{
			this.execute = execute;
		}

		public AsyncRelayCommand(Action<T> execute, Func<T, bool> canExecute)
			: base(execute, canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public override void Execute(object parameter)
		{
			InProgress = true;
			RaiseCanExecuteChanged();
			Task.Factory.StartNew(() => execute((T)parameter))
				.ContinueWith(
					t =>
					{
						InProgress = false;
						RaiseCanExecuteChanged();
					}, TaskScheduler.FromCurrentSynchronizationContext());
		}


		public bool InProgress
		{
			get { return inProgress; }
			set
			{
				inProgress = value;
				RaisePropertyChanged("InProgress");
			}
		}
		private bool inProgress;

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
