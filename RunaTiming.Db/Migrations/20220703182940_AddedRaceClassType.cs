using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunaTiming.Db.Migrations
{
    public partial class AddedRaceClassType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassType",
                table: "Races",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassType",
                table: "Races");
        }
    }
}
