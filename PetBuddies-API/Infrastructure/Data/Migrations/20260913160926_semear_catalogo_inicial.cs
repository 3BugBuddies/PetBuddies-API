using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBuddies_API.Infrastructure.Data.Migrations
{
    /// <summary>
    /// O catálogo nasce povoado. Sem isto, o motor do Java lê catálogo vazio como
    /// "nenhum protocolo compatível" — mesma resposta, sem erro e sem log — e o plano
    /// preventivo não nasce em banco novo.
    ///
    /// Espelha o papel do V2__seed_demonstracao.sql do lado Java.
    /// </summary>
    public partial class semear_catalogo_inicial : Migration
    {
        // A PK é ValueGeneratedOnAdd: a regra precisa do id do protocolo já na
        // inserção, então os ids vão explícitos — mesma razão do V2 do Java.
        private const long ProtocoloCao = 1L;
        private const long ProtocoloGato = 2L;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var nascidoEm = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "T_PB_PROTOCOLO",
                columns: new[] { "ID_PROTOCOLO", "NM_NOME", "TP_CATEGORIA_PROTOCOLO", "ES_ESPECIE", "AT_ATIVO", "DS_DESCRICAO", "CA_CREATED_AT" },
                values: new object[,]
                {
                    { ProtocoloCao, "Preventivo anual — cão", "PREVENTIVO", "CACHORRO", true,
                      "Vacinação, vermifugação e check-up de rotina para cães.", nascidoEm },
                    { ProtocoloGato, "Preventivo anual — gato", "PREVENTIVO", "GATO", true,
                      "Vacinação, vermifugação e check-up de rotina para gatos.", nascidoEm }
                });

            // As duas âncoras estão aqui de propósito: NASCIMENTO materializa item no
            // plano, ULTIMA_REALIZACAO não materializa nada e reaparece como sugestão.
            // Semear só a primeira deixaria metade do motor sem dado para exercitar.
            migrationBuilder.InsertData(
                table: "T_PB_REGRA_PROTOCOLO",
                columns: new[] { "ID_REGRA_PROTOCOLO", "ID_PROTOCOLO", "TP_TIPO_CUIDADO", "NM_NOME", "NR_OFFSET", "TP_UNIDADE_OFFSET", "TP_DATA_BASE", "NR_INTERVALO", "TP_UNIDADE_INTERVALO", "NR_REPETICOES", "DS_DESCRICAO" },
                values: new object[,]
                {
                    { 1L, ProtocoloCao, "VACINACAO", "Antirrábica", 12, "MESES", "NASCIMENTO", 12, "MESES", 5,
                      "Primeira dose aos 12 meses, reforço anual." },
                    { 2L, ProtocoloCao, "VERMIFUGACAO", "Vermífugo", 3, "MESES", "NASCIMENTO", 6, "MESES", 10,
                      "A cada seis meses a partir dos três meses de vida." },
                    { 3L, ProtocoloCao, "EXAME", "Hemograma de rotina", 12, "MESES", "ULTIMA_REALIZACAO", null, null, 1,
                      "Um ano depois do último hemograma." },

                    { 4L, ProtocoloGato, "VACINACAO", "Tríplice felina", 3, "MESES", "NASCIMENTO", 12, "MESES", 5,
                      "Primeira dose aos três meses, reforço anual." },
                    { 5L, ProtocoloGato, "VERMIFUGACAO", "Vermífugo", 2, "MESES", "NASCIMENTO", 6, "MESES", 10,
                      "A cada seis meses a partir dos dois meses de vida." },
                    { 6L, ProtocoloGato, "EXAME", "Hemograma de rotina", 12, "MESES", "ULTIMA_REALIZACAO", null, null, 1,
                      "Um ano depois do último hemograma." }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // A regra sai antes do protocolo: FK_REGRA_PROTOCOLO aponta para ele.
            for (long id = 1; id <= 6; id++)
            {
                migrationBuilder.DeleteData("T_PB_REGRA_PROTOCOLO", "ID_REGRA_PROTOCOLO", id);
            }

            migrationBuilder.DeleteData("T_PB_PROTOCOLO", "ID_PROTOCOLO", ProtocoloCao);
            migrationBuilder.DeleteData("T_PB_PROTOCOLO", "ID_PROTOCOLO", ProtocoloGato);
        }
    }
}
