using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data.Configurations
{
    public class OfertaConfiguration : IEntityTypeConfiguration<OfertaEntity>
    {
        public void Configure(EntityTypeBuilder<OfertaEntity> builder)
        {
            builder.ToTable("T_PB_OFERTA", tabela =>
            {
                tabela.HasCheckConstraint(
                    "CK_OFERTA_ATO",
                    "TP_ATO IN ('PROCEDIMENTO','CONSULTA','PROTOCOLO')");
                tabela.HasCheckConstraint(
                    "CK_OFERTA_ALVO",
                    "(TP_ATO IN ('PROCEDIMENTO','CONSULTA') AND TP_SUBTIPO IS NOT NULL AND ID_PROTOCOLO IS NULL) "
                    + "OR (TP_ATO = 'PROTOCOLO' AND ID_PROTOCOLO IS NOT NULL AND TP_SUBTIPO IS NULL)");
                tabela.HasCheckConstraint("CK_OFERTA_VALOR", "NR_VALOR >= 0");
            });

            builder.HasKey(oferta => oferta.Id)
                .HasName("PK_T_PB_OFERTA");

            builder.Property(oferta => oferta.Id)
                .HasColumnName("ID_OFERTA")
                .HasColumnType("NUMBER(10)")
                .ValueGeneratedOnAdd();

            // Sem FK: T_PB_CLINICA nao existe neste schema.
            builder.Property(oferta => oferta.ClinicaId)
                .HasColumnName("ID_CLINICA")
                .HasColumnType("NUMBER(10)")
                .IsRequired();

            builder.Property(oferta => oferta.Ato)
                .HasColumnName("TP_ATO")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(oferta => oferta.Subtipo)
                .HasColumnName("TP_SUBTIPO")
                .HasMaxLength(50);

            builder.Property(oferta => oferta.ProtocoloId)
                .HasColumnName("ID_PROTOCOLO")
                .HasColumnType("NUMBER(19)");

            builder.Property(oferta => oferta.Descricao)
                .HasColumnName("DS_DESCRICAO")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(oferta => oferta.Valor)
                .HasColumnName("NR_VALOR")
                .HasColumnType("NUMBER(10,2)")
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(oferta => oferta.InicioVigencia)
                .HasColumnName("DT_INICIO_VIGENCIA")
                .IsRequired();

            builder.Property(oferta => oferta.CreatedAt)
                .HasColumnName("CA_CREATED_AT")
                .IsRequired();

            builder.Property(oferta => oferta.UpdatedAt)
                .HasColumnName("AT_UPDATED_AT");

            // O DDL usa indice funcional com NVL; no Oracle a composta simples equivale.
            builder.HasIndex(oferta => new
            {
                oferta.ClinicaId,
                oferta.Ato,
                oferta.Subtipo,
                oferta.ProtocoloId,
                oferta.InicioVigencia
            })
                .HasDatabaseName("UX_OFERTA_VIGENCIA")
                .IsUnique()
                // Sem HasFilter(null) o EF filtra as colunas nulaveis e o indice cobre zero linhas.
                .HasFilter(null);

            builder.HasOne(oferta => oferta.Protocolo)
                .WithMany()
                .HasForeignKey(oferta => oferta.ProtocoloId)
                .HasConstraintName("FK_OFERTA_PROTOCOLO")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
