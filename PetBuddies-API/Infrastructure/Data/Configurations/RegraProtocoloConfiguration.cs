using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data.Configurations
{
    public class RegraProtocoloConfiguration : IEntityTypeConfiguration<RegraProtocoloEntity>
    {
        public void Configure(EntityTypeBuilder<RegraProtocoloEntity> builder)
        {
            builder.ToTable("T_PB_REGRA_PROTOCOLO", tabela =>
            {
                tabela.HasCheckConstraint(
                    "CK_REGPROT_TIPO",
                    "TP_TIPO_CUIDADO IN ('VACINACAO','VERMIFUGACAO','EXAME','RETORNO','CIRURGIA','MEDICACAO','HIGIENE')");
                tabela.HasCheckConstraint(
                    "CK_REGPROT_UNID_OFFSET",
                    "TP_UNIDADE_OFFSET IN ('DIAS','SEMANAS','MESES')");
                tabela.HasCheckConstraint(
                    "CK_REGPROT_ANCORA",
                    "TP_DATA_BASE IN ('NASCIMENTO','DATA_CIRURGIA','ULTIMA_REALIZACAO')");
                tabela.HasCheckConstraint(
                    "CK_REGPROT_UNID_INTERV",
                    "TP_UNIDADE_INTERVALO IN ('DIAS','SEMANAS','MESES')");
                tabela.HasCheckConstraint(
                    "CK_REGPROT_RECORRENCIA",
                    "(NR_INTERVALO IS NULL AND TP_UNIDADE_INTERVALO IS NULL) "
                    + "OR (NR_INTERVALO IS NOT NULL AND TP_UNIDADE_INTERVALO IS NOT NULL)");
                tabela.HasCheckConstraint("CK_REGPROT_REPETICOES", "NR_REPETICOES >= 1");
            });

            builder.HasKey(regra => regra.Id)
                .HasName("PK_T_PB_REGRA_PROTOCOLO");

            builder.Property(regra => regra.Id)
                .HasColumnName("ID_REGRA_PROTOCOLO")
                .HasColumnType("NUMBER(19)")
                .ValueGeneratedOnAdd();

            builder.Property(regra => regra.ProtocoloId)
                .HasColumnName("ID_PROTOCOLO")
                .HasColumnType("NUMBER(19)")
                .IsRequired();

            builder.Property(regra => regra.Tipo)
                .HasColumnName("TP_TIPO_CUIDADO")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(regra => regra.Nome)
                .HasColumnName("NM_NOME")
                .HasMaxLength(255)
                .IsRequired();

            // HasPrecision, e nao HasColumnType("NUMBER(4)"): com o tipo cru o provider
            // Oracle resolve o CLR de volta para byte.
            builder.Property(regra => regra.Offset)
                .HasColumnName("NR_OFFSET")
                .HasPrecision(4)
                .IsRequired();

            builder.Property(regra => regra.UnidadeOffset)
                .HasColumnName("TP_UNIDADE_OFFSET")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(regra => regra.DataBase)
                .HasColumnName("TP_DATA_BASE")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(regra => regra.Intervalo)
                .HasColumnName("NR_INTERVALO")
                .HasPrecision(4);

            builder.Property(regra => regra.UnidadeIntervalo)
                .HasColumnName("TP_UNIDADE_INTERVALO")
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(regra => regra.Repeticoes)
                .HasColumnName("NR_REPETICOES")
                .HasPrecision(3)
                .IsRequired();

            builder.Property(regra => regra.Descricao)
                .HasColumnName("DS_DESCRICAO")
                .HasMaxLength(2000);

            builder.HasOne(regra => regra.Protocolo)
                .WithMany(protocolo => protocolo.Regras)
                .HasForeignKey(regra => regra.ProtocoloId)
                .HasConstraintName("FK_REGPROT_PROTOCOLO")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
