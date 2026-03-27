using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Pos.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderDeliveryAddressFields4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseProductName",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseProductPrice",
                table: "OrderItems",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationJson",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseProductName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BaseProductPrice",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ConfigurationJson",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderItems");
        }
    }
}
