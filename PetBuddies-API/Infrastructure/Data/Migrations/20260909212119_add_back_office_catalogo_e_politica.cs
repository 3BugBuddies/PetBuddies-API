using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBuddies_API.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_back_office_catalogo_e_politica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_PB_PROTOCOLO",
                columns: table => new
                {
                    ID_PROTOCOLO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_NOME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    TP_CATEGORIA_PROTOCOLO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ES_ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    AT_ATIVO = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: true),
                    DS_DESCRICAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_PROTOCOLO", x => x.ID_PROTOCOLO);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_REGRA_PONTUACAO",
                columns: table => new
                {
                    ID_REGRA_PONTUACAO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TP_GESTO = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    NR_PONTOS = table.Column<int>(type: "NUMBER(5)", precision: 5, nullable: false),
                    DT_INICIO_VIGENCIA = table.Column<DateTime>(type: "DATE", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_REGRA_PONTUACAO", x => x.ID_REGRA_PONTUACAO);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_OFERTA",
                columns: table => new
                {
                    ID_OFERTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TP_ATO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    TP_SUBTIPO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    ID_PROTOCOLO = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    DS_DESCRICAO = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    NR_VALOR = table.Column<decimal>(type: "NUMBER(10,2)", precision: 10, scale: 2, nullable: false),
                    DT_INICIO_VIGENCIA = table.Column<DateTime>(type: "DATE", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_OFERTA", x => x.ID_OFERTA);
                    table.ForeignKey(
                        name: "FK_OFERTA_PROTOCOLO",
                        column: x => x.ID_PROTOCOLO,
                        principalTable: "T_PB_PROTOCOLO",
                        principalColumn: "ID_PROTOCOLO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_REGRA_PROTOCOLO",
                columns: table => new
                {
                    ID_REGRA_PROTOCOLO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PROTOCOLO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TP_TIPO_CUIDADO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NM_NOME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    NR_OFFSET = table.Column<int>(type: "NUMBER(4)", precision: 4, nullable: false),
                    TP_UNIDADE_OFFSET = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    TP_DATA_BASE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    NR_INTERVALO = table.Column<int>(type: "NUMBER(4)", precision: 4, nullable: true),
                    TP_UNIDADE_INTERVALO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    NR_REPETICOES = table.Column<int>(type: "NUMBER(3)", precision: 3, nullable: false),
                    DS_DESCRICAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_REGRA_PROTOCOLO", x => x.ID_REGRA_PROTOCOLO);
                    table.ForeignKey(
                        name: "FK_REGPROT_PROTOCOLO",
                        column: x => x.ID_PROTOCOLO,
                        principalTable: "T_PB_PROTOCOLO",
                        principalColumn: "ID_PROTOCOLO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_OFERTA_ID_PROTOCOLO",
                table: "T_PB_OFERTA",
                column: "ID_PROTOCOLO");

            migrationBuilder.CreateIndex(
                name: "UX_OFERTA_VIGENCIA",
                table: "T_PB_OFERTA",
                columns: new[] { "ID_CLINICA", "TP_ATO", "TP_SUBTIPO", "ID_PROTOCOLO", "DT_INICIO_VIGENCIA" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_PONTUACAO_VIGENCIA",
                table: "T_PB_REGRA_PONTUACAO",
                columns: new[] { "ID_CLINICA", "TP_GESTO", "DT_INICIO_VIGENCIA" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_REGRA_PROTOCOLO_ID_PROTOCOLO",
                table: "T_PB_REGRA_PROTOCOLO",
                column: "ID_PROTOCOLO");

            // Os CHECK saem do CreateTable de proposito: o provider Oracle embrulha o
            // CREATE TABLE em EXECUTE IMMEDIATE '...' e nao escapa as aspas dos literais,
            // gerando ORA-06550 na subida. migrationBuilder.Sql emite DDL direto.
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_PROTOCOLO ADD CONSTRAINT CK_PROTOCOLO_ATIVO CHECK (AT_ATIVO IN (0,1))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_PROTOCOLO ADD CONSTRAINT CK_PROTOCOLO_CATEGORIA CHECK (TP_CATEGORIA_PROTOCOLO IN ('PREVENTIVO','POS_CIRURGICO'))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_PROTOCOLO ADD CONSTRAINT CK_PROTOCOLO_ESPECIE CHECK (ES_ESPECIE IN ('CACHORRO','GATO','PASSARO','COELHO','HAMSTER','OUTRO'))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PONTUACAO ADD CONSTRAINT CK_PONTUACAO_GESTO CHECK (TP_GESTO IN ('PLANO_CRIADO','CONSULTA_AGENDADA','CONSULTA_REALIZADA','PROCEDIMENTO_EXECUTADO'))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PONTUACAO ADD CONSTRAINT CK_PONTUACAO_PONTOS CHECK (NR_PONTOS > 0)");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_OFERTA ADD CONSTRAINT CK_OFERTA_ALVO CHECK ((TP_ATO IN ('PROCEDIMENTO','CONSULTA') AND TP_SUBTIPO IS NOT NULL AND ID_PROTOCOLO IS NULL) OR (TP_ATO = 'PROTOCOLO' AND ID_PROTOCOLO IS NOT NULL AND TP_SUBTIPO IS NULL))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_OFERTA ADD CONSTRAINT CK_OFERTA_ATO CHECK (TP_ATO IN ('PROCEDIMENTO','CONSULTA','PROTOCOLO'))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_OFERTA ADD CONSTRAINT CK_OFERTA_VALOR CHECK (NR_VALOR >= 0)");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PROTOCOLO ADD CONSTRAINT CK_REGPROT_ANCORA CHECK (TP_DATA_BASE IN ('NASCIMENTO','DATA_CIRURGIA','ULTIMA_REALIZACAO'))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PROTOCOLO ADD CONSTRAINT CK_REGPROT_RECORRENCIA CHECK ((NR_INTERVALO IS NULL AND TP_UNIDADE_INTERVALO IS NULL) OR (NR_INTERVALO IS NOT NULL AND TP_UNIDADE_INTERVALO IS NOT NULL))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PROTOCOLO ADD CONSTRAINT CK_REGPROT_REPETICOES CHECK (NR_REPETICOES >= 1)");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PROTOCOLO ADD CONSTRAINT CK_REGPROT_TIPO CHECK (TP_TIPO_CUIDADO IN ('VACINACAO','VERMIFUGACAO','EXAME','RETORNO','CIRURGIA','MEDICACAO','HIGIENE'))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PROTOCOLO ADD CONSTRAINT CK_REGPROT_UNID_INTERV CHECK (TP_UNIDADE_INTERVALO IN ('DIAS','SEMANAS','MESES'))");
            migrationBuilder.Sql(
                "ALTER TABLE T_PB_REGRA_PROTOCOLO ADD CONSTRAINT CK_REGPROT_UNID_OFFSET CHECK (TP_UNIDADE_OFFSET IN ('DIAS','SEMANAS','MESES'))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_PB_OFERTA");

            migrationBuilder.DropTable(
                name: "T_PB_REGRA_PONTUACAO");

            migrationBuilder.DropTable(
                name: "T_PB_REGRA_PROTOCOLO");

            migrationBuilder.DropTable(
                name: "T_PB_PROTOCOLO");
        }
    }
}
