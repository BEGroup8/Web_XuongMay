using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_XuongMay.Migrations
{
    /// <inheritdoc />
    public partial class AddLoai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaLoai",
                table: "Catagory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Loai",
                columns: table => new
                {
                    MaLoai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loai", x => x.MaLoai);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catagory_MaLoai",
                table: "Catagory",
                column: "MaLoai");

            migrationBuilder.AddForeignKey(
                name: "FK_Catagory_Loai_MaLoai",
                table: "Catagory",
                column: "MaLoai",
                principalTable: "Loai",
                principalColumn: "MaLoai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catagory_Loai_MaLoai",
                table: "Catagory");

            migrationBuilder.DropTable(
                name: "Loai");

            migrationBuilder.DropIndex(
                name: "IX_Catagory_MaLoai",
                table: "Catagory");

            migrationBuilder.DropColumn(
                name: "MaLoai",
                table: "Catagory");
        }
    }
}
