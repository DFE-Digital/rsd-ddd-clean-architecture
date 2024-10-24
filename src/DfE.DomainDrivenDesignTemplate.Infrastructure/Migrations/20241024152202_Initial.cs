using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DfE.DomainDrivenDesignTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "scl");

            migrationBuilder.CreateTable(
                name: "PrincipalDetails",
                schema: "scl",
                columns: table => new
                {
                    PrincipalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipalDetails", x => x.PrincipalId);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                schema: "scl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrimitiveId = table.Column<int>(type: "int", nullable: false, computedColumnSql: "[Id]"),
                    PrincipalId = table.Column<int>(type: "int", nullable: false),
                    SchoolName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameListAs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameDisplayAs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFullTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastRefresh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schools_PrincipalDetails_PrincipalId",
                        column: x => x.PrincipalId,
                        principalSchema: "scl",
                        principalTable: "PrincipalDetails",
                        principalColumn: "PrincipalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schools_PrincipalId",
                schema: "scl",
                table: "Schools",
                column: "PrincipalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schools",
                schema: "scl");

            migrationBuilder.DropTable(
                name: "PrincipalDetails",
                schema: "scl");
        }
    }
}
