using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_ProjectTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserProjectTask_ProjectTask_AssignedProjectTasksId",
                table: "AppUserProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_Projects_ProjectId",
                table: "ProjectTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTask",
                table: "ProjectTask");

            migrationBuilder.RenameTable(
                name: "ProjectTask",
                newName: "ProjectTasks");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTask_ProjectId",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTasks",
                table: "ProjectTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserProjectTask_ProjectTasks_AssignedProjectTasksId",
                table: "AppUserProjectTask",
                column: "AssignedProjectTasksId",
                principalTable: "ProjectTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserProjectTask_ProjectTasks_AssignedProjectTasksId",
                table: "AppUserProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTasks",
                table: "ProjectTasks");

            migrationBuilder.RenameTable(
                name: "ProjectTasks",
                newName: "ProjectTask");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_ProjectId",
                table: "ProjectTask",
                newName: "IX_ProjectTask_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTask",
                table: "ProjectTask",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserProjectTask_ProjectTask_AssignedProjectTasksId",
                table: "AppUserProjectTask",
                column: "AssignedProjectTasksId",
                principalTable: "ProjectTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_Projects_ProjectId",
                table: "ProjectTask",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
