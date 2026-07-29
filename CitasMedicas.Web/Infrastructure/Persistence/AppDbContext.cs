using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Specialty> Specialties => Set<Specialty>();

    public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();

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

            entity.HasMany(doctor => doctor.Availabilities)
                .WithOne(availability => availability.Doctor)
                .HasForeignKey(availability => availability.DoctorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DoctorAvailability>(entity =>
        {
            entity.ToTable("DoctorAvailabilities");
            entity.HasKey(availability => availability.Id);
            entity.Property(availability => availability.DayOfWeek).IsRequired();
            entity.Property(availability => availability.StartTime).HasColumnType("time").IsRequired();
            entity.Property(availability => availability.EndTime).HasColumnType("time").IsRequired();
            entity.HasIndex(availability => new { availability.DoctorId, availability.DayOfWeek }).IsUnique();
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
