using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PoliceSoft.Aquas.Model.Initializer.ViewModel
{
	public class TreeViewModel : ViewModelBase
	{
		public object SelectedItem
		{
			get { return selectedItem; }
			set
			{
				if (selectedItem != value)
				{
					selectedItem = value;
				}
			}
		}
		private object selectedItem = null;

		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				Set(ref isSelected, value);
				if (isSelected)
					SelectedItem = this;
			}
		}
		private bool isSelected;
	}
}
