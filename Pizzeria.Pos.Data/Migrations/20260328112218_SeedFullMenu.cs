using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pizzeria.Pos.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedFullMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GroupName",
                value: "Ser");

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GroupName", "Name", "SortOrder" },
                values: new object[] { "Ser", "Mozzarella", 2 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Ser", "Gorgonzola", 8m, 3 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Ser", "Parmezan", 7m, 4 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Mięsne", "Szynka", 7m, 1 });

            migrationBuilder.InsertData(
                table: "PizzaAddonDefinitions",
                columns: new[] { "Id", "GroupName", "IsActive", "Name", "Price", "SortOrder" },
                values: new object[,]
                {
                    { 6, "Mięsne", true, "Salami", 7m, 2 },
                    { 7, "Mięsne", true, "Boczek", 8m, 3 },
                    { 8, "Mięsne", true, "Kurczak z grilla", 9m, 4 },
                    { 9, "Mięsne", true, "Pepperoni", 8m, 5 },
                    { 10, "Warzywne", true, "Pieczarki", 5m, 1 },
                    { 11, "Warzywne", true, "Papryka", 4m, 2 },
                    { 12, "Warzywne", true, "Cebula", 3m, 3 },
                    { 13, "Warzywne", true, "Oliwki", 5m, 4 },
                    { 14, "Warzywne", true, "Pomi dory suśzone", 6m, 5 },
                    { 15, "Warzywne", true, "Rukola", 4m, 6 },
                    { 16, "Warzywne", true, "Ananas", 4m, 7 },
                    { 17, "Sosy", true, "Dodatkowy sos pomidorowy", 4m, 1 },
                    { 18, "Sosy", true, "Sos czosnkowy", 4m, 2 },
                    { 19, "Sosy", true, "Sos BBQ", 4m, 3 },
                    { 20, "Sosy", true, "Sos ostry", 4m, 4 },
                    { 21, "Sosy", true, "Pesto", 5m, 5 }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Margherita", 32m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Pizza", "Pepperoni", 38m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Name", "Price" },
                values: new object[,]
                {
                    { 3, "Pizza", "Quattro Formaggi", 42m },
                    { 4, "Pizza", "Prosciutto", 40m },
                    { 5, "Pizza", "Diavola", 39m },
                    { 6, "Pizza", "Hawajska", 37m },
                    { 7, "Pizza", "Capricciosa", 40m },
                    { 8, "Pizza", "Vege", 35m },
                    { 9, "Pizza", "BBQ Kurczak", 41m },
                    { 10, "Pizza", "Fungi", 36m },
                    { 11, "Przystawki", "Chleb czosnkowy", 12m },
                    { 12, "Przystawki", "Bruschetta", 14m },
                    { 13, "Przystawki", "Skrzydełka buffalo", 22m },
                    { 14, "Przystawki", "Krewetki w tempurze", 26m },
                    { 15, "Przystawki", "Zupa pomidorowa", 14m },
                    { 16, "Napoje zimne", "Coca-Cola 0,33l", 7m },
                    { 17, "Napoje zimne", "Coca-Cola 0,5l", 9m },
                    { 18, "Napoje zimne", "Sprite 0,33l", 7m },
                    { 19, "Napoje zimne", "Fanta 0,33l", 7m },
                    { 20, "Napoje zimne", "Woda niegazowana", 5m },
                    { 21, "Napoje zimne", "Woda gazowana", 5m },
                    { 22, "Napoje zimne", "Sok pomarańczowy", 8m },
                    { 23, "Napoje zimne", "Piwo Peroni 0,5l", 12m },
                    { 24, "Napoje ciepłe", "Kawa espresso", 8m },
                    { 25, "Napoje ciepłe", "Cappuccino", 10m },
                    { 26, "Napoje ciepłe", "Latte macchiato", 11m },
                    { 27, "Napoje ciepłe", "Herbata", 7m },
                    { 28, "Desery", "Tiramisu", 16m },
                    { 29, "Desery", "Lody kulkowe", 12m },
                    { 30, "Desery", "Sernik", 14m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsActive", "Name", "Pin", "Role" },
                values: new object[] { 3, true, "Kelner2", "2211", "Kelner" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

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

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GroupName",
                value: "Serowe");

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GroupName", "Name", "SortOrder" },
                values: new object[] { "Mięsne", "Szynka", 1 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Sosy", "Dodatkowy sos", 4m, 1 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Warzywne", "Pieczarki", 5m, 1 });

            migrationBuilder.UpdateData(
                table: "PizzaAddonDefinitions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GroupName", "Name", "Price", "SortOrder" },
                values: new object[] { "Warzywne", "Papryka", 4m, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Price" },
                values: new object[] { "Margherita 30cm", 35m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Napoje zimne", "Cola 0.5l", 8m });
        }
    }
}
