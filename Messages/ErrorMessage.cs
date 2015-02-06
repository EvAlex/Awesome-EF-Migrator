using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PoliceSoft.Aquas.Model.Initializer.Messages
{
	public class ErrorMessage : ObservableObject
	{
		public ErrorMessage(string message)
			:  this(message, null)
		{
		}

		public ErrorMessage(Exception exception)
			:this (null, exception)
		{
		}

		public ErrorMessage(string message, Exception exception)
		{
			Message = message;
			Exception = exception;
		}


		public string Message
		{
			get { return message; }
			set { Set(ref message, value); }
		}
		private string message;


		public Exception Exception
		{
			get { return exception; }
			set
			{
				Set(ref exception, value);
				RaisePropertyChanged(() => ExceptionType);
				RaisePropertyChanged(() => HasException);
			}
		}
		private Exception exception;

		public Type ExceptionType
		{
			get { return exception == null ? null : exception.GetType(); }
		}

		public bool HasException
		{
			get { return exception != null; }
		}
    }
}
