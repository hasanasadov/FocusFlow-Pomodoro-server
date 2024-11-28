using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_DeletedRequiredKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMemberships",
                table: "GroupMemberships");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "GroupMemberships",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMemberships",
                table: "GroupMemberships",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberships_RoleId",
                table: "GroupMemberships",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMemberships",
                table: "GroupMemberships");

            migrationBuilder.DropIndex(
                name: "IX_GroupMemberships_RoleId",
                table: "GroupMemberships");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "GroupMemberships",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMemberships",
                table: "GroupMemberships",
                column: "RoleId");
        }
    }
}
