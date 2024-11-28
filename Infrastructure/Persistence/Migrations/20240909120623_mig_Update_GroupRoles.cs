using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_Update_GroupRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberships_GroupRole_RoleId",
                table: "GroupMemberships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupRole",
                table: "GroupRole");

            migrationBuilder.RenameTable(
                name: "GroupRole",
                newName: "GroupRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupRoles",
                table: "GroupRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberships_GroupRoles_RoleId",
                table: "GroupMemberships",
                column: "RoleId",
                principalTable: "GroupRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberships_GroupRoles_RoleId",
                table: "GroupMemberships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupRoles",
                table: "GroupRoles");

            migrationBuilder.RenameTable(
                name: "GroupRoles",
                newName: "GroupRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupRole",
                table: "GroupRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberships_GroupRole_RoleId",
                table: "GroupMemberships",
                column: "RoleId",
                principalTable: "GroupRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
