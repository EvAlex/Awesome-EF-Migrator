using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PoliceSoft.Aquas.Model.Initializer.Models;

namespace PoliceSoft.Aquas.Model.Initializer
{
	public class AsyncRelayCommand<T, TResult> : ObservableObject, ICommand
	{
		private readonly Func<T, TResult> asyncExecute;
		private readonly Action<TResult> executeOnUiThread;
		private readonly Func<T, bool> canExecute;

		public AsyncRelayCommand(Func<T, TResult> asyncExecute, Action<TResult> executeOnUiThread)
			: this(asyncExecute, executeOnUiThread, t => true)
		{
		}

		public AsyncRelayCommand(Func<T, TResult> asyncExecute, Action<TResult> executeOnUiThread, Func<T, bool> canExecute)
		{
			this.asyncExecute = asyncExecute;
			this.executeOnUiThread = executeOnUiThread;
			this.canExecute = canExecute;
		}

		public bool InProgress
		{
			get { return inProgress; }
			set
			{
				Set(ref inProgress, value);
				RaiseCanExecuteChanged();
			}
		}
		private bool inProgress;

		public void RaiseCanExecuteChanged()
		{
			var temp = CanExecuteChanged;
			if (temp != null)
			{
				temp(this, new EventArgs());
			}
		}
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			var arg = parameter == null ? default(T) : (T)parameter;
			return canExecute(arg);
		}

		public void Execute(object parameter)
		{
			var arg = parameter == null ? default(T) : (T)parameter;
			InProgress = true;
			Task.Factory.StartNew(() => asyncExecute(arg))
				.ContinueWith(
					t =>
					{
						executeOnUiThread(t.Result);
						InProgress = false;
					}, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
