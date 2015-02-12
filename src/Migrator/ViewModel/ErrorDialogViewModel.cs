using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PoliceSoft.Aquas.Model.Initializer.Messages;

namespace PoliceSoft.Aquas.Model.Initializer.ViewModel
{
	public class ErrorDialogViewModel : ViewModelBase
	{
		public ErrorDialogViewModel()
		{
			MessengerInstance.Register<ErrorMessage>(this, OnErrorMessage);
		}

		private void OnErrorMessage(ErrorMessage message)
		{
			Error = message;
		}

		private ErrorMessage error;

		public ErrorMessage Error
		{
			get { return error; }
			set { Set(ref error, value); }
		}

	}
}
