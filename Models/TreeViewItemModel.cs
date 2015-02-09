using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	public class TreeViewItemModel : ObservableObject
	{

		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				Set(ref isSelected, value);
			}
		}
		private bool isSelected;

		public bool IsExpanded
		{
			get { return isExpanded; }
			set
			{
				Set(ref isExpanded, value);
			}
		}
		private bool isExpanded;
	}
}
