using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pizzeria.Pos.Data.Migrations
{
    /// <inheritdoc />
    public partial class PizzaConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PizzaAddonDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    GroupName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaAddonDefinitions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PizzaAddonDefinitions",
                columns: new[] { "Id", "GroupName", "IsActive", "Name", "Price", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Serowe", true, "Extra ser", 6m, 1 },
                    { 2, "Mięsne", true, "Szynka", 7m, 1 },
                    { 3, "Sosy", true, "Dodatkowy sos", 4m, 1 },
                    { 4, "Warzywne", true, "Pieczarki", 5m, 1 },
                    { 5, "Warzywne", true, "Papryka", 4m, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PizzaAddonDefinitions");
        }
    }
}
