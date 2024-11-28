using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_DeletedGroupRolePermissionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupRoleGroupRolePermission");

            migrationBuilder.DropTable(
                name: "GroupRolePermissions");

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

            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                table: "GroupRoles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "GroupRoles");

            migrationBuilder.CreateTable(
                name: "GroupRolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Permission = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRolePermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupRoleGroupRolePermission",
                columns: table => new
                {
                    GroupRolesId = table.Column<int>(type: "INTEGER", nullable: false),
                    PermissionsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoleGroupRolePermission", x => new { x.GroupRolesId, x.PermissionsId });
                    table.ForeignKey(
                        name: "FK_GroupRoleGroupRolePermission_GroupRolePermissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "GroupRolePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRoleGroupRolePermission_GroupRoles_GroupRolesId",
                        column: x => x.GroupRolesId,
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
                name: "IX_GroupRoleGroupRolePermission_PermissionsId",
                table: "GroupRoleGroupRolePermission",
                column: "PermissionsId");
        }
    }
}
