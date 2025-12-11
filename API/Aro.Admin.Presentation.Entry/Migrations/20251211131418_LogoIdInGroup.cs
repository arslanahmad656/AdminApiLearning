using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class LogoIdInGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Groups");

            migrationBuilder.AddColumn<Guid>(
                name: "IconId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "GroupFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupFiles_FileResources_FileId",
                        column: x => x.FileId,
                        principalTable: "FileResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_IconId",
                table: "Groups",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupFiles_FileId",
                table: "GroupFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupFiles_GroupId_FileId",
                table: "GroupFiles",
                columns: new[] { "GroupId", "FileId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_FileResources_IconId",
                table: "Groups",
                column: "IconId",
                principalTable: "FileResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_FileResources_IconId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "GroupFiles");

            migrationBuilder.DropIndex(
                name: "IX_Groups_IconId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IconId",
                table: "Groups");

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "Groups",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
