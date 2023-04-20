using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Birai.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Bvid = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    TypeName = table.Column<string>(type: "TEXT", nullable: false),
                    DateTime = table.Column<string>(type: "TEXT", nullable: false),
                    CopyRight = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubmiterId = table.Column<string>(type: "TEXT", nullable: false),
                    SubmiterName = table.Column<string>(type: "TEXT", nullable: false),
                    SubmitReason = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WatchedVideo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Bvid = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchedVideo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoInfos");

            migrationBuilder.DropTable(
                name: "WatchedVideo");
        }
    }
}
