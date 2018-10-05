using Microsoft.EntityFrameworkCore.Migrations;

namespace dpearson4_MVC_Music.Data.MusicMigrations
{
    public partial class UniqueTitleYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Albums_YearProduced_Name",
                schema: "MUSIC",
                table: "Albums",
                columns: new[] { "YearProduced", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Albums_YearProduced_Name",
                schema: "MUSIC",
                table: "Albums");
        }
    }
}
