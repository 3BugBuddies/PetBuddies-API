using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Infrastructure.Data.Converters;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }



        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.CreatedAt = now;
                if (entry.State == EntityState.Modified)
                    entry.Entity.UpdatedAt = now;
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType.IsEnum)
                    {
                        property.SetProviderClrType(typeof(string));
                        property.SetMaxLength(50);
                    }

                    if (property.ClrType == typeof(bool) || property.ClrType == typeof(bool?))
                        property.SetColumnType("NUMBER(1)");

                    // DateOnly nao tem traducao nativa no provider Oracle: sem isto a coluna
                    // nasce NVARCHAR2(10) e a data vira texto. Convencao global em vez de
                    // atributo por coluna — a proxima coluna de data ja nasce certa.
                    if (property.ClrType == typeof(DateOnly) || property.ClrType == typeof(DateOnly?))
                    {
                        property.SetColumnType("DATE");
                        property.SetValueConverter(property.ClrType == typeof(DateOnly)
                            ? new DateOnlyConverter()
                            : new NullableDateOnlyConverter());
                    }
                }
            }

            ConfigurarDelecoes(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Comportamento de exclusão declarado, e não herdado da convenção.
        /// </summary>
        /// <remarks>
        /// O EF aplica <c>Cascade</c> por omissão em toda FK obrigatória, e é o que vale
        /// hoje para as onze relações antigas. Para as tabelas da Sprint 3 isso seria
        /// errado: apagar uma clínica levaria o catálogo junto, e apagar um catálogo
        /// levaria regras já assinadas. A régua aqui é <c>Restrict</c> em tudo que aponta
        /// para catálogo ou para ato assinado, e <c>Cascade</c> apenas do cabeçalho do
        /// check-in para o que foi extraído e apurado a partir dele — apagar um relato
        /// apaga a leitura daquele relato, nunca a prescrição.
        /// </remarks>
        private static void ConfigurarDelecoes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CondicaoClinicaEntity>()
                .HasOne(condicao => condicao.Clinica).WithMany(clinica => clinica.CondicoesClinicas)
                .HasForeignKey(condicao => condicao.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CondicaoClinicaEntity>()
                .HasOne(condicao => condicao.VeterinarioAutor).WithMany()
                .HasForeignKey(condicao => condicao.VeterinarioAutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrescricaoEntity>()
                .HasOne(prescricao => prescricao.Animal).WithMany(animal => animal.Prescricoes)
                .HasForeignKey(prescricao => prescricao.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrescricaoEntity>()
                .HasOne(prescricao => prescricao.Veterinario).WithMany()
                .HasForeignKey(prescricao => prescricao.VeterinarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrescricaoEntity>()
                .HasOne(prescricao => prescricao.RegistroAtendimento).WithMany()
                .HasForeignKey(prescricao => prescricao.RegistroAtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RegraPrescricaoEntity>()
                .HasOne(regra => regra.Prescricao).WithMany(prescricao => prescricao.Regras)
                .HasForeignKey(regra => regra.PrescricaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RegraPrescricaoEntity>()
                .HasOne(regra => regra.CondicaoClinica).WithMany()
                .HasForeignKey(regra => regra.CondicaoClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckinTratamentoEntity>()
                .HasOne(checkin => checkin.Animal).WithMany(animal => animal.Checkins)
                .HasForeignKey(checkin => checkin.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckinExtracaoEntity>()
                .HasOne(extracao => extracao.CheckinTratamento).WithMany(checkin => checkin.Extracoes)
                .HasForeignKey(extracao => extracao.CheckinTratamentoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CheckinExtracaoEntity>()
                .HasOne(extracao => extracao.CondicaoClinica).WithMany()
                .HasForeignKey(extracao => extracao.CondicaoClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckinResultadoEntity>()
                .HasOne(resultado => resultado.CheckinTratamento).WithMany(checkin => checkin.Resultados)
                .HasForeignKey(resultado => resultado.CheckinTratamentoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CheckinResultadoEntity>()
                .HasOne(resultado => resultado.Prescricao).WithMany()
                .HasForeignKey(resultado => resultado.PrescricaoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckinResultadoEntity>()
                .HasOne(resultado => resultado.RegraAplicada).WithMany()
                .HasForeignKey(resultado => resultado.RegraAplicadaId)
                .OnDelete(DeleteBehavior.SetNull);

            // A janela solta a reserva quando a consulta e excluida. Cancelamento e outro
            // caminho, e nao passa por aqui: quem cuida dele e o PR N8.
            modelBuilder.Entity<JanelaAtendimentoEntity>()
                .HasOne(janela => janela.Consulta).WithMany()
                .HasForeignKey(janela => janela.ConsultaId)
                .OnDelete(DeleteBehavior.SetNull);
        }



        public DbSet<AnimalEntity> Animais { get; set; }
        public DbSet<ClinicaEntity> Clinicas { get; set; }
        public DbSet<ConsultaEntity> Consultas { get; set; }
        public DbSet<JanelaAtendimentoEntity> JanelasAtendimento { get; set; }
        public DbSet<ProcedimentoEntity> Procedimentos { get; set; }
        public DbSet<RegistroAtendimentoEntity> RegistrosAtendimento { get; set; }
        public DbSet<ResponsavelEntity> Responsaveis { get; set; }
        public DbSet<VeterinarioEntity> Veterinarios { get; set; }

        public DbSet<PrescricaoEntity> Prescricoes { get; set; }
        public DbSet<RegraPrescricaoEntity> RegrasPrescricao { get; set; }
        public DbSet<CondicaoClinicaEntity> CondicoesClinicas { get; set; }
        public DbSet<CheckinTratamentoEntity> CheckinsTratamento { get; set; }
        public DbSet<CheckinExtracaoEntity> CheckinsExtracao { get; set; }
        public DbSet<CheckinResultadoEntity> CheckinsResultado { get; set; }
    }
}
