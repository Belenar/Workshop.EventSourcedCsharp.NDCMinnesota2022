using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeerSender.API.Migrations.ReadcontextMigrations
{
    /// <inheritdoc />
    public partial class CreateReadDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Package_beers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brewery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beername = table.Column<string>(name: "Beer_name", type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package_beers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projection_checkpoints",
                columns: table => new
                {
                    Projectionname = table.Column<string>(name: "Projection_name", type: "nvarchar(450)", nullable: false),
                    Eventid = table.Column<int>(name: "Event_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projection_checkpoints", x => x.Projectionname);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Package_beers");

            migrationBuilder.DropTable(
                name: "Projection_checkpoints");
        }
    }
}
