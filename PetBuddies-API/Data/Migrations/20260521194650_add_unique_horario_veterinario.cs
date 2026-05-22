using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBuddies_API.Migrations
{
    /// <inheritdoc />
    public partial class add_unique_horario_veterinario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DropIndex já executou antes da falha por duplicatas — Oracle auto-commita DDL
            migrationBuilder.CreateIndex(
                name: "IX_T_PB_JANELA_ATENDIMENTO_ID_VETERINARIO_DH_DATA_HORA_INICIO",
                table: "T_PB_JANELA_ATENDIMENTO",
                columns: new[] { "ID_VETERINARIO", "DH_DATA_HORA_INICIO" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_T_PB_JANELA_ATENDIMENTO_ID_VETERINARIO_DH_DATA_HORA_INICIO",
                table: "T_PB_JANELA_ATENDIMENTO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PB_JANELA_ATENDIMENTO_ID_VETERINARIO",
                table: "T_PB_JANELA_ATENDIMENTO",
                column: "ID_VETERINARIO");
        }
    }
}
