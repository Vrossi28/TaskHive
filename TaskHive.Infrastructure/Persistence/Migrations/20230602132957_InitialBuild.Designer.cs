﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskHive.Infrastructure.Persistence;

#nullable disable

namespace TaskHive.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(TaskHiveContext))]
    [Migration("20230602132957_InitialBuild")]
    partial class InitialBuild
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("TaskHive.Core.Entities.Account", b =>
                {
                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccountState")
                        .HasColumnType("int");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OriginCountry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Premium")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("PremiumExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RenewalReminderDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ResetTokenExpiration")
                        .HasColumnType("datetime2");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("SignUpType")
                        .HasColumnType("int");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("AccountId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("RoleId");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.AccountAccessLocation", b =>
                {
                    b.Property<Guid>("AccountAccessLocationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("AccessDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("City")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("AccountAccessLocationId");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountAccessLocation");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.AccountLoggedWork", b =>
                {
                    b.Property<Guid>("AccountLoggedWorkId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IssueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartingDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("TimeSpent")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("AccountLoggedWorkId");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountLoggedWork");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.AccountRoleDesc", b =>
                {
                    b.Property<int>("AccountRoleDescId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("AccountRoleDescId");

                    b.ToTable("AccountRoleDesc");

                    b.HasData(
                        new
                        {
                            AccountRoleDescId = 1,
                            Description = "Administrator"
                        },
                        new
                        {
                            AccountRoleDescId = 2,
                            Description = "Project Manager"
                        },
                        new
                        {
                            AccountRoleDescId = 3,
                            Description = "Team Member"
                        },
                        new
                        {
                            AccountRoleDescId = 4,
                            Description = "Observer"
                        },
                        new
                        {
                            AccountRoleDescId = 5,
                            Description = "Customer"
                        });
                });

            modelBuilder.Entity("TaskHive.Core.Entities.AccountWorkspace", b =>
                {
                    b.Property<Guid>("AccountWorkspaceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<Guid>("WorkspaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AccountWorkspaceId");

                    b.HasIndex("AccountId");

                    b.HasIndex("RoleId");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("AccountWorkspace");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Company", b =>
                {
                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CompanyState")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CompanyId");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Invite", b =>
                {
                    b.Property<Guid>("InviteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InvitedEmail")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<Guid>("WorkspaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("InviteId");

                    b.HasIndex("AccountId");

                    b.ToTable("Invite");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Issue", b =>
                {
                    b.Property<Guid>("IssueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssignedTo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CurrentAssignee")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EstimatedDelivery")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("EstimatedTime")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid?>("IssueResolutionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("WorkspaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IssueId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("IssueResolutionId")
                        .IsUnique()
                        .HasFilter("[IssueResolutionId] IS NOT NULL");

                    b.HasIndex("Priority");

                    b.HasIndex("Status");

                    b.HasIndex("Type");

                    b.ToTable("Issue");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueComment", b =>
                {
                    b.Property<Guid>("IssueCommentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IssueId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IssueCommentId");

                    b.HasIndex("AccountId");

                    b.HasIndex("IssueId");

                    b.ToTable("IssueComment");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueFile", b =>
                {
                    b.Property<Guid>("IssueFileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileFriendlyName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FileStoredName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<Guid>("IssueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UrlExpiryDate")
                        .HasColumnType("datetime2");

                    b.HasKey("IssueFileId");

                    b.HasIndex("IssueId");

                    b.ToTable("IssueFile");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueHistory", b =>
                {
                    b.Property<Guid>("IssueHistoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("IssueId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IssueHistoryId");

                    b.HasIndex("AccountId");

                    b.HasIndex("IssueId");

                    b.ToTable("IssueHistory");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssuePriorityDesc", b =>
                {
                    b.Property<int>("IssuePriorityId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IssuePriorityId");

                    b.ToTable("IssuePriorityDesc");

                    b.HasData(
                        new
                        {
                            IssuePriorityId = 1,
                            Description = "Minor"
                        },
                        new
                        {
                            IssuePriorityId = 2,
                            Description = "Moderate"
                        },
                        new
                        {
                            IssuePriorityId = 3,
                            Description = "Critical"
                        },
                        new
                        {
                            IssuePriorityId = 4,
                            Description = "Major"
                        });
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueResolution", b =>
                {
                    b.Property<Guid>("IssueResolutionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SolverAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("IssueResolutionId");

                    b.HasIndex("Type");

                    b.ToTable("IssueResolution");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueResolutionDesc", b =>
                {
                    b.Property<int>("IssueResolutionId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IssueResolutionId");

                    b.ToTable("IssueResolutionDesc");

                    b.HasData(
                        new
                        {
                            IssueResolutionId = 1,
                            Description = "Completed"
                        },
                        new
                        {
                            IssueResolutionId = 2,
                            Description = "Cannot Reproduce"
                        },
                        new
                        {
                            IssueResolutionId = 3,
                            Description = "Duplicate"
                        },
                        new
                        {
                            IssueResolutionId = 4,
                            Description = "On Hold"
                        },
                        new
                        {
                            IssueResolutionId = 5,
                            Description = "Invalid"
                        },
                        new
                        {
                            IssueResolutionId = 6,
                            Description = "No Defect"
                        },
                        new
                        {
                            IssueResolutionId = 7,
                            Description = "External Defect"
                        },
                        new
                        {
                            IssueResolutionId = 8,
                            Description = "No Maintenance Agreement"
                        },
                        new
                        {
                            IssueResolutionId = 9,
                            Description = "Offer Declined"
                        },
                        new
                        {
                            IssueResolutionId = 10,
                            Description = "Offer Expired"
                        },
                        new
                        {
                            IssueResolutionId = 11,
                            Description = "Rejected"
                        },
                        new
                        {
                            IssueResolutionId = 12,
                            Description = "Verified"
                        },
                        new
                        {
                            IssueResolutionId = 13,
                            Description = "Resolved"
                        },
                        new
                        {
                            IssueResolutionId = 14,
                            Description = "Done"
                        },
                        new
                        {
                            IssueResolutionId = 15,
                            Description = "Unresolved"
                        });
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueStatusDesc", b =>
                {
                    b.Property<int>("IssueStatusId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IssueStatusId");

                    b.ToTable("IssueStatusDesc");

                    b.HasData(
                        new
                        {
                            IssueStatusId = 1,
                            Description = "Open"
                        },
                        new
                        {
                            IssueStatusId = 2,
                            Description = "In Progress"
                        },
                        new
                        {
                            IssueStatusId = 3,
                            Description = "Closed"
                        });
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueTypeDesc", b =>
                {
                    b.Property<int>("IssueTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("IssueTypeId");

                    b.ToTable("IssueTypeDesc");

                    b.HasData(
                        new
                        {
                            IssueTypeId = 1,
                            Description = "Clarification"
                        },
                        new
                        {
                            IssueTypeId = 2,
                            Description = "Defect Removal"
                        },
                        new
                        {
                            IssueTypeId = 3,
                            Description = "Design"
                        },
                        new
                        {
                            IssueTypeId = 4,
                            Description = "Deployment"
                        },
                        new
                        {
                            IssueTypeId = 5,
                            Description = "Design Review"
                        },
                        new
                        {
                            IssueTypeId = 6,
                            Description = "Documentation"
                        },
                        new
                        {
                            IssueTypeId = 7,
                            Description = "Estimation"
                        },
                        new
                        {
                            IssueTypeId = 8,
                            Description = "Expert Task"
                        },
                        new
                        {
                            IssueTypeId = 9,
                            Description = "Integration"
                        },
                        new
                        {
                            IssueTypeId = 10,
                            Description = "Monitoring"
                        },
                        new
                        {
                            IssueTypeId = 11,
                            Description = "Planning"
                        },
                        new
                        {
                            IssueTypeId = 12,
                            Description = "Reproduction"
                        },
                        new
                        {
                            IssueTypeId = 13,
                            Description = "Review"
                        },
                        new
                        {
                            IssueTypeId = 14,
                            Description = "Testing"
                        },
                        new
                        {
                            IssueTypeId = 15,
                            Description = "Test Design"
                        },
                        new
                        {
                            IssueTypeId = 16,
                            Description = "Verification"
                        },
                        new
                        {
                            IssueTypeId = 17,
                            Description = "Work Log"
                        },
                        new
                        {
                            IssueTypeId = 18,
                            Description = "Invoice"
                        },
                        new
                        {
                            IssueTypeId = 19,
                            Description = "Approval"
                        },
                        new
                        {
                            IssueTypeId = 20,
                            Description = "Coordination"
                        },
                        new
                        {
                            IssueTypeId = 21,
                            Description = "Solution Design"
                        },
                        new
                        {
                            IssueTypeId = 22,
                            Description = "Solution Design Review"
                        },
                        new
                        {
                            IssueTypeId = 23,
                            Description = "Implementation"
                        });
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Workspace", b =>
                {
                    b.Property<Guid>("WorkspaceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Currency")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<bool>("HasHourlyRate")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("WorkspaceId");

                    b.HasIndex("CompanyId");

                    b.ToTable("Workspace");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.WorkspaceValuePerHour", b =>
                {
                    b.Property<Guid>("WorkspaceValuePerHourId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("HourlyRate")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("IssueType")
                        .HasColumnType("int");

                    b.Property<Guid>("WorkspaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("WorkspaceValuePerHourId");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("WorkspaceValuePerHour");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Account", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Company", "Company")
                        .WithMany("Accounts")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskHive.Core.Entities.AccountRoleDesc", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.AccountAccessLocation", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Account", "Account")
                        .WithMany("AccountAccessLocations")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.AccountLoggedWork", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Account", "Account")
                        .WithMany("AccountLoggedWorks")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.AccountWorkspace", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Account", "Account")
                        .WithMany("AccountWorkspaces")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskHive.Core.Entities.AccountRoleDesc", "RoleDesc")
                        .WithMany("AccountsWorkspaces")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TaskHive.Core.Entities.Workspace", "Workspace")
                        .WithMany("AccountWorkspaces")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("RoleDesc");

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Invite", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Account", "Account")
                        .WithMany("Invites")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Issue", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Account", "Account")
                        .WithMany("Issues")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskHive.Core.Entities.IssueResolution", "IssueResolution")
                        .WithOne("Issue")
                        .HasForeignKey("TaskHive.Core.Entities.Issue", "IssueResolutionId");

                    b.HasOne("TaskHive.Core.Entities.IssuePriorityDesc", "IssuePriorityDesc")
                        .WithMany("Issues")
                        .HasForeignKey("Priority")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskHive.Core.Entities.IssueStatusDesc", "IssueStatusDesc")
                        .WithMany("Issues")
                        .HasForeignKey("Status")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskHive.Core.Entities.IssueTypeDesc", "IssueTypeDesc")
                        .WithMany("Issues")
                        .HasForeignKey("Type")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("IssuePriorityDesc");

                    b.Navigation("IssueResolution");

                    b.Navigation("IssueStatusDesc");

                    b.Navigation("IssueTypeDesc");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueComment", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Account", "Account")
                        .WithMany("IssueComments")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TaskHive.Core.Entities.Issue", "Issue")
                        .WithMany("IssueComments")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueFile", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Issue", "Issue")
                        .WithMany("IssueFiles")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueHistory", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Account", "Account")
                        .WithMany("IssueHistory")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TaskHive.Core.Entities.Issue", "Issue")
                        .WithMany("IssueHistory")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueResolution", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.IssueResolutionDesc", "IssueResolutionDesc")
                        .WithMany("IssueResolutions")
                        .HasForeignKey("Type")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IssueResolutionDesc");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Workspace", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Company", "Company")
                        .WithMany("Workspaces")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.WorkspaceValuePerHour", b =>
                {
                    b.HasOne("TaskHive.Core.Entities.Workspace", "Workspace")
                        .WithMany("WorkspaceHourlyRates")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Account", b =>
                {
                    b.Navigation("AccountAccessLocations");

                    b.Navigation("AccountLoggedWorks");

                    b.Navigation("AccountWorkspaces");

                    b.Navigation("Invites");

                    b.Navigation("IssueComments");

                    b.Navigation("IssueHistory");

                    b.Navigation("Issues");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.AccountRoleDesc", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("AccountsWorkspaces");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Company", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("Workspaces");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Issue", b =>
                {
                    b.Navigation("IssueComments");

                    b.Navigation("IssueFiles");

                    b.Navigation("IssueHistory");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssuePriorityDesc", b =>
                {
                    b.Navigation("Issues");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueResolution", b =>
                {
                    b.Navigation("Issue")
                        .IsRequired();
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueResolutionDesc", b =>
                {
                    b.Navigation("IssueResolutions");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueStatusDesc", b =>
                {
                    b.Navigation("Issues");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.IssueTypeDesc", b =>
                {
                    b.Navigation("Issues");
                });

            modelBuilder.Entity("TaskHive.Core.Entities.Workspace", b =>
                {
                    b.Navigation("AccountWorkspaces");

                    b.Navigation("WorkspaceHourlyRates");
                });
#pragma warning restore 612, 618
        }
    }
}
