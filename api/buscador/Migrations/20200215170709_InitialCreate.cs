using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace buscador.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roupas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    Preco = table.Column<decimal>(type: "real", nullable: false),
                    UrlProduto = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    UrlImagem = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    Categoria = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    Origem = table.Column<string>(type: "text", maxLength: 25, nullable: false),
                    DataBusca = table.Column<DateTime>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roupas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoupasTamanho",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoupaId = table.Column<int>(nullable: false),
                    Tamanho = table.Column<string>(type: "text", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoupasTamanho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoupasTamanho_Roupas_RoupaId",
                        column: x => x.RoupaId,
                        principalTable: "Roupas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoupasTamanho_RoupaId",
                table: "RoupasTamanho",
                column: "RoupaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoupasTamanho");

            migrationBuilder.DropTable(
                name: "Roupas");
        }
    }
}
