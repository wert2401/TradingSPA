using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradingSite.Migrations
{
    public partial class AddedUpTimetoItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "upTime",
                table: "Items",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "upTime",
                table: "Items");
        }
    }
}
