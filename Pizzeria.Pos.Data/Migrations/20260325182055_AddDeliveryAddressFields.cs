using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Pos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryApartmentNumber",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryCity",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryHouseNumber",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPostalCode",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryStreet",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliverySymbol",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryApartmentNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryCity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryHouseNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPostalCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryStreet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliverySymbol",
                table: "Orders");
        }
    }
}
