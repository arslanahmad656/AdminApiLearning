using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class Migration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_ContactInfos_ContactInfoId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "ContactInfoId",
                table: "Groups",
                newName: "PrimaryContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_ContactInfoId",
                table: "Groups",
                newName: "IX_Groups_PrimaryContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_PrimaryContactId",
                table: "Groups",
                column: "PrimaryContactId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_PrimaryContactId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "PrimaryContactId",
                table: "Groups",
                newName: "ContactInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_PrimaryContactId",
                table: "Groups",
                newName: "IX_Groups_ContactInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_ContactInfos_ContactInfoId",
                table: "Groups",
                column: "ContactInfoId",
                principalTable: "ContactInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
