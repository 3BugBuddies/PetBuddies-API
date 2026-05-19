using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Models;

namespace PetBuddies_API.Data
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

        public DbSet<AnimalEntity> Animais { get; set; }
        public DbSet<ClinicaEntity> Clinicas { get; set; }
        public DbSet<ConsultaEntity> Consultas { get; set; }
        public DbSet<EnderecoEntity> Enderecos { get; set; }
        public DbSet<JanelaAtendimentoEntity> JanelasAtendimento { get; set; }
        public DbSet<ProcedimentoEntity> Procedimentos { get; set; }
        public DbSet<ProntuarioEntity> Prontuarios { get; set; }
        public DbSet<RegistroAtendimentoEntity> RegistrosAtendimento { get; set; }
        public DbSet<ResponsavelEntity> Responsaveis { get; set; }
        public DbSet<TipoAnimalEntity> TiposAnimal { get; set; }
        public DbSet<VeterinarioEntity> Veterinarios { get; set; }
    }
}
