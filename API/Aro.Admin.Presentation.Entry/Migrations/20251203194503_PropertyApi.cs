using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class PropertyApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Contacts_ContactId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_ContactId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_ContactId1",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ContactId1",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ContactId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ContactId1",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Groups");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactId",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeySellingPoints",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketingDescription",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketingTitle",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PropertyFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyFiles_FileResources_FileId",
                        column: x => x.FileId,
                        principalTable: "FileResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyFiles_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFiles_FileId",
                table: "PropertyFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFiles_PropertyId_FileId",
                table: "PropertyFiles",
                columns: new[] { "PropertyId", "FileId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Users_ContactId",
                table: "Properties",
                column: "ContactId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Users_ContactId",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "PropertyFiles");

            migrationBuilder.DropColumn(
                name: "KeySellingPoints",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MarketingDescription",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MarketingTitle",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Addresses");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactId",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId1",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ContactId1",
                table: "Properties",
                column: "ContactId1");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ContactId",
                table: "Groups",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Contacts_ContactId",
                table: "Groups",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Contacts_ContactId",
                table: "Properties",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Contacts_ContactId1",
                table: "Properties",
                column: "ContactId1",
                principalTable: "Contacts",
                principalColumn: "Id");
        }
    }
}
