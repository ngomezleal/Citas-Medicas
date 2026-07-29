using CitasMedicas.Web.Modules.Doctors;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctors");
            entity.HasKey(doctor => doctor.Id);
            entity.Property(doctor => doctor.FullName).HasMaxLength(200).IsRequired();
        });
    }
}
