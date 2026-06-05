using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gateway.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelsWithDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MeanAbsoluteDeviation",
                table: "AuditLogs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PeakToAverageRatio",
                table: "AuditLogs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StandardDeviation",
                table: "AuditLogs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeanAbsoluteDeviation",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "PeakToAverageRatio",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "StandardDeviation",
                table: "AuditLogs");
        }
    }
}
