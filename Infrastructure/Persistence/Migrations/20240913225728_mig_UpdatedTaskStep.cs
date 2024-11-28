using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_UpdatedTaskStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskSteps_ProjectTask_ProjectTaskId",
                table: "TaskSteps");

            migrationBuilder.DropIndex(
                name: "IX_TaskSteps_ProjectTaskId",
                table: "TaskSteps");

            migrationBuilder.DropColumn(
                name: "ProjectTaskId",
                table: "TaskSteps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectTaskId",
                table: "TaskSteps",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskSteps_ProjectTaskId",
                table: "TaskSteps",
                column: "ProjectTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSteps_ProjectTask_ProjectTaskId",
                table: "TaskSteps",
                column: "ProjectTaskId",
                principalTable: "ProjectTask",
                principalColumn: "Id");
        }
    }
}
