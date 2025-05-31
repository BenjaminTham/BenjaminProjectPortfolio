using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Sql.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexForColorAndSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sizes_Name",
                table: "Sizes",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Colors_HexCode",
                table: "Colors",
                column: "HexCode",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Colors_Name",
                table: "Colors",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sizes_Name",
                table: "Sizes");

            migrationBuilder.DropIndex(
                name: "IX_Colors_HexCode",
                table: "Colors");

            migrationBuilder.DropIndex(
                name: "IX_Colors_Name",
                table: "Colors");
        }
    }
}
