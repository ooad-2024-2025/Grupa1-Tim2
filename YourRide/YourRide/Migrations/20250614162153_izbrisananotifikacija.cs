using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourRide.Migrations
{
    /// <inheritdoc />
    public partial class izbrisananotifikacija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifikacija");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifikacija",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PosiljalacId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PrimalacId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VrijemeSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    poruka = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifikacija", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Notifikacija_AspNetUsers_PosiljalacId",
                        column: x => x.PosiljalacId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifikacija_AspNetUsers_PrimalacId",
                        column: x => x.PrimalacId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifikacija_PosiljalacId",
                table: "Notifikacija",
                column: "PosiljalacId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifikacija_PrimalacId",
                table: "Notifikacija",
                column: "PrimalacId");
        }
    }
}
