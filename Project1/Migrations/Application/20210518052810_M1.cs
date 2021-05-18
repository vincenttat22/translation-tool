using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project1.Migrations.Application
{
    public partial class M1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExportFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    ExportedFileName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    TranslatedFromCode = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    TranslatedToCode = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(5)", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileManagement", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportFiles");

            migrationBuilder.DropTable(
                name: "FileManagement");
        }
    }
}
