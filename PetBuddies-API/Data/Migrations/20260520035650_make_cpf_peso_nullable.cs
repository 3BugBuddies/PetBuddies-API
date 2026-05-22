using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBuddies_API.Migrations
{
    /// <inheritdoc />
    public partial class make_cpf_peso_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NR_CPF",
                table: "T_PB_RESPONSAVEL",
                type: "NVARCHAR2(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<decimal>(
                name: "NR_PESO",
                table: "T_PB_ANIMAL",
                type: "NUMBER(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "NUMBER(5,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NR_CPF",
                table: "T_PB_RESPONSAVEL",
                type: "NVARCHAR2(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NR_PESO",
                table: "T_PB_ANIMAL",
                type: "NUMBER(5,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "NUMBER(5,2)",
                oldNullable: true);
        }
    }
}
