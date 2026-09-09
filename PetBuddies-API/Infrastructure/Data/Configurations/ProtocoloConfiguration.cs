using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data.Configurations
{
    public class ProtocoloConfiguration : IEntityTypeConfiguration<ProtocoloEntity>
    {
        public void Configure(EntityTypeBuilder<ProtocoloEntity> builder)
        {
            builder.ToTable("T_PB_PROTOCOLO", tabela =>
            {
                tabela.HasCheckConstraint(
                    "CK_PROTOCOLO_CATEGORIA",
                    "TP_CATEGORIA_PROTOCOLO IN ('PREVENTIVO','POS_CIRURGICO')");
                tabela.HasCheckConstraint(
                    "CK_PROTOCOLO_ESPECIE",
                    "ES_ESPECIE IN ('CACHORRO','GATO','PASSARO','COELHO','HAMSTER','OUTRO')");
                tabela.HasCheckConstraint("CK_PROTOCOLO_ATIVO", "AT_ATIVO IN (0,1)");
            });

            builder.HasKey(protocolo => protocolo.Id)
                .HasName("PK_T_PB_PROTOCOLO");

            builder.Property(protocolo => protocolo.Id)
                .HasColumnName("ID_PROTOCOLO")
                .HasColumnType("NUMBER(19)")
                .ValueGeneratedOnAdd();

            builder.Property(protocolo => protocolo.Nome)
                .HasColumnName("NM_NOME")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(protocolo => protocolo.Categoria)
                .HasColumnName("TP_CATEGORIA_PROTOCOLO")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(protocolo => protocolo.Especie)
                .HasColumnName("ES_ESPECIE")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            // Sem HasSentinel(true) o EF omite a coluna do INSERT e um protocolo
            // cadastrado inativo nasce ativo.
            builder.Property(protocolo => protocolo.Ativo)
                .HasColumnName("AT_ATIVO")
                .HasColumnType("NUMBER(1)")
                .HasDefaultValue(true)
                .HasSentinel(true)
                .IsRequired();

            builder.Property(protocolo => protocolo.Descricao)
                .HasColumnName("DS_DESCRICAO")
                .HasMaxLength(2000);

            builder.Property(protocolo => protocolo.CreatedAt)
                .HasColumnName("CA_CREATED_AT")
                .IsRequired();

            builder.Ignore(protocolo => protocolo.UpdatedAt);
        }
    }
}
