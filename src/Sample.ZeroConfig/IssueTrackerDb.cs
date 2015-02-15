using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ZeroConfig
{
	public class IssueTrackerDb : DbContext
	{
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Issue>()
				.HasRequired(i => i.Issuer)
				.WithMany(u => u.IssuedIssues)
				.HasForeignKey(i => i.IssuerId)
				.WillCascadeOnDelete(false);
			modelBuilder.Entity<Issue>()
				.HasOptional(i => i.Assignee)
				.WithMany(u => u.AssignedIssues)
				.HasForeignKey(i => i.AssigneeId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<IssueStatusChange>()
				.HasRequired(i => i.Initiator)
				.WithMany(u => u.IssueStatusChanges)
				.HasForeignKey(i => i.InitiatorId)
				.WillCascadeOnDelete(false);
			modelBuilder.Entity<IssueStatusChange>()
				.HasRequired(i => i.Issue)
				.WithMany(u => u.StatusChanges)
				.HasForeignKey(i => i.IssueId)
				.WillCascadeOnDelete(false);
			modelBuilder.Entity<IssueStatusChange>()
				.HasRequired(i => i.Status)
				.WithMany(u => u.StatusChanges)
				.HasForeignKey(i => i.StatusId)
				.WillCascadeOnDelete(false);

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Issue> Issues { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<IssueStatus> IssueStatuses { get; set; }

		public DbSet<IssueStatusChange> IssueStatusChanges { get; set; }
	}
}
