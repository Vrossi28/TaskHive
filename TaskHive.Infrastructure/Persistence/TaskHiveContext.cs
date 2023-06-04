using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Core.Enums;

namespace TaskHive.Infrastructure.Persistence
{
    public class TaskHiveContext : DbContext
    {
        public TaskHiveContext() { }
        public TaskHiveContext(DbContextOptions<TaskHiveContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder();

                configuration.SetBasePath(System.IO.Directory.GetCurrentDirectory());
                configuration.AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: false);
                configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true);
                configuration.AddEnvironmentVariables();

                IConfigurationRoot Configuration = configuration.Build();

                optionsBuilder.UseSqlServer(Configuration.GetConnectionString(InfrastructureContants.ConnectionString));

                Configuration = null;

                configuration = null;
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            #region Account
            builder.Entity<Account>(u =>
            {
                u.Property(a => a.AccountId).ValueGeneratedNever();

                u.HasMany(c => c.AccountLoggedWorks)
                     .WithOne(c => c.Account)
                     .HasForeignKey(c => c.AccountId)
                     .HasPrincipalKey(c => c.AccountId);

                u.HasMany(c => c.AccountWorkspaces)
                    .WithOne(c => c.Account)
                    .HasForeignKey(c => c.AccountId)
                    .HasPrincipalKey(c => c.AccountId);

                u.HasMany(c => c.Invites)
                    .WithOne(c => c.Account)
                    .HasForeignKey(c => c.AccountId)
                    .HasPrincipalKey(c => c.AccountId);

                u.HasMany(c => c.Issues)
                    .WithOne(c => c.Account)
                    .HasForeignKey(c => c.CreatedBy)
                    .HasPrincipalKey(c => c.AccountId);

                u.HasMany(c => c.Issues)
                    .WithOne(c => c.Account)
                    .HasForeignKey(c => c.AssignedTo)
                    .HasPrincipalKey(c => c.AccountId);

                u.HasMany(c => c.Issues)
                    .WithOne(c => c.Account)
                    .HasForeignKey(c => c.CreatedBy)
                    .HasPrincipalKey(c => c.AccountId);

                u.HasMany(c => c.AccountAccessLocations)
                    .WithOne(c => c.Account)
                    .HasForeignKey(c => c.AccountId)
                    .HasPrincipalKey(c => c.AccountId);
            });
            #endregion

            #region AccountAccessLocation
            builder.Entity<AccountAccessLocation>(u =>
            {
                u.Property(c => c.AccountAccessLocationId).ValueGeneratedNever();

                u.HasKey(c => c.AccountAccessLocationId);
            });
            #endregion

            #region AccountRoleDesc
            builder.Entity<AccountRoleDesc>(u =>
            {
                u.Property(c => c.AccountRoleDescId).ValueGeneratedNever();

                u.HasMany(c => c.Accounts)
                    .WithOne(c => c.Role)
                    .HasForeignKey(c => c.RoleId)
                    .HasPrincipalKey(c => c.AccountRoleDescId);

                u.HasData(
                    new AccountRoleDesc() { AccountRoleDescId = RoleType.Administrator, Description = "Administrator" },
                    new AccountRoleDesc() { AccountRoleDescId = RoleType.ProjectManager, Description = "Project Manager" },
                    new AccountRoleDesc() { AccountRoleDescId = RoleType.TeamMember, Description = "Team Member" },
                    new AccountRoleDesc() { AccountRoleDescId = RoleType.Observer, Description = "Observer" },
                    new AccountRoleDesc() { AccountRoleDescId = RoleType.Customer, Description = "Customer" }
                    );
            });
            #endregion

            #region AccountWorkspace
            builder.Entity<AccountWorkspace>(u =>
            {
                u.Property(c => c.AccountWorkspaceId).ValueGeneratedNever();

                u.HasOne(c => c.Workspace)
                    .WithMany(c => c.AccountWorkspaces)
                    .HasForeignKey(c => c.WorkspaceId)
                    .HasPrincipalKey(c => c.WorkspaceId)
                    .OnDelete(DeleteBehavior.NoAction);

                u.HasOne(c => c.RoleDesc)
                    .WithMany(c => c.AccountsWorkspaces)
                    .HasForeignKey(c => c.RoleId)
                    .HasPrincipalKey(c => c.AccountRoleDescId)
                    .OnDelete(DeleteBehavior.NoAction);

                u.HasKey(c => c.AccountWorkspaceId);
            });
            #endregion

