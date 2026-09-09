using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data.Configurations
{
    public class RegraPontuacaoConfiguration : IEntityTypeConfiguration<RegraPontuacaoEntity>
    {
        public void Configure(EntityTypeBuilder<RegraPontuacaoEntity> builder)
        {
            builder.ToTable("T_PB_REGRA_PONTUACAO", tabela =>
            {
                tabela.HasCheckConstraint(
                    "CK_PONTUACAO_GESTO",
                    "TP_GESTO IN ('PLANO_CRIADO','CONSULTA_AGENDADA','CONSULTA_REALIZADA','PROCEDIMENTO_EXECUTADO')");
                tabela.HasCheckConstraint("CK_PONTUACAO_PONTOS", "NR_PONTOS > 0");
            });

            builder.HasKey(regra => regra.Id)
                .HasName("PK_T_PB_REGRA_PONTUACAO");

            builder.Property(regra => regra.Id)
                .HasColumnName("ID_REGRA_PONTUACAO")
                .HasColumnType("NUMBER(10)")
                .ValueGeneratedOnAdd();

            // Sem FK: T_PB_CLINICA sai do .NET quando o Java absorver o registro.
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

            builder.HasIndex(regra => new { regra.ClinicaId, regra.Gesto, regra.InicioVigencia })
                .HasDatabaseName("UK_PONTUACAO_VIGENCIA")
                .IsUnique();
        }
    }
}
