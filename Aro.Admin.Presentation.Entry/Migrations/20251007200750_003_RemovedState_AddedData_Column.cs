using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aro.Admin.Presentation.Entry.Migrations
{
    /// <inheritdoc />
    public partial class _003_RemovedState_AddedData_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "After",
                table: "AuditTrails");

            // from the following line, we are doing it manually.
            //migrationBuilder.RenameColumn(
            //    name: "Before",
            //    table: "AuditTrails",
            //    newName: "Data");

            migrationBuilder.DropColumn(
                name: "Before",
                table: "AuditTrails");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "Data",
            //    table: "AuditTrails",
            //    newName: "Before");

            migrationBuilder.AddColumn<string>(
                name: "Before",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "After",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "Data",
                table: "AuditTrails");
        }
    }
}