            #region AccountLoggedWork
            builder.Entity<AccountLoggedWork>(u =>
            {
                u.Property(c => c.AccountLoggedWorkId).ValueGeneratedNever();
                u.Property(c => c.TimeSpent).HasPrecision(18, 2);

                u.HasKey(c => c.AccountLoggedWorkId);
            });
            #endregion

            #region Company
            builder.Entity<Company>(u =>
            {
                u.Property(c => c.CompanyId).ValueGeneratedNever();

                u.HasMany(c => c.Accounts)
                    .WithOne(c => c.Company)
                    .HasForeignKey(c => c.CompanyId)
                    .HasPrincipalKey(c => c.CompanyId);

                u.HasMany(c => c.Workspaces)
                    .WithOne(c => c.Company)
                    .HasForeignKey(c => c.CompanyId)
                    .HasPrincipalKey(c => c.CompanyId);
            });
            #endregion

            #region Invite
            builder.Entity<Invite>(u =>
            {
                u.HasKey(c => c.InviteId);
            });
            #endregion

            #region Issue
            builder.Entity<Issue>(u =>
            {
                u.Property(c => c.IssueId).ValueGeneratedNever();
                u.Property(c => c.EstimatedTime).HasPrecision(18, 2);

                u.HasOne(c => c.IssueResolution)
                    .WithOne(c => c.Issue)
                    .HasForeignKey<Issue>(c => c.IssueResolutionId)
                    .HasPrincipalKey<IssueResolution>(c => c.IssueResolutionId)
                    .IsRequired(false);

                u.HasKey(c => c.IssueId);
            });
            #endregion

