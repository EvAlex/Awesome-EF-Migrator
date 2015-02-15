using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace EvAlex.AwesomeEfMigrator.Models
{
	public class TreeViewItemModel : ObservableObject
	{

		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				Set(ref isSelected, value);
				if (isSelected)
					RaiseSelected();
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

		private void RaiseSelected()
		{
			var temp = Selected;
			if (temp != null)
			{
				temp(this);
			}
		}
		public event Action<TreeViewItemModel> Selected;
	}
}
