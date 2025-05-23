using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourRide.Migrations
{
    /// <inheritdoc />
    public partial class voznja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PutnikId",
                table: "Voznja",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RutaId",
                table: "Voznja",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VozacId",
                table: "Voznja",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Voznja_PutnikId",
                table: "Voznja",
                column: "PutnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Voznja_RutaId",
                table: "Voznja",
                column: "RutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Voznja_VozacId",
                table: "Voznja",
                column: "VozacId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voznja_Korisnik_PutnikId",
                table: "Voznja",
                column: "PutnikId",
                principalTable: "Korisnik",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Voznja_Korisnik_VozacId",
                table: "Voznja",
                column: "VozacId",
                principalTable: "Korisnik",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Voznja_Ruta_RutaId",
                table: "Voznja",
                column: "RutaId",
                principalTable: "Ruta",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voznja_Korisnik_PutnikId",
                table: "Voznja");

            migrationBuilder.DropForeignKey(
                name: "FK_Voznja_Korisnik_VozacId",
                table: "Voznja");

            migrationBuilder.DropForeignKey(
                name: "FK_Voznja_Ruta_RutaId",
                table: "Voznja");

            migrationBuilder.DropIndex(
                name: "IX_Voznja_PutnikId",
                table: "Voznja");

            migrationBuilder.DropIndex(
                name: "IX_Voznja_RutaId",
                table: "Voznja");

            migrationBuilder.DropIndex(
                name: "IX_Voznja_VozacId",
                table: "Voznja");

            migrationBuilder.DropColumn(
                name: "PutnikId",
                table: "Voznja");

            migrationBuilder.DropColumn(
                name: "RutaId",
                table: "Voznja");

            migrationBuilder.DropColumn(
                name: "VozacId",
                table: "Voznja");
        }
    }
}
