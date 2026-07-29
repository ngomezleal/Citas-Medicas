using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Specialty> Specialties => Set<Specialty>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctors");
            entity.HasKey(doctor => doctor.Id);
            entity.Property(doctor => doctor.FullName).HasMaxLength(200).IsRequired();
            entity.HasOne(doctor => doctor.Specialty)
                .WithMany(specialty => specialty.Doctors)
                .HasForeignKey(doctor => doctor.SpecialtyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.ToTable("Specialties");
            entity.HasKey(specialty => specialty.Id);
            entity.Property(specialty => specialty.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(specialty => specialty.Name).IsUnique();
            entity.HasData(
                new Specialty { Id = 1, Name = "Medicina general" },
                new Specialty { Id = 2, Name = "Cardiología" },
                new Specialty { Id = 3, Name = "Dermatología" },
                new Specialty { Id = 4, Name = "Pediatría" },
                new Specialty { Id = 5, Name = "Ginecología" },
                new Specialty { Id = 6, Name = "Odontología" });
        });
    }
}
