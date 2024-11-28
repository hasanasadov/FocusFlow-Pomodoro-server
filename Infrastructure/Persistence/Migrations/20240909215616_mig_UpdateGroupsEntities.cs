using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_UpdateGroupsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberships_GroupRoles_RoleId",
                table: "GroupMemberships");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "GroupMemberships",
                newName: "GroupRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMemberships_RoleId",
                table: "GroupMemberships",
                newName: "IX_GroupMemberships_GroupRoleId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupRoles",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "GroupRoles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "GroupRoles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupRoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Permission = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_GroupRoles_GroupRoleId",
                        column: x => x.GroupRoleId,
                        principalTable: "GroupRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GroupRoles",
                columns: new[] { "Id", "GroupId", "IsDefault", "Name" },
                values: new object[,]
                {
                    { 1, null, true, "Admin" },
                    { 2, null, true, "Manager" },
                    { 3, null, true, "Member" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_GroupId",
                table: "GroupRoles",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_GroupRoleId",
                table: "RolePermission",
                column: "GroupRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberships_GroupRoles_GroupRoleId",
                table: "GroupMemberships",
                column: "GroupRoleId",
                principalTable: "GroupRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupRoles_Groups_GroupId",
                table: "GroupRoles",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberships_GroupRoles_GroupRoleId",
                table: "GroupMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupRoles_Groups_GroupId",
                table: "GroupRoles");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropIndex(
                name: "IX_GroupRoles_GroupId",
                table: "GroupRoles");

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "GroupRoles");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "GroupRoles");

            migrationBuilder.RenameColumn(
                name: "GroupRoleId",
                table: "GroupMemberships",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMemberships_GroupRoleId",
                table: "GroupMemberships",
                newName: "IX_GroupMemberships_RoleId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupRoles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberships_GroupRoles_RoleId",
                table: "GroupMemberships",
                column: "RoleId",
                principalTable: "GroupRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
