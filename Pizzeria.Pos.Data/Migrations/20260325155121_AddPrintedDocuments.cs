using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Pos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrintedDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintedDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    DocumentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PrinterName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    JobName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ContentPreview = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintedDocuments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintedDocuments");
        }
    }
}
