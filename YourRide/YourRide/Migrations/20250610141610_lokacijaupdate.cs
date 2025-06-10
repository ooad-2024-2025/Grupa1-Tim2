using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourRide.Migrations
{
    /// <inheritdoc />
    public partial class lokacijaupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latituda",
                table: "Lokacija",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longituda",
                table: "Lokacija",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latituda",
                table: "Lokacija");

            migrationBuilder.DropColumn(
                name: "Longituda",
                table: "Lokacija");
        }
    }
}
