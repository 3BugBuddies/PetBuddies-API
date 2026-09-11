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

                    // DateOnly nao tem traducao nativa no provider Oracle: converte para DATE.
                    if (property.ClrType == typeof(DateOnly))
                    {
                        property.SetColumnType("DATE");
                        property.SetValueConverter(new DateOnlyConverter());
                    }
                }
            }

            // Depois do laço de propósito: a configuração explícita tem de vencer a convenção.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        // Back-office da clínica
        public DbSet<ProtocoloEntity> Protocolos { get; set; }
        public DbSet<RegraProtocoloEntity> RegrasProtocolo { get; set; }
        public DbSet<OfertaEntity> Ofertas { get; set; }
        public DbSet<RegraPontuacaoEntity> RegrasPontuacao { get; set; }
    }
}
