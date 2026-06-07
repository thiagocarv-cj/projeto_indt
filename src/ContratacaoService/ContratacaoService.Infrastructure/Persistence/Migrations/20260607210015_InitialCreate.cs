using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContratacaoService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contratacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeSegurado = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    ValorCobertura = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DataContratacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contratacoes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contratacoes_PropostaId",
                table: "contratacoes",
                column: "PropostaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contratacoes");
        }
    }
}
