using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModerationService.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(36)", nullable: false),
                    reported_user_id = table.Column<string>(type: "char(36)", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    reported_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sanction",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(36)", nullable: false),
                    report_id = table.Column<string>(type: "char(36)", nullable: true),
                    user_id = table.Column<string>(type: "char(36)", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sanction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sanction_Report_report_id",
                        column: x => x.report_id,
                        principalTable: "Report",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sanction_report_id",
                table: "Sanction",
                column: "report_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sanction");

            migrationBuilder.DropTable(
                name: "Report");
        }
    }
}
