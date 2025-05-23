using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourRide.Migrations
{
    /// <inheritdoc />
    public partial class voznjadone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Cijena",
                table: "Voznja",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "VrijemePocetka",
                table: "Voznja",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "VrijemeZavrsetka",
                table: "Voznja",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Voznja",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cijena",
                table: "Voznja");

            migrationBuilder.DropColumn(
                name: "VrijemePocetka",
                table: "Voznja");

            migrationBuilder.DropColumn(
                name: "VrijemeZavrsetka",
                table: "Voznja");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Voznja");
        }
    }
}
