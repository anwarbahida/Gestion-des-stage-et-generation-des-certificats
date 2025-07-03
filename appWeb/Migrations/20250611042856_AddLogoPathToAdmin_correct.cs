using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoPathToAdmin_correct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
            name: "LogoPath",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "AspNetUsers");


        }
    }
}
