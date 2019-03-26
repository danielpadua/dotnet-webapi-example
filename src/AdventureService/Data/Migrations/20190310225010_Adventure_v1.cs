using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace AdventureService.Data.Migrations
{
    public partial class Adventure_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "addventure",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    main_photo_url = table.Column<string>(maxLength: 255, nullable: false),
                    photos_url = table.Column<string[]>(nullable: true),
                    title = table.Column<string>(maxLength: 60, nullable: false),
                    description = table.Column<string>(nullable: false),
                    location = table.Column<Point>(nullable: false),
                    level = table.Column<string>(maxLength: 15, nullable: false),
                    category_id = table.Column<string>(nullable: true),
                    rating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addventure", x => x.id);
                    table.ForeignKey(
                        name: "fk_addventure_category_category_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adventure_tag",
                columns: table => new
                {
                    adventure_id = table.Column<string>(nullable: false),
                    tag_name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_adventure_tag", x => new { x.adventure_id, x.tag_name });
                    table.ForeignKey(
                        name: "fk_adventure_tag_addventure_adventure_id",
                        column: x => x.adventure_id,
                        principalTable: "addventure",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_adventure_tag_tag_tag_name",
                        column: x => x.tag_name,
                        principalTable: "tag",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_addventure_category_id",
                table: "addventure",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_addventure_rating",
                table: "addventure",
                column: "rating",
                filter: "rating between 0 and 5");

            migrationBuilder.CreateIndex(
                name: "ix_adventure_tag_tag_name",
                table: "adventure_tag",
                column: "tag_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adventure_tag");

            migrationBuilder.DropTable(
                name: "addventure");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropTable(
                name: "category");
        }
    }
}
