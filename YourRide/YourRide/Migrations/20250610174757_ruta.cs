using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourRide.Migrations
{
    /// <inheritdoc />
    public partial class ruta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrajanjeMinuta",
                table: "Ruta");

            migrationBuilder.DropColumn(
                name: "UdaljenostKm",
                table: "Ruta");

            

            migrationBuilder.AlterColumn<int>(
                name: "KrajnjaLokacijaId",
                table: "Ruta",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            
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

            migrationBuilder.AlterColumn<int>(
                name: "KrajnjaLokacijaId",
                table: "Ruta",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TrajanjeMinuta",
                table: "Ruta",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "UdaljenostKm",
                table: "Ruta",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Udaljenost",
                table: "Lokacija",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
