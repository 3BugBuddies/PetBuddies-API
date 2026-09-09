using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBuddies_API.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class remover_registro_clinico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_PB_JANELA_ATENDIMENTO");

            migrationBuilder.DropTable(
                name: "T_PB_PROCEDIMENTO");

            migrationBuilder.DropTable(
                name: "T_PB_REGRA_PRESCRICAO");

            migrationBuilder.DropTable(
                name: "T_PB_CONDICAO_CLINICA");

            migrationBuilder.DropTable(
                name: "T_PB_PRESCRICAO");

            migrationBuilder.DropTable(
                name: "T_PB_REGISTRO_ATENDIMENTO");

            migrationBuilder.DropTable(
                name: "T_PB_CONSULTA");

            migrationBuilder.DropTable(
                name: "T_PB_ANIMAL");

            migrationBuilder.DropTable(
                name: "T_PB_VETERINARIO");

            migrationBuilder.DropTable(
                name: "T_PB_RESPONSAVEL");

            migrationBuilder.DropTable(
                name: "T_PB_CLINICA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_PB_CLINICA",
                columns: table => new
                {
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NR_CNPJ = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EM_EMAIL = table.Column<string>(type: "NVARCHAR2(254)", maxLength: 254, nullable: true),
                    NM_NOME_CLINICA = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    TL_TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_CLINICA", x => x.ID_CLINICA);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_RESPONSAVEL",
                columns: table => new
                {
                    ID_RESPONSAVEL = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EM_EMAIL = table.Column<string>(type: "NVARCHAR2(254)", maxLength: 254, nullable: true),
                    NM_NOME_RESPONSAVEL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    TL_TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_RESPONSAVEL", x => x.ID_RESPONSAVEL);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_VETERINARIO",
                columns: table => new
                {
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AT_ATIVO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NR_CRMV = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    EM_EMAIL = table.Column<string>(type: "NVARCHAR2(254)", maxLength: 254, nullable: true),
                    NM_NOME_VETERINARIO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_VETERINARIO", x => x.ID_VETERINARIO);
                    table.ForeignKey(
                        name: "FK_T_PB_VETERINARIO_T_PB_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "T_PB_CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_ANIMAL",
                columns: table => new
                {
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_RESPONSAVEL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    OB_ALERGIA = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    CT_CASTRADO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CN_CONDICAO_CRONICA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_DATA_NASCIMENTO = table.Column<DateTime>(type: "DATE", nullable: false),
                    ES_ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FT_FOTO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    NM_NOME_ANIMAL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    OB_OBSERVACOES = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    NR_PESO = table.Column<decimal>(type: "NUMBER(5,2)", nullable: true),
                    PT_PORTE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RC_RACA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    SX_SEXO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_ANIMAL", x => x.ID_ANIMAL);
                    table.ForeignKey(
                        name: "FK_T_PB_ANIMAL_T_PB_RESPONSAVEL_ID_RESPONSAVEL",
                        column: x => x.ID_RESPONSAVEL,
                        principalTable: "T_PB_RESPONSAVEL",
                        principalColumn: "ID_RESPONSAVEL",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_CONDICAO_CLINICA",
                columns: table => new
                {
                    ID_CONDICAO_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_VETERINARIO_AUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AT_ATIVO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CD_CODIGO = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FL_CRITICA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TP_FONTE_VALOR = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DS_ROTULO = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    TP_DADO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DS_UNIDADE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_CONDICAO_CLINICA", x => x.ID_CONDICAO_CLINICA);
                    table.ForeignKey(
                        name: "FK_T_PB_CONDICAO_CLINICA_T_PB_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "T_PB_CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_PB_CONDICAO_CLINICA_T_PB_VETERINARIO_ID_VETERINARIO_AUTOR",
                        column: x => x.ID_VETERINARIO_AUTOR,
                        principalTable: "T_PB_VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_CONSULTA",
                columns: table => new
                {
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DH_DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    MT_MOTIVO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    OB_OBSERVACAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    ST_STATUS_CONSULTA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    TP_TIPO_CONSULTA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_CONSULTA", x => x.ID_CONSULTA);
                    table.ForeignKey(
                        name: "FK_T_PB_CONSULTA_T_PB_ANIMAL_ID_ANIMAL",
                        column: x => x.ID_ANIMAL,
                        principalTable: "T_PB_ANIMAL",
                        principalColumn: "ID_ANIMAL",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_PB_CONSULTA_T_PB_VETERINARIO_ID_VETERINARIO",
                        column: x => x.ID_VETERINARIO,
                        principalTable: "T_PB_VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_JANELA_ATENDIMENTO",
                columns: table => new
                {
                    ID_JANELA_ATENDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DH_DATA_HORA_INICIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_JANELA_ATENDIMENTO", x => x.ID_JANELA_ATENDIMENTO);
                    table.ForeignKey(
                        name: "FK_T_PB_JANELA_ATENDIMENTO_T_PB_CONSULTA_ID_CONSULTA",
                        column: x => x.ID_CONSULTA,
                        principalTable: "T_PB_CONSULTA",
                        principalColumn: "ID_CONSULTA",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_T_PB_JANELA_ATENDIMENTO_T_PB_VETERINARIO_ID_VETERINARIO",
                        column: x => x.ID_VETERINARIO,
                        principalTable: "T_PB_VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_REGISTRO_ATENDIMENTO",
                columns: table => new
                {
                    ID_REGISTRO_ATENDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AN_ANAMNESE = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_DATA_ATENDIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DG_DIAGNOSTICO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    OB_OBSERVACAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    PR_PROXIMA_VACINA = table.Column<DateTime>(type: "DATE", nullable: true),
                    PR_PROXIMO_RETORNO = table.Column<DateTime>(type: "DATE", nullable: true),
                    TR_TRATAMENTO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_REGISTRO_ATENDIMENTO", x => x.ID_REGISTRO_ATENDIMENTO);
                    table.ForeignKey(
                        name: "FK_T_PB_REGISTRO_ATENDIMENTO_T_PB_ANIMAL_ID_ANIMAL",
                        column: x => x.ID_ANIMAL,
                        principalTable: "T_PB_ANIMAL",
                        principalColumn: "ID_ANIMAL",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_PB_REGISTRO_ATENDIMENTO_T_PB_CONSULTA_ID_CONSULTA",
                        column: x => x.ID_CONSULTA,
                        principalTable: "T_PB_CONSULTA",
                        principalColumn: "ID_CONSULTA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_PRESCRICAO",
                columns: table => new
                {
                    ID_PRESCRICAO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_REGISTRO_ATENDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_INICIO = table.Column<DateTime>(type: "DATE", nullable: false),
                    NR_DOSE_MAX = table.Column<decimal>(type: "NUMBER(8,3)", nullable: false),
                    NR_DOSE_MIN = table.Column<decimal>(type: "NUMBER(8,3)", nullable: false),
                    NR_DURACAO_DIAS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NR_FREQUENCIA_DIA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_MATERIAL_ORIGEM = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NM_MEDICAMENTO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    TX_ORIENTACAO = table.Column<string>(type: "CLOB", nullable: true),
                    DS_UNIDADE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NR_VERSAO_ORIGEM = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_PRESCRICAO", x => x.ID_PRESCRICAO);
                    table.ForeignKey(
                        name: "FK_T_PB_PRESCRICAO_T_PB_ANIMAL_ID_ANIMAL",
                        column: x => x.ID_ANIMAL,
                        principalTable: "T_PB_ANIMAL",
                        principalColumn: "ID_ANIMAL",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_PB_PRESCRICAO_T_PB_REGISTRO_ATENDIMENTO_ID_REGISTRO_ATENDIMENTO",
                        column: x => x.ID_REGISTRO_ATENDIMENTO,
                        principalTable: "T_PB_REGISTRO_ATENDIMENTO",
                        principalColumn: "ID_REGISTRO_ATENDIMENTO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_PB_PRESCRICAO_T_PB_VETERINARIO_ID_VETERINARIO",
                        column: x => x.ID_VETERINARIO,
                        principalTable: "T_PB_VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_PROCEDIMENTO",
                columns: table => new
                {
                    ID_PROCEDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_REGISTRO_ATENDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AN_ANEXOS_URL = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_DATA_PREVISTA_FIM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_DATA_PREVISTA_INICIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DS_DESCRICAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    NM_NOME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    OB_OBSERVACAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    ST_STATUS_PROCEDIMENTO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    TP_TIPO_PROCEDIMENTO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_PROCEDIMENTO", x => x.ID_PROCEDIMENTO);
                    table.ForeignKey(
                        name: "FK_T_PB_PROCEDIMENTO_T_PB_ANIMAL_ID_ANIMAL",
                        column: x => x.ID_ANIMAL,
                        principalTable: "T_PB_ANIMAL",
                        principalColumn: "ID_ANIMAL",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_PB_PROCEDIMENTO_T_PB_REGISTRO_ATENDIMENTO_ID_REGISTRO_ATENDIMENTO",
                        column: x => x.ID_REGISTRO_ATENDIMENTO,
                        principalTable: "T_PB_REGISTRO_ATENDIMENTO",
                        principalColumn: "ID_REGISTRO_ATENDIMENTO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_PB_PROCEDIMENTO_T_PB_VETERINARIO_ID_VETERINARIO",
                        column: x => x.ID_VETERINARIO,
                        principalTable: "T_PB_VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_REGRA_PRESCRICAO",
                columns: table => new
                {
                    ID_REGRA_PRESCRICAO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CONDICAO_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_PRESCRICAO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TP_ACAO_DOSE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TP_FONTE_VALOR_CONGELADA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NR_LIMITE = table.Column<decimal>(type: "NUMBER(10,3)", nullable: true),
                    TP_OPERADOR = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NR_ORDEM = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DS_ROTULO_CONGELADO = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    TP_DADO_CONGELADO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_REGRA_PRESCRICAO", x => x.ID_REGRA_PRESCRICAO);
                    table.ForeignKey(
                        name: "FK_T_PB_REGRA_PRESCRICAO_T_PB_CONDICAO_CLINICA_ID_CONDICAO_CLINICA",
                        column: x => x.ID_CONDICAO_CLINICA,
                        principalTable: "T_PB_CONDICAO_CLINICA",
                        principalColumn: "ID_CONDICAO_CLINICA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_PB_REGRA_PRESCRICAO_T_PB_PRESCRICAO_ID_PRESCRICAO",
                        column: x => x.ID_PRESCRICAO,
                        principalTable: "T_PB_PRESCRICAO",
                        principalColumn: "ID_PRESCRICAO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_ANIMAL_ID_RESPONSAVEL",
                table: "T_PB_ANIMAL",
                column: "ID_RESPONSAVEL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CLINICA_NR_CNPJ",
                table: "T_PB_CLINICA",
                column: "NR_CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CONDICAO_CLINICA_ID_CLINICA_CD_CODIGO",
                table: "T_PB_CONDICAO_CLINICA",
                columns: new[] { "ID_CLINICA", "CD_CODIGO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CONDICAO_CLINICA_ID_VETERINARIO_AUTOR",
                table: "T_PB_CONDICAO_CLINICA",
                column: "ID_VETERINARIO_AUTOR");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CONSULTA_ID_ANIMAL",
                table: "T_PB_CONSULTA",
                column: "ID_ANIMAL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CONSULTA_ID_VETERINARIO",
                table: "T_PB_CONSULTA",
                column: "ID_VETERINARIO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_JANELA_ATENDIMENTO_ID_CONSULTA",
                table: "T_PB_JANELA_ATENDIMENTO",
                column: "ID_CONSULTA");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_JANELA_ATENDIMENTO_ID_VETERINARIO_DH_DATA_HORA_INICIO",
                table: "T_PB_JANELA_ATENDIMENTO",
                columns: new[] { "ID_VETERINARIO", "DH_DATA_HORA_INICIO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_PRESCRICAO_ID_ANIMAL",
                table: "T_PB_PRESCRICAO",
                column: "ID_ANIMAL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_PRESCRICAO_ID_REGISTRO_ATENDIMENTO",
                table: "T_PB_PRESCRICAO",
                column: "ID_REGISTRO_ATENDIMENTO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_PRESCRICAO_ID_VETERINARIO",
                table: "T_PB_PRESCRICAO",
                column: "ID_VETERINARIO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_PROCEDIMENTO_ID_ANIMAL",
                table: "T_PB_PROCEDIMENTO",
                column: "ID_ANIMAL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_PROCEDIMENTO_ID_REGISTRO_ATENDIMENTO",
                table: "T_PB_PROCEDIMENTO",
                column: "ID_REGISTRO_ATENDIMENTO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_PROCEDIMENTO_ID_VETERINARIO",
                table: "T_PB_PROCEDIMENTO",
                column: "ID_VETERINARIO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_REGISTRO_ATENDIMENTO_ID_ANIMAL",
                table: "T_PB_REGISTRO_ATENDIMENTO",
                column: "ID_ANIMAL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_REGISTRO_ATENDIMENTO_ID_CONSULTA",
                table: "T_PB_REGISTRO_ATENDIMENTO",
                column: "ID_CONSULTA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_REGRA_PRESCRICAO_ID_CONDICAO_CLINICA",
                table: "T_PB_REGRA_PRESCRICAO",
                column: "ID_CONDICAO_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_REGRA_PRESCRICAO_ID_PRESCRICAO",
                table: "T_PB_REGRA_PRESCRICAO",
                column: "ID_PRESCRICAO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_VETERINARIO_ID_CLINICA",
                table: "T_PB_VETERINARIO",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_VETERINARIO_NR_CRMV",
                table: "T_PB_VETERINARIO",
                column: "NR_CRMV",
                unique: true);
        }
    }
}
