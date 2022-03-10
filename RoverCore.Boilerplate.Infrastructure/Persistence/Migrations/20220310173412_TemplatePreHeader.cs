using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoverCore.Boilerplate.Infrastructure.Persistence.Migrations
{
    public partial class TemplatePreHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreHeader",
                table: "Template",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreHeader",
                table: "Template");
        }
    }
}
