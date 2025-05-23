using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourRide.Migrations
{
    /// <inheritdoc />
    public partial class notifikacija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PosiljalacId",
                table: "Notifikacija",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrimalacId",
                table: "Notifikacija",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VrijemeSlanja",
                table: "Notifikacija",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "poruka",
                table: "Notifikacija",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notifikacija_PosiljalacId",
                table: "Notifikacija",
                column: "PosiljalacId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifikacija_PrimalacId",
                table: "Notifikacija",
                column: "PrimalacId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifikacija_Korisnik_PosiljalacId",
                table: "Notifikacija",
                column: "PosiljalacId",
                principalTable: "Korisnik",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifikacija_Korisnik_PrimalacId",
                table: "Notifikacija",
                column: "PrimalacId",
                principalTable: "Korisnik",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifikacija_Korisnik_PosiljalacId",
                table: "Notifikacija");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifikacija_Korisnik_PrimalacId",
                table: "Notifikacija");

            migrationBuilder.DropIndex(
                name: "IX_Notifikacija_PosiljalacId",
                table: "Notifikacija");

            migrationBuilder.DropIndex(
                name: "IX_Notifikacija_PrimalacId",
                table: "Notifikacija");

            migrationBuilder.DropColumn(
                name: "PosiljalacId",
                table: "Notifikacija");

            migrationBuilder.DropColumn(
                name: "PrimalacId",
                table: "Notifikacija");

            migrationBuilder.DropColumn(
                name: "VrijemeSlanja",
                table: "Notifikacija");

            migrationBuilder.DropColumn(
                name: "poruka",
                table: "Notifikacija");
        }
    }
}
