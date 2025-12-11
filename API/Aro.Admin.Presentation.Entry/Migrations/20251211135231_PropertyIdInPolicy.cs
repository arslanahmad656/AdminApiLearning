using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class PropertyIdInPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PropertyId",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PropertyId1",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_PropertyId",
                table: "Policies",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_PropertyId1",
                table: "Policies",
                column: "PropertyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Properties_PropertyId",
                table: "Policies",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Properties_PropertyId1",
                table: "Policies",
                column: "PropertyId1",
                principalTable: "Properties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Properties_PropertyId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Properties_PropertyId1",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_PropertyId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_PropertyId1",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "PropertyId1",
                table: "Policies");
        }
    }
}
