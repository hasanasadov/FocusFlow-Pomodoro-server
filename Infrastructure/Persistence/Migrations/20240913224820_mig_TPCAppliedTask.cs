using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_TPCAppliedTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserTasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTaskId",
                table: "TaskSteps",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Label = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FinishDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserProjectTask",
                columns: table => new
                {
                    AssignedProjectTasksId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedUsersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserProjectTask", x => new { x.AssignedProjectTasksId, x.AssignedUsersId });
                    table.ForeignKey(
                        name: "FK_AppUserProjectTask_AspNetUsers_AssignedUsersId",
                        column: x => x.AssignedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserProjectTask_ProjectTask_AssignedProjectTasksId",
                        column: x => x.AssignedProjectTasksId,
                        principalTable: "ProjectTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskSteps_ProjectTaskId",
                table: "TaskSteps",
                column: "ProjectTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserProjectTask_AssignedUsersId",
                table: "AppUserProjectTask",
                column: "AssignedUsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSteps_ProjectTask_ProjectTaskId",
                table: "TaskSteps",
                column: "ProjectTaskId",
                principalTable: "ProjectTask",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskSteps_ProjectTask_ProjectTaskId",
                table: "TaskSteps");

            migrationBuilder.DropTable(
                name: "AppUserProjectTask");

            migrationBuilder.DropTable(
                name: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_TaskSteps_ProjectTaskId",
                table: "TaskSteps");

            migrationBuilder.DropColumn(
                name: "ProjectTaskId",
                table: "TaskSteps");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserTasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