            #region IssueComment
            builder.Entity<IssueComment>(u =>
            {
                u.Property(c => c.IssueCommentId).ValueGeneratedNever();

                u.HasOne(c => c.Issue)
                    .WithMany(c => c.IssueComments)
                    .HasForeignKey(c => c.IssueId)
                    .HasPrincipalKey(c => c.IssueId)
                    .OnDelete(DeleteBehavior.NoAction);

                u.HasOne(c => c.Account)
                    .WithMany(c => c.IssueComments)
                    .HasForeignKey(c => c.AccountId)
                    .HasPrincipalKey(c => c.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            #endregion

            #region IssueFile
            builder.Entity<IssueFile>(u =>
            {
                u.Property(c => c.IssueFileId).ValueGeneratedNever();

                u.HasOne(c => c.Issue)
                    .WithMany(c => c.IssueFiles)
                    .HasForeignKey(c => c.IssueId)
                    .HasPrincipalKey(c => c.IssueId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            #endregion

            #region IssuePriorityDesc
            builder.Entity<IssuePriorityDesc>(u =>
            {
                u.Property(c => c.IssuePriorityId).ValueGeneratedNever();

                u.HasMany(c => c.Issues)
                    .WithOne(c => c.IssuePriorityDesc)
                    .HasForeignKey(c => c.Priority)
                    .HasPrincipalKey(c => c.IssuePriorityId);

                u.HasData(
                        new IssuePriorityDesc { IssuePriorityId = PriorityType.Minor, Description = "Minor" },
                        new IssuePriorityDesc { IssuePriorityId = PriorityType.Moderate, Description = "Moderate" },
                        new IssuePriorityDesc { IssuePriorityId = PriorityType.Critical, Description = "Critical" },
                        new IssuePriorityDesc { IssuePriorityId = PriorityType.Major, Description = "Major" }
                        );
            });
            #endregion

            #region IssueResolution
            builder.Entity<IssueResolution>(u =>
            {
                u.HasKey(c => c.IssueResolutionId);
            });
            #endregion

            #region IssueResolutionDesc
            builder.Entity<IssueResolutionDesc>(u =>
            {
                u.Property(c => c.IssueResolutionId).ValueGeneratedNever();

                u.HasMany(c => c.IssueResolutions)
                    .WithOne(c => c.IssueResolutionDesc)
                    .HasForeignKey(c => c.Type)
                    .HasPrincipalKey(c => c.IssueResolutionId);

                u.HasData(
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.Completed, Description = "Completed" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.CannotReproduce, Description = "Cannot Reproduce" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.Duplicate, Description = "Duplicate" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.OnHold, Description = "On Hold" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.Invalid, Description = "Invalid" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.NoDefect, Description = "No Defect" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.ExternalDefect, Description = "External Defect" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.NoMaintenanceAgreement, Description = "No Maintenance Agreement" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.OfferDeclined, Description = "Offer Declined" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.OfferExpired, Description = "Offer Expired" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.Rejected, Description = "Rejected" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.Verified, Description = "Verified" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.Resolved, Description = "Resolved" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.Done, Description = "Done" },
                        new IssueResolutionDesc { IssueResolutionId = ResolutionType.Unresolved, Description = "Unresolved" }
                        );
            });
            #endregion

            #region IssueStatusDesc
            builder.Entity<IssueStatusDesc>(u =>
            {
                u.Property(c => c.IssueStatusId).ValueGeneratedNever();

                u.HasMany(c => c.Issues)
                    .WithOne(c => c.IssueStatusDesc)
                    .HasForeignKey(c => c.Status)
                    .HasPrincipalKey(c => c.IssueStatusId);

                u.HasData(
                        new IssueStatusDesc { IssueStatusId = StatusType.Open, Description = "Open" },
                        new IssueStatusDesc { IssueStatusId = StatusType.InProgress, Description = "In Progress" },
                        new IssueStatusDesc { IssueStatusId = StatusType.Closed, Description = "Closed" }
                        );
            });
            #endregion

            #region IssueTypeDesc
            builder.Entity<IssueTypeDesc>(u =>
            {
                u.Property(c => c.IssueTypeId).ValueGeneratedNever();

                u.HasMany(c => c.Issues)
                    .WithOne(c => c.IssueTypeDesc)
                    .HasForeignKey(c => c.Type)
                    .HasPrincipalKey(c => c.IssueTypeId);

                u.HasData(
                       new IssueTypeDesc { IssueTypeId = IssueType.Clarification, Description = "Clarification" },
                       new IssueTypeDesc { IssueTypeId = IssueType.DefectRemoval, Description = "Defect Removal" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Design, Description = "Design" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Deployment, Description = "Deployment" },
                       new IssueTypeDesc { IssueTypeId = IssueType.DesignReview, Description = "Design Review" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Documentation, Description = "Documentation" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Estimation, Description = "Estimation" },
                       new IssueTypeDesc { IssueTypeId = IssueType.ExpertTask, Description = "Expert Task" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Integration, Description = "Integration" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Monitoring, Description = "Monitoring" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Planning, Description = "Planning" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Reproduction, Description = "Reproduction" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Review, Description = "Review" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Testing, Description = "Testing" },
                       new IssueTypeDesc { IssueTypeId = IssueType.TestDesign, Description = "Test Design" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Verification, Description = "Verification" },
                       new IssueTypeDesc { IssueTypeId = IssueType.WorkLog, Description = "Work Log" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Invoice, Description = "Invoice" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Approval, Description = "Approval" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Coordination, Description = "Coordination" },
                       new IssueTypeDesc { IssueTypeId = IssueType.SolutionDesign, Description = "Solution Design" },
                       new IssueTypeDesc { IssueTypeId = IssueType.SolutionDesignReview, Description = "Solution Design Review" },
                       new IssueTypeDesc { IssueTypeId = IssueType.Implementation, Description = "Implementation" }
                       );
            });
            #endregion

            #region WorkspaceValuePerHour
            builder.Entity<WorkspaceValuePerHour>(u =>
            {
                u.Property(c => c.IssueType).ValueGeneratedNever();
                u.Property(c => c.HourlyRate).HasPrecision(18, 2);
            });
            #endregion

            #region Workspace
            builder.Entity<Workspace>(u =>
            {
                u.Property(c => c.WorkspaceId).ValueGeneratedNever();

                u.HasMany(c => c.WorkspaceHourlyRates)
                    .WithOne(c => c.Workspace)
                    .HasForeignKey(c => c.WorkspaceId)
                    .HasPrincipalKey(c => c.WorkspaceId);
            });
            #endregion
        }

        public DbSet<Account> Account { get; set; }
        public DbSet<AccountAccessLocation> AccountAccessLocation { get; set;}
        public DbSet<AccountLoggedWork> AccountLoggedWork { get; set; }
        public DbSet<AccountRoleDesc> AccountRoleDesc { get; set; }
        public DbSet<AccountWorkspace> AccountWorkspace { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Invite> Invite { get; set; }
        public DbSet<Issue> Issue { get; set; }
        public DbSet<IssueComment> IssueComment { get; set; }
        public DbSet<IssueFile> IssueFile { get; set; }
        public DbSet<IssuePriorityDesc> IssuePriorityDesc { get; set; }
        public DbSet<IssueResolution> IssueResolution { get; set; }
        public DbSet<IssueResolutionDesc> IssueResolutionDesc { get; set; }
        public DbSet<IssueStatusDesc> IssueStatusDesc { get; set; }
        public DbSet<IssueTypeDesc> IssueTypeDesc { get; set; }
        public DbSet<Workspace> Workspace { get; set; }
        public DbSet<WorkspaceValuePerHour> WorkspaceValuePerHour { get;set; }
    }
}
