using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBuddies_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_PB_ENDERECO",
                columns: table => new
                {
                    ID_ENDERECO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    LG_LOGRADOURO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    NR_NUMERO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CM_COMPLEMENTO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    BR_BAIRRO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    CD_CIDADE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ES_ESTADO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    NR_CEP = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_ENDERECO", x => x.ID_ENDERECO);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_TIPO_ANIMAL",
                columns: table => new
                {
                    ID_TIPO_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ES_ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RC_RACA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    PT_PORTE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_TIPO_ANIMAL", x => x.ID_TIPO_ANIMAL);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_CLINICA",
                columns: table => new
                {
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_NOME_CLINICA = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    NR_CNPJ = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    TL_TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    EM_EMAIL = table.Column<string>(type: "NVARCHAR2(254)", maxLength: 254, nullable: true),
                    ID_ENDERECO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_CLINICA", x => x.ID_CLINICA);
                    table.ForeignKey(
                        name: "FK_T_PB_CLINICA_T_PB_ENDERECO_ID_ENDERECO",
                        column: x => x.ID_ENDERECO,
                        principalTable: "T_PB_ENDERECO",
                        principalColumn: "ID_ENDERECO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_RESPONSAVEL",
                columns: table => new
                {
                    ID_RESPONSAVEL = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_NOME_RESPONSAVEL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    NR_CPF = table.Column<string>(type: "NVARCHAR2(11)", maxLength: 11, nullable: false),
                    DT_DATA_NASCIMENTO = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    TL_TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    EM_EMAIL = table.Column<string>(type: "NVARCHAR2(254)", maxLength: 254, nullable: true),
                    ST_STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_ENDERECO = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_RESPONSAVEL", x => x.ID_RESPONSAVEL);
                    table.ForeignKey(
                        name: "FK_T_PB_RESPONSAVEL_T_PB_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "T_PB_CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_PB_RESPONSAVEL_T_PB_ENDERECO_ID_ENDERECO",
                        column: x => x.ID_ENDERECO,
                        principalTable: "T_PB_ENDERECO",
                        principalColumn: "ID_ENDERECO");
                });

            migrationBuilder.CreateTable(
                name: "T_PB_VETERINARIO",
                columns: table => new
                {
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_NOME_VETERINARIO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    NR_CRMV = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    ES_ESPECIALIDADE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    TL_TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    EM_EMAIL = table.Column<string>(type: "NVARCHAR2(254)", maxLength: 254, nullable: true),
                    AE_ATENDE_EMERGENCIA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AT_ATIVO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
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
                    NM_NOME_ANIMAL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    SX_SEXO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DT_DATA_NASCIMENTO = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    NR_PESO = table.Column<decimal>(type: "NUMBER(5,2)", nullable: false),
                    CN_CONDICAO_CRONICA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PC_PRE_CADASTRO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CT_CASTRADO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FT_FOTO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    ID_RESPONSAVEL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_TIPO_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_T_PB_ANIMAL_T_PB_TIPO_ANIMAL_ID_TIPO_ANIMAL",
                        column: x => x.ID_TIPO_ANIMAL,
                        principalTable: "T_PB_TIPO_ANIMAL",
                        principalColumn: "ID_TIPO_ANIMAL",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_JANELA_ATENDIMENTO",
                columns: table => new
                {
                    ID_JANELA_ATENDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DH_DATA_HORA_INICIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DH_DATA_HORA_FIM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DR_DURACAO_SLOT = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_JANELA_ATENDIMENTO", x => x.ID_JANELA_ATENDIMENTO);
                    table.ForeignKey(
                        name: "FK_T_PB_JANELA_ATENDIMENTO_T_PB_VETERINARIO_ID_VETERINARIO",
                        column: x => x.ID_VETERINARIO,
                        principalTable: "T_PB_VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_CONSULTA",
                columns: table => new
                {
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TP_TIPO_CONSULTA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DH_DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ST_STATUS_CONSULTA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    EM_EMERGENCIA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PR_PRIORIDADE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    OB_OBSERVACAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    MT_MOTIVO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
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
                        name: "FK_T_PB_CONSULTA_T_PB_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "T_PB_CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_PB_CONSULTA_T_PB_VETERINARIO_ID_VETERINARIO",
                        column: x => x.ID_VETERINARIO,
                        principalTable: "T_PB_VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_PRONTUARIO",
                columns: table => new
                {
                    ID_PRONTUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    OB_ALERGIA = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    OB_OBSERVACOES = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AT_UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PB_PRONTUARIO", x => x.ID_PRONTUARIO);
                    table.ForeignKey(
                        name: "FK_T_PB_PRONTUARIO_T_PB_ANIMAL_ID_ANIMAL",
                        column: x => x.ID_ANIMAL,
                        principalTable: "T_PB_ANIMAL",
                        principalColumn: "ID_ANIMAL",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_REGISTRO_ATENDIMENTO",
                columns: table => new
                {
                    ID_REGISTRO_ATENDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DT_DATA_ATENDIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AN_ANAMNESE = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    DG_DIAGNOSTICO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    TR_TRATAMENTO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    OB_OBSERVACAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    PR_PROXIMO_RETORNO = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    PR_PROXIMA_VACINA = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_PRONTUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_T_PB_REGISTRO_ATENDIMENTO_T_PB_PRONTUARIO_ID_PRONTUARIO",
                        column: x => x.ID_PRONTUARIO,
                        principalTable: "T_PB_PRONTUARIO",
                        principalColumn: "ID_PRONTUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PB_PROCEDIMENTO",
                columns: table => new
                {
                    ID_PROCEDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TP_TIPO_PROCEDIMENTO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NM_NOME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    DS_DESCRICAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    ST_STATUS_PROCEDIMENTO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DT_DATA_PREVISTA_INICIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_DATA_PREVISTA_FIM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AN_ANEXOS_URL = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    OB_OBSERVACAO = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    ID_REGISTRO_ATENDIMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CA_CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_ANIMAL_ID_RESPONSAVEL",
                table: "T_PB_ANIMAL",
                column: "ID_RESPONSAVEL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_ANIMAL_ID_TIPO_ANIMAL",
                table: "T_PB_ANIMAL",
                column: "ID_TIPO_ANIMAL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CLINICA_ID_ENDERECO",
                table: "T_PB_CLINICA",
                column: "ID_ENDERECO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CONSULTA_ID_ANIMAL",
                table: "T_PB_CONSULTA",
                column: "ID_ANIMAL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CONSULTA_ID_CLINICA",
                table: "T_PB_CONSULTA",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_CONSULTA_ID_VETERINARIO",
                table: "T_PB_CONSULTA",
                column: "ID_VETERINARIO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_JANELA_ATENDIMENTO_ID_VETERINARIO",
                table: "T_PB_JANELA_ATENDIMENTO",
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
                name: "IX_T_PB_PRONTUARIO_ID_ANIMAL",
                table: "T_PB_PRONTUARIO",
                column: "ID_ANIMAL",
                unique: true);

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
                name: "IX_T_PB_REGISTRO_ATENDIMENTO_ID_PRONTUARIO",
                table: "T_PB_REGISTRO_ATENDIMENTO",
                column: "ID_PRONTUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_RESPONSAVEL_ID_CLINICA",
                table: "T_PB_RESPONSAVEL",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_RESPONSAVEL_ID_ENDERECO",
                table: "T_PB_RESPONSAVEL",
                column: "ID_ENDERECO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_VETERINARIO_ID_CLINICA",
                table: "T_PB_VETERINARIO",
                column: "ID_CLINICA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_PB_JANELA_ATENDIMENTO");

            migrationBuilder.DropTable(
                name: "T_PB_PROCEDIMENTO");

            migrationBuilder.DropTable(
                name: "T_PB_REGISTRO_ATENDIMENTO");

            migrationBuilder.DropTable(
                name: "T_PB_CONSULTA");

            migrationBuilder.DropTable(
                name: "T_PB_PRONTUARIO");

            migrationBuilder.DropTable(
                name: "T_PB_VETERINARIO");

            migrationBuilder.DropTable(
                name: "T_PB_ANIMAL");

            migrationBuilder.DropTable(
                name: "T_PB_RESPONSAVEL");

            migrationBuilder.DropTable(
                name: "T_PB_TIPO_ANIMAL");

            migrationBuilder.DropTable(
                name: "T_PB_CLINICA");

            migrationBuilder.DropTable(
                name: "T_PB_ENDERECO");
        }
    }
}
