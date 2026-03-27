using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Pos.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderDeliveryAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApartmentNumber",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Orders");
        }
    }
}
