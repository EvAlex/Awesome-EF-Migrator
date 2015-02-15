using System.Collections.Generic;

namespace Sample.ZeroConfig
{
	public class User
	{
		public int Id { get; set; }

		public string Username { get; set; }

		public string Fullname { get; set; }
		public virtual ICollection<Issue> IssuedIssues { get; internal set; }
		public virtual ICollection<Issue> AssignedIssues { get; internal set; }
		public virtual ICollection<IssueStatusChange> IssueStatusChanges { get; internal set; }
	}
}