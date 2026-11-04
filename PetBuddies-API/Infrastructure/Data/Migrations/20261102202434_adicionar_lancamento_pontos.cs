using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBuddies_API.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class adicionar_lancamento_pontos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_PB_LANCAMENTO_PONTOS",
                columns: table => new
                {
                    ID_LANCAMENTO_PONTOS = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CONSULTA = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_RESPONSAVEL = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_ANIMAL = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DH_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NR_PONTOS = table.Column<int>(type: "NUMBER(6)", precision: 6, nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_LANCAMENTO_PONTOS", x => x.ID_LANCAMENTO_PONTOS);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LANCAMENTO_RESPONSAVEL",
                table: "T_PB_LANCAMENTO_PONTOS",
                column: "ID_RESPONSAVEL");

            migrationBuilder.CreateIndex(
                name: "UK_LANCAMENTO_CONSULTA",
                table: "T_PB_LANCAMENTO_PONTOS",
                column: "ID_CONSULTA",
                unique: true);

            // CHECK via Sql, como em add_back_office_catalogo_e_politica: o provider Oracle
            // nao escapa as aspas dos literais quando gera a constraint.
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_LANCAMENTO_PONTOS ADD CONSTRAINT CK_LANCAMENTO_PONTOS CHECK (NR_PONTOS >= 0)");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PONTUACAO DROP CONSTRAINT CK_PONTUACAO_GESTO");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PONTUACAO ADD CONSTRAINT CK_PONTUACAO_GESTO CHECK (TP_GESTO IN ('PLANO_CRIADO','CONSULTA_AGENDADA','CONSULTA_REALIZADA','PROCEDIMENTO_EXECUTADO','CHECKIN_REGISTRADO','ADESAO_COMPLETA','EVOLUCAO_POSITIVA'))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_PB_LANCAMENTO_PONTOS");

            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PONTUACAO DROP CONSTRAINT CK_PONTUACAO_GESTO");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PONTUACAO ADD CONSTRAINT CK_PONTUACAO_GESTO CHECK (TP_GESTO IN ('PLANO_CRIADO','CONSULTA_AGENDADA','CONSULTA_REALIZADA','PROCEDIMENTO_EXECUTADO'))");
        }
    }
}
