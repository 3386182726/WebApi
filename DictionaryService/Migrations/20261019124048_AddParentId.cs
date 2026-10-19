using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DictionaryService.Migrations
{
    /// <inheritdoc />
    public partial class AddParentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "Dictionaries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Dictionaries");
        }
    }
}
