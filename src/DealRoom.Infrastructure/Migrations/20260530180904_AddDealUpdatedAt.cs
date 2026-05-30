using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealRoom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDealUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Deals",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Deals");
        }
    }
}
