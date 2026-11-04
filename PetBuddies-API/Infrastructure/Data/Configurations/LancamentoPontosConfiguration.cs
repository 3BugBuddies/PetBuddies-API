using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data.Configurations
{
    public class LancamentoPontosConfiguration : IEntityTypeConfiguration<LancamentoPontosEntity>
    {
        public void Configure(EntityTypeBuilder<LancamentoPontosEntity> builder)
        {
            builder.ToTable("T_PB_LANCAMENTO_PONTOS", tabela =>
            {
                // Zero é válido: consulta sem regra vigente ainda gera o lançamento, e é
                // ele que impede a mesma consulta de pontuar duas vezes.
                tabela.HasCheckConstraint("CK_LANCAMENTO_PONTOS", "NR_PONTOS >= 0");
            });

            builder.HasKey(lancamento => lancamento.Id)
                .HasName("PK_T_PB_LANCAMENTO_PONTOS");

            builder.Property(lancamento => lancamento.Id)
                .HasColumnName("ID_LANCAMENTO_PONTOS")
                .HasColumnType("NUMBER(10)")
                .ValueGeneratedOnAdd();

            // Sem FK nos ids abaixo: consulta, responsável, animal e clínica vivem no schema do Java.
            builder.Property(lancamento => lancamento.ConsultaId)
                .HasColumnName("ID_CONSULTA")
                .HasColumnType("NUMBER(19)")
                .IsRequired();

            builder.Property(lancamento => lancamento.ResponsavelId)
                .HasColumnName("ID_RESPONSAVEL")
                .HasColumnType("NUMBER(19)")
                .IsRequired();

            builder.Property(lancamento => lancamento.AnimalId)
                .HasColumnName("ID_ANIMAL")
                .HasColumnType("NUMBER(19)")
                .IsRequired();

            builder.Property(lancamento => lancamento.ClinicaId)
                .HasColumnName("ID_CLINICA")
                .HasColumnType("NUMBER(10)")
                .IsRequired();

            builder.Property(lancamento => lancamento.DataConsulta)
                .HasColumnName("DH_CONSULTA")
                .IsRequired();

            builder.Property(lancamento => lancamento.Pontos)
                .HasColumnName("NR_PONTOS")
                .HasPrecision(6)
                .IsRequired();

            builder.Property(lancamento => lancamento.CreatedAt)
                .HasColumnName("CA_CREATED_AT")
                .IsRequired();

            builder.Property(lancamento => lancamento.UpdatedAt)
                .HasColumnName("AT_UPDATED_AT");

            builder.HasIndex(lancamento => lancamento.ConsultaId)
                .HasDatabaseName("UK_LANCAMENTO_CONSULTA")
                .IsUnique();

            builder.HasIndex(lancamento => lancamento.ResponsavelId)
                .HasDatabaseName("IX_LANCAMENTO_RESPONSAVEL");
        }
    }
}
