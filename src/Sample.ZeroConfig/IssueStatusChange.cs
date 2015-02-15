using System;

namespace Sample.ZeroConfig
{
	public class IssueStatusChange
	{
		public int Id { get; set; }

		public DateTime ChangeTime { get; set; }

		public int InitiatorId { get; set; }

		public virtual User Initiator { get; set; }

		public int IssueId { get; set; }

		public virtual Issue Issue { get; set; }

		public int StatusId { get; set; }

		public virtual IssueStatus Status { get; set; }
	}
}