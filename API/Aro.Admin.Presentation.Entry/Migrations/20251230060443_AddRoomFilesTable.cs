using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsThumbnail = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomFiles_FileResources_FileId",
                        column: x => x.FileId,
                        principalTable: "FileResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomFiles_RoomId_FileId",
                table: "RoomFiles",
                columns: new[] { "RoomId", "FileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomFiles_FileId",
                table: "RoomFiles",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomFiles");
        }
    }
}
