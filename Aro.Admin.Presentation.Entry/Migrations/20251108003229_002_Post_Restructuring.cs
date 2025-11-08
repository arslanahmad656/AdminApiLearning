using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class _002_Post_Restructuring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditTrails_Users_ActorUserId",
                table: "AuditTrails");

            migrationBuilder.DropIndex(
                name: "IX_AuditTrails_ActorUserId",
                table: "AuditTrails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_ActorUserId",
                table: "AuditTrails",
                column: "ActorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditTrails_Users_ActorUserId",
                table: "AuditTrails",
                column: "ActorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
