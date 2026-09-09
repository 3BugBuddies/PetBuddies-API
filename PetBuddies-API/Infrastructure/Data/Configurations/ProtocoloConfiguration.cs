using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data.Configurations
{
    /// <summary>
    /// <c>T_PB_PROTOCOLO</c> (<c>01_ddl.sql:634</c>). Traduzida de <c>ProtocoloEntity.java</c>,
    /// com os nomes de coluna do ADR s3-24 §7b.
    /// </summary>
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

            // NUMBER(19), e nao NUMBER(10) como o resto do .NET: o Java guarda o
            // ID_PROTOCOLO como referencia solta em Long (ADR s3-25).
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

            // HasSentinel(true) e obrigatorio aqui, nao enfeite: com DEFAULT 1 numa coluna
            // bool nao nulavel, o EF nao distingue "false explicito" de "default do CLR" e
            // omite a coluna do INSERT — um protocolo cadastrado inativo nasceria ativo.
            // O sentinela diz qual valor significa "deixe o banco decidir".
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

            // A tabela nao tem AT_UPDATED_AT — divergencia deliberada em relacao ao resto
            // das entidades, que herdam as duas colunas da BaseEntity.
            builder.Ignore(protocolo => protocolo.UpdatedAt);
        }
    }
}
