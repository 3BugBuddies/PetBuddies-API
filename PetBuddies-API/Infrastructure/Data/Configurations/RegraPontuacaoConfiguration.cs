using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data.Configurations
{
    /// <summary>
    /// <c>T_PB_REGRA_PONTUACAO</c> (<c>01_ddl.sql:499</c>).
    /// </summary>
    /// <remarks>
    /// <c>FK_PONTUACAO_CLINICA</c> do DDL não é declarada aqui, pelo mesmo motivo de
    /// <see cref="OfertaConfiguration"/>.
    /// </remarks>
    public class RegraPontuacaoConfiguration : IEntityTypeConfiguration<RegraPontuacaoEntity>
    {
        public void Configure(EntityTypeBuilder<RegraPontuacaoEntity> builder)
        {
            builder.ToTable("T_PB_REGRA_PONTUACAO", tabela =>
            {
                tabela.HasCheckConstraint(
                    "CK_PONTUACAO_GESTO",
                    "TP_GESTO IN ('PLANO_CRIADO','CONSULTA_AGENDADA','CONSULTA_REALIZADA','PROCEDIMENTO_EXECUTADO')");
                // Gesto que tira ponto nao existe no programa.
                tabela.HasCheckConstraint("CK_PONTUACAO_PONTOS", "NR_PONTOS > 0");
            });

            builder.HasKey(regra => regra.Id)
                .HasName("PK_T_PB_REGRA_PONTUACAO");

            builder.Property(regra => regra.Id)
                .HasColumnName("ID_REGRA_PONTUACAO")
                .HasColumnType("NUMBER(10)")
                .ValueGeneratedOnAdd();

            builder.Property(regra => regra.ClinicaId)
                .HasColumnName("ID_CLINICA")
                .HasColumnType("NUMBER(10)")
                .IsRequired();

            builder.Property(regra => regra.Gesto)
                .HasColumnName("TP_GESTO")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(regra => regra.Pontos)
                .HasColumnName("NR_PONTOS")
                .HasPrecision(5)
                .IsRequired();

            builder.Property(regra => regra.InicioVigencia)
                .HasColumnName("DT_INICIO_VIGENCIA")
                .IsRequired();

            builder.Property(regra => regra.CreatedAt)
                .HasColumnName("CA_CREATED_AT")
                .IsRequired();

            builder.Property(regra => regra.UpdatedAt)
                .HasColumnName("AT_UPDATED_AT");

            // Um valor por clinica, gesto e vigencia. Aqui a unicidade e simples: nenhuma
            // coluna da chave e nulavel.
            builder.HasIndex(regra => new { regra.ClinicaId, regra.Gesto, regra.InicioVigencia })
                .HasDatabaseName("UK_PONTUACAO_VIGENCIA")
                .IsUnique();
        }
    }
}
