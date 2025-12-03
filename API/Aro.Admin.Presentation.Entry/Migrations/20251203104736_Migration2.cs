using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Contacts_ContactId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Contacts_ContactId1",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ContactId1",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ContactId1",
                table: "Groups");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryContactId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Groups_PrimaryContactId",
                table: "Groups",
                column: "PrimaryContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_ContactInfos_PrimaryContactId",
                table: "Groups",
                column: "PrimaryContactId",
                principalTable: "ContactInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Contacts_ContactId",
                table: "Groups",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_ContactInfos_PrimaryContactId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Contacts_ContactId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_PrimaryContactId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "PrimaryContactId",
                table: "Groups");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId1",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ContactId1",
                table: "Groups",
                column: "ContactId1");

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
        }
    }
}
