using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_MtMGroupRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupRolePermissions_GroupRoles_GroupRoleId",
                table: "GroupRolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_GroupRolePermissions_GroupRoleId",
                table: "GroupRolePermissions");

            migrationBuilder.DropColumn(
                name: "GroupRoleId",
                table: "GroupRolePermissions");

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

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoleGroupRolePermission_PermissionsId",
                table: "GroupRoleGroupRolePermission",
                column: "PermissionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupRoleGroupRolePermission");

            migrationBuilder.AddColumn<int>(
                name: "GroupRoleId",
                table: "GroupRolePermissions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupRolePermissions_GroupRoleId",
                table: "GroupRolePermissions",
                column: "GroupRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupRolePermissions_GroupRoles_GroupRoleId",
                table: "GroupRolePermissions",
                column: "GroupRoleId",
                principalTable: "GroupRoles",
                principalColumn: "Id");
        }
    }
}
