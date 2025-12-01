using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class NormalizedGroupsAndContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_PrimaryContactId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "PrimaryContactId",
                table: "Groups",
                newName: "ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_PrimaryContactId",
                table: "Groups",
                newName: "IX_Groups_ContactId");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId1",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId1",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId1",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId1",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId1",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

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
                name: "IX_Properties_AddressId",
                table: "Properties",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_AddressId1",
                table: "Properties",
                column: "AddressId1");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ContactId",
                table: "Properties",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ContactId1",
                table: "Properties",
                column: "ContactId1");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_GroupId1",
                table: "Properties",
                column: "GroupId1");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_AddressId",
                table: "Groups",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_AddressId1",
                table: "Groups",
                column: "AddressId1");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ContactId1",
                table: "Groups",
                column: "ContactId1");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Addresses_AddressId",
                table: "Groups",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Addresses_AddressId1",
                table: "Groups",
                column: "AddressId1",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Contacts_ContactId",
                table: "Groups",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Contacts_ContactId1",
                table: "Groups",
                column: "ContactId1",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Addresses_AddressId",
                table: "Properties",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Addresses_AddressId1",
                table: "Properties",
                column: "AddressId1",
                principalTable: "Addresses",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Groups_GroupId1",
                table: "Properties",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Addresses_AddressId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Addresses_AddressId1",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Contacts_ContactId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Contacts_ContactId1",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Addresses_AddressId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Addresses_AddressId1",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_ContactId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_ContactId1",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Groups_GroupId1",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Properties_AddressId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_AddressId1",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ContactId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ContactId1",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_GroupId1",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Groups_AddressId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_AddressId1",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ContactId1",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "AddressId1",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ContactId1",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "AddressId1",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ContactId1",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "Groups",
                newName: "PrimaryContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_ContactId",
                table: "Groups",
                newName: "IX_Groups_PrimaryContactId");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Groups",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Groups",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Groups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Groups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Groups",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_PrimaryContactId",
                table: "Groups",
                column: "PrimaryContactId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
