using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_UpdatedMembershipUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupMemberships_UserId_GroupId_RoleId",
                table: "GroupMemberships");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberships_UserId_GroupId",
                table: "GroupMemberships",
                columns: new[] { "UserId", "GroupId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupMemberships_UserId_GroupId",
                table: "GroupMemberships");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberships_UserId_GroupId_RoleId",
                table: "GroupMemberships",
                columns: new[] { "UserId", "GroupId", "RoleId" },
                unique: true);
        }
    }
}
