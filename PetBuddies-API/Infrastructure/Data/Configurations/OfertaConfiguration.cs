using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data.Configurations
{
    /// <summary>
    /// <c>T_PB_OFERTA</c> (<c>01_ddl.sql:466</c>).
    /// </summary>
    /// <remarks>
    /// <c>FK_OFERTA_CLINICA</c> do DDL <b>não</b> é declarada aqui: <c>T_PB_CLINICA</c> sai do
    /// .NET quando o Java absorver o registro (ADR s3-25), e o schema deste serviço termina com
    /// as quatro tabelas do back-office. <c>ID_CLINICA</c> fica como referência solta.
    /// </remarks>
    public class OfertaConfiguration : IEntityTypeConfiguration<OfertaEntity>
    {
        public void Configure(EntityTypeBuilder<OfertaEntity> builder)
        {
            builder.ToTable("T_PB_OFERTA", tabela =>
            {
                tabela.HasCheckConstraint(
                    "CK_OFERTA_ATO",
                    "TP_ATO IN ('PROCEDIMENTO','CONSULTA','PROTOCOLO')");
                // O alvo tem de casar com o tipo do ato: enum nos dois primeiros, linha no
                // terceiro, e nunca os dois nem nenhum.
                tabela.HasCheckConstraint(
                    "CK_OFERTA_ALVO",
                    "(TP_ATO IN ('PROCEDIMENTO','CONSULTA') AND TP_SUBTIPO IS NOT NULL AND ID_PROTOCOLO IS NULL) "
                    + "OR (TP_ATO = 'PROTOCOLO' AND ID_PROTOCOLO IS NOT NULL AND TP_SUBTIPO IS NULL)");
                // Preco negativo nao e desconto, e erro de digitacao.
                tabela.HasCheckConstraint("CK_OFERTA_VALOR", "NR_VALOR >= 0");
            });

            builder.HasKey(oferta => oferta.Id)
                .HasName("PK_T_PB_OFERTA");

            builder.Property(oferta => oferta.Id)
                .HasColumnName("ID_OFERTA")
                .HasColumnType("NUMBER(10)")
                .ValueGeneratedOnAdd();

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

            // Precisao explicita: sem ela o provider arredonda em silencio.
            builder.Property(oferta => oferta.Valor)
                .HasColumnName("NR_VALOR")
                .HasColumnType("NUMBER(10,2)")
                .HasPrecision(10, 2)
                .IsRequired();

            // DateOnly, convertida para DATE pela convencao global do N2 — vigencia nao e texto.
            builder.Property(oferta => oferta.InicioVigencia)
                .HasColumnName("DT_INICIO_VIGENCIA")
                .IsRequired();

            builder.Property(oferta => oferta.CreatedAt)
                .HasColumnName("CA_CREATED_AT")
                .IsRequired();

            builder.Property(oferta => oferta.UpdatedAt)
                .HasColumnName("AT_UPDATED_AT");

            // A chave natural da vigencia por sucessao. O DDL usa indice funcional com NVL; a
            // composta simples entrega a mesma garantia no Oracle, que conta chave
            // parcialmente nula para a unicidade (o proprio comentario do DDL diz isso).
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
                // Sem filtro: o EF filtraria as colunas nulaveis por padrao, e como uma das
                // duas do alvo e SEMPRE nula, o indice cobriria zero linhas.
                .HasFilter(null);

            builder.HasOne(oferta => oferta.Protocolo)
                .WithMany()
                .HasForeignKey(oferta => oferta.ProtocoloId)
                .HasConstraintName("FK_OFERTA_PROTOCOLO")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
