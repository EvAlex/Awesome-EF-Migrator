using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.ZeroConfig
{
	public class Issue
	{
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }

		public string Description { get; set; }

		public int IssuerId { get; set; }

		[ForeignKey("IssuerId")]
		public virtual User Issuer { get; set; }

		public int? AssigneeId { get; set; }

		[ForeignKey("AssigneeId")]
		public virtual User Assignee { get; set; }

		public DateTime IssueTime { get; set; }

		public virtual ICollection<IssueStatusChange> StatusChanges { get; set; }
	}
}