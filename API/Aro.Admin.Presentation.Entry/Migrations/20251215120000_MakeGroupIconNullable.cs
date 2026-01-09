using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class MakeGroupIconNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop default constraint on IconId if it exists so we can alter nullability safely.
            migrationBuilder.Sql(
                """
                DECLARE @df NVARCHAR(128);
                SELECT @df = d.name
                FROM sys.default_constraints d
                JOIN sys.columns c
                  ON d.parent_object_id = c.object_id
                 AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID(N'[Groups]')
                  AND c.name = N'IconId';
                IF @df IS NOT NULL
                    EXEC('ALTER TABLE [Groups] DROP CONSTRAINT [' + @df + ']');
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_FileResources_IconId",
                table: "Groups");

            migrationBuilder.AlterColumn<Guid>(
                name: "IconId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "IconId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_FileResources_IconId",
                table: "Groups",
                column: "IconId",
                principalTable: "FileResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

