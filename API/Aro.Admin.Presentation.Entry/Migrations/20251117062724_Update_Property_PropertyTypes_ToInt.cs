using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class Update_Property_PropertyTypes_ToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PropertyTypes",
                table: "Properties",
                type: "int",
                nullable: false,
                comment: "Flags enum stored as int for multiple property types",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "JSON array of PropertyType enum values");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PropertyTypes",
                table: "Properties",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "JSON array of PropertyType enum values",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Flags enum stored as int for multiple property types");
        }
    }
}
