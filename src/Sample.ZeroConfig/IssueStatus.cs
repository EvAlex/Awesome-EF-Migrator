using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sample.ZeroConfig
{
	public class IssueStatus
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }

		public virtual ICollection<IssueStatusChange> StatusChanges { get; set; }
	}
}