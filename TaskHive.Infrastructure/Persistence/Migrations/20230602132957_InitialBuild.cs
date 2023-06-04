using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskHive.Infrastructure.Persistence.Migrations
{
    public partial class InitialBuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountRoleDesc",
                columns: table => new
                {
                    AccountRoleDescId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRoleDesc", x => x.AccountRoleDescId);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "IssuePriorityDesc",
                columns: table => new
                {
                    IssuePriorityId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuePriorityDesc", x => x.IssuePriorityId);
                });

            migrationBuilder.CreateTable(
                name: "IssueResolutionDesc",
                columns: table => new
                {
                    IssueResolutionId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueResolutionDesc", x => x.IssueResolutionId);
                });

            migrationBuilder.CreateTable(
                name: "IssueStatusDesc",
                columns: table => new
                {
                    IssueStatusId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueStatusDesc", x => x.IssueStatusId);
                });

            migrationBuilder.CreateTable(
                name: "IssueTypeDesc",
                columns: table => new
                {
                    IssueTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueTypeDesc", x => x.IssueTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountState = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignUpType = table.Column<int>(type: "int", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Premium = table.Column<bool>(type: "bit", nullable: false),
                    PremiumExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RenewalReminderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Account_AccountRoleDesc_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AccountRoleDesc",
                        principalColumn: "AccountRoleDescId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Account_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workspace",
                columns: table => new
                {
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HasHourlyRate = table.Column<bool>(type: "bit", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace", x => x.WorkspaceId);
                    table.ForeignKey(
                        name: "FK_Workspace_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueResolution",
                columns: table => new
                {
                    IssueResolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolverAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueResolution", x => x.IssueResolutionId);
                    table.ForeignKey(
                        name: "FK_IssueResolution_IssueResolutionDesc_Type",
                        column: x => x.Type,
                        principalTable: "IssueResolutionDesc",
                        principalColumn: "IssueResolutionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountAccessLocation",
                columns: table => new
                {
                    AccountAccessLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAccessLocation", x => x.AccountAccessLocationId);
                    table.ForeignKey(
                        name: "FK_AccountAccessLocation_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountLoggedWork",
                columns: table => new
                {
                    AccountLoggedWorkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSpent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLoggedWork", x => x.AccountLoggedWorkId);
                    table.ForeignKey(
                        name: "FK_AccountLoggedWork_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invite",
                columns: table => new
                {
                    InviteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    InvitedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invite", x => x.InviteId);
                    table.ForeignKey(
                        name: "FK_Invite_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountWorkspace",
                columns: table => new
                {
                    AccountWorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountWorkspace", x => x.AccountWorkspaceId);
                    table.ForeignKey(
                        name: "FK_AccountWorkspace_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountWorkspace_AccountRoleDesc_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AccountRoleDesc",
                        principalColumn: "AccountRoleDescId");
                    table.ForeignKey(
                        name: "FK_AccountWorkspace_Workspace_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspace",
                        principalColumn: "WorkspaceId");
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceValuePerHour",
                columns: table => new
                {
                    WorkspaceValuePerHourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueType = table.Column<int>(type: "int", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceValuePerHour", x => x.WorkspaceValuePerHourId);
                    table.ForeignKey(
                        name: "FK_WorkspaceValuePerHour_Workspace_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspace",
                        principalColumn: "WorkspaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Issue",
                columns: table => new
                {
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentAssignee = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedDelivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedTime = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IssueResolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issue", x => x.IssueId);
                    table.ForeignKey(
                        name: "FK_Issue_Account_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Issue_IssuePriorityDesc_Priority",
                        column: x => x.Priority,
                        principalTable: "IssuePriorityDesc",
                        principalColumn: "IssuePriorityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Issue_IssueResolution_IssueResolutionId",
                        column: x => x.IssueResolutionId,
                        principalTable: "IssueResolution",
                        principalColumn: "IssueResolutionId");
                    table.ForeignKey(
                        name: "FK_Issue_IssueStatusDesc_Status",
                        column: x => x.Status,
                        principalTable: "IssueStatusDesc",
                        principalColumn: "IssueStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Issue_IssueTypeDesc_Type",
                        column: x => x.Type,
                        principalTable: "IssueTypeDesc",
                        principalColumn: "IssueTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueComment",
                columns: table => new
                {
                    IssueCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueComment", x => x.IssueCommentId);
                    table.ForeignKey(
                        name: "FK_IssueComment_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_IssueComment_Issue_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issue",
                        principalColumn: "IssueId");
                });

            migrationBuilder.CreateTable(
                name: "IssueFile",
                columns: table => new
                {
                    IssueFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileFriendlyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FileStoredName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueFile", x => x.IssueFileId);
                    table.ForeignKey(
                        name: "FK_IssueFile_Issue_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issue",
                        principalColumn: "IssueId");
                });

            migrationBuilder.CreateTable(
                name: "IssueHistory",
                columns: table => new
                {
                    IssueHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueHistory", x => x.IssueHistoryId);
                    table.ForeignKey(
                        name: "FK_IssueHistory_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_IssueHistory_Issue_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issue",
                        principalColumn: "IssueId");
                });

            migrationBuilder.InsertData(
                table: "AccountRoleDesc",
                columns: new[] { "AccountRoleDescId", "Description" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "Project Manager" },
                    { 3, "Team Member" },
                    { 4, "Observer" },
                    { 5, "Customer" }
                });

            migrationBuilder.InsertData(
                table: "IssuePriorityDesc",
                columns: new[] { "IssuePriorityId", "Description" },
                values: new object[,]
                {
                    { 1, "Minor" },
                    { 2, "Moderate" },
                    { 3, "Critical" },
                    { 4, "Major" }
                });

            migrationBuilder.InsertData(
                table: "IssueResolutionDesc",
                columns: new[] { "IssueResolutionId", "Description" },
                values: new object[,]
                {
                    { 1, "Completed" },
                    { 2, "Cannot Reproduce" },
                    { 3, "Duplicate" },
                    { 4, "On Hold" },
                    { 5, "Invalid" },
                    { 6, "No Defect" },
                    { 7, "External Defect" },
                    { 8, "No Maintenance Agreement" },
                    { 9, "Offer Declined" },
                    { 10, "Offer Expired" },
                    { 11, "Rejected" },
                    { 12, "Verified" },
                    { 13, "Resolved" },
                    { 14, "Done" },
                    { 15, "Unresolved" }
                });

            migrationBuilder.InsertData(
                table: "IssueStatusDesc",
                columns: new[] { "IssueStatusId", "Description" },
                values: new object[,]
                {
                    { 1, "Open" },
                    { 2, "In Progress" },
                    { 3, "Closed" }
                });

            migrationBuilder.InsertData(
                table: "IssueTypeDesc",
                columns: new[] { "IssueTypeId", "Description" },
                values: new object[,]
                {
                    { 1, "Clarification" },
                    { 2, "Defect Removal" },
                    { 3, "Design" },
                    { 4, "Deployment" },
                    { 5, "Design Review" },
                    { 6, "Documentation" },
                    { 7, "Estimation" },
                    { 8, "Expert Task" },
                    { 9, "Integration" },
                    { 10, "Monitoring" },
                    { 11, "Planning" },
                    { 12, "Reproduction" },
                    { 13, "Review" },
                    { 14, "Testing" },
                    { 15, "Test Design" }
                });

            migrationBuilder.InsertData(
                table: "IssueTypeDesc",
                columns: new[] { "IssueTypeId", "Description" },
                values: new object[,]
                {
                    { 16, "Verification" },
                    { 17, "Work Log" },
                    { 18, "Invoice" },
                    { 19, "Approval" },
                    { 20, "Coordination" },
                    { 21, "Solution Design" },
                    { 22, "Solution Design Review" },
                    { 23, "Implementation" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_CompanyId",
                table: "Account",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleId",
                table: "Account",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAccessLocation_AccountId",
                table: "AccountAccessLocation",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLoggedWork_AccountId",
                table: "AccountLoggedWork",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountWorkspace_AccountId",
                table: "AccountWorkspace",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountWorkspace_RoleId",
                table: "AccountWorkspace",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountWorkspace_WorkspaceId",
                table: "AccountWorkspace",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invite_AccountId",
                table: "Invite",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_CreatedBy",
                table: "Issue",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_IssueResolutionId",
                table: "Issue",
                column: "IssueResolutionId",
                unique: true,
                filter: "[IssueResolutionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_Priority",
                table: "Issue",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_Status",
                table: "Issue",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_Type",
                table: "Issue",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_IssueComment_AccountId",
                table: "IssueComment",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueComment_IssueId",
                table: "IssueComment",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueFile_IssueId",
                table: "IssueFile",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueHistory_AccountId",
                table: "IssueHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueHistory_IssueId",
                table: "IssueHistory",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueResolution_Type",
                table: "IssueResolution",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_CompanyId",
                table: "Workspace",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceValuePerHour_WorkspaceId",
                table: "WorkspaceValuePerHour",
                column: "WorkspaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountAccessLocation");

            migrationBuilder.DropTable(
                name: "AccountLoggedWork");

            migrationBuilder.DropTable(
                name: "AccountWorkspace");

            migrationBuilder.DropTable(
                name: "Invite");

            migrationBuilder.DropTable(
                name: "IssueComment");

            migrationBuilder.DropTable(
                name: "IssueFile");

            migrationBuilder.DropTable(
                name: "IssueHistory");

            migrationBuilder.DropTable(
                name: "WorkspaceValuePerHour");

            migrationBuilder.DropTable(
                name: "Issue");

            migrationBuilder.DropTable(
                name: "Workspace");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "IssuePriorityDesc");

            migrationBuilder.DropTable(
                name: "IssueResolution");

            migrationBuilder.DropTable(
                name: "IssueStatusDesc");

            migrationBuilder.DropTable(
                name: "IssueTypeDesc");

            migrationBuilder.DropTable(
                name: "AccountRoleDesc");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "IssueResolutionDesc");
        }
    }
}
