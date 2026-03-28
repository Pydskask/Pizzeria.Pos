using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pizzeria.Pos.Data.Migrations
{
    /// <inheritdoc />
    public partial class nowemenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Ser", "Ricotta", 8m, 5 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "SortOrder" },
                values: new object[] { "Szynka", 1 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Salami", 7m, 2 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Boczek", 8m, 3 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Kurczak z grilla", 9m, 4 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Mięsne", "Pepperoni", 8m, 5 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Mięsne", "Nduja", 9m, 6 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Mięsne", "Salsiccia", 9m, 7 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "SortOrder" },
                values: new object[] { "Pieczarki", 1 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Papryka", 4m, 2 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Cebula", 3m, 3 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Oliwki czarne", 5m, 4 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Warzywne", "Oliwki zielone", 5m, 5 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Warzywne", "Pomidory suszone", 6m, 6 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "GroupName", "Name", "SortOrder" },
                values: new object[] { "Warzywne", "Rukola", 7 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "GroupName", "Name", "SortOrder" },
                values: new object[] { "Warzywne", "Ananas", 8 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Warzywne", "Szpinak", 4m, 9 });

            migrationBuilder.InsertData(
                table: "PizzaAddonDefinitions",
                columns: new[] { "Id", "GroupName", "IsActive", "Name", "Price", "SortOrder" },
                values: new object[,]
                {
                    { 22, "Warzywne", true, "Karczoch", 6m, 10 },
                    { 23, "Sosy", true, "Sos pomidorowy", 4m, 1 },
                    { 24, "Sosy", true, "Sos czosnkowy", 4m, 2 },
                    { 25, "Sosy", true, "Sos BBQ", 4m, 3 },
                    { 26, "Sosy", true, "Sos ostry", 4m, 4 },
                    { 27, "Sosy", true, "Pesto", 5m, 5 },
                    { 28, "Sosy", true, "Sos śmietanowy", 4m, 6 }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Pizza", "Tonno", 39m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Pizza", "Romana", 36m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Pizza", "Calabrese", 41m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Pizza", "Truflowa", 48m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Napoje zimne", "Coca-Cola 0,33l", 7m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Coca-Cola 0,5l", 9m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Sprite 0,33l", 7m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "Fanta 0,33l");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Woda niegazowana", 5m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                column: "Name",
                value: "Woda gazowana");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Sok pomarańczowy", 8m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "Sok jabłkowy");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Lemońska domowa", 10m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Mięsne", "Szynka", 7m, 1 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "SortOrder" },
                values: new object[] { "Salami", 2 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Boczek", 8m, 3 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Kurczak z grilla", 9m, 4 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Pepperoni", 8m, 5 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Warzywne", "Pieczarki", 5m, 1 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Warzywne", "Papryka", 4m, 2 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Warzywne", "Cebula", 3m, 3 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "SortOrder" },
                values: new object[] { "Oliwki", 4 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Pomi dory suśzone", 6m, 5 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Rukola", 4m, 6 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "Price", "SortOrder" },
                values: new object[] { "Ananas", 4m, 7 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Sosy", "Dodatkowy sos pomidorowy", 4m, 1 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Sosy", "Sos czosnkowy", 4m, 2 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "GroupName", "Name", "SortOrder" },
                values: new object[] { "Sosy", "Sos BBQ", 3 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "GroupName", "Name", "SortOrder" },
                values: new object[] { "Sosy", "Sos ostry", 4 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Sosy", "Pesto", 5m, 5 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Przystawki", "Chleb czosnkowy", 12m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Przystawki", "Bruschetta", 14m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Przystawki", "Skrzydełka buffalo", 22m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Przystawki", "Krewetki w tempurze", 26m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Przystawki", "Zupa pomidorowa", 14m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Coca-Cola 0,33l", 7m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Coca-Cola 0,5l", 9m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "Sprite 0,33l");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Fanta 0,33l", 7m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                column: "Name",
                value: "Woda niegazowana");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Woda gazowana", 5m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "Sok pomarańczowy");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Piwo Peroni 0,5l", 12m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Name", "Price" },
                values: new object[,]
                {
                    { 28, "Desery", "Tiramisu", 16m },
                    { 29, "Desery", "Lody kulkowe", 12m },
                    { 30, "Desery", "Sernik", 14m }
                });
        }
    }
}
