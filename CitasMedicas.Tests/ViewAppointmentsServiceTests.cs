using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Appointments;
using CitasMedicas.Web.Modules.Appointments.ViewAppointments;
using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Tests;

public class ViewAppointmentsServiceTests
{
    [Fact]
    public async Task GetAsync_ReturnsAllAppointmentsWithTheirDetailsOrderedByDateAndTime()
    {
        await using var dbContext = CreateDbContext();
        var (firstDoctor, secondDoctor) = await AddDoctorsAsync(dbContext);
        dbContext.Appointments.AddRange(
            new Appointment
            {
                DoctorId = firstDoctor.Id,
                PatientName = "Paciente posterior",
                Date = new DateOnly(2026, 8, 5),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 0)
            },
            new Appointment
            {
                DoctorId = secondDoctor.Id,
                PatientName = "Paciente primero",
                Date = new DateOnly(2026, 8, 4),
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(9, 0)
            },
            new Appointment
            {
                DoctorId = firstDoctor.Id,
                PatientName = "Paciente segundo",
                Date = new DateOnly(2026, 8, 4),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(11, 0)
            });
        await dbContext.SaveChangesAsync();

        var appointments = await new ViewAppointmentsService(dbContext).GetAsync(CancellationToken.None);

        Assert.Collection(
            appointments,
            appointment =>
            {
                Assert.Equal("Paciente primero", appointment.PatientName);
                Assert.Equal("Dr. Bruno Díaz", appointment.DoctorFullName);
                Assert.Equal(new DateOnly(2026, 8, 4), appointment.Date);
                Assert.Equal(new TimeOnly(8, 0), appointment.StartTime);
                Assert.Equal(new TimeOnly(9, 0), appointment.EndTime);
            },
            appointment => Assert.Equal("Paciente segundo", appointment.PatientName),
            appointment => Assert.Equal("Paciente posterior", appointment.PatientName));
    }

    [Fact]
    public async Task GetAsync_WithNoAppointments_ReturnsEmptyList()
    {
        await using var dbContext = CreateDbContext();

        var appointments = await new ViewAppointmentsService(dbContext).GetAsync(CancellationToken.None);

        Assert.Empty(appointments);
    }

    private static AppDbContext CreateDbContext() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task<(Doctor FirstDoctor, Doctor SecondDoctor)> AddDoctorsAsync(AppDbContext dbContext)
    {
        dbContext.Specialties.Add(new Specialty { Id = 1, Name = "Medicina general" });
        var firstDoctor = new Doctor { FullName = "Dra. Ana López", SpecialtyId = 1 };
        var secondDoctor = new Doctor { FullName = "Dr. Bruno Díaz", SpecialtyId = 1 };
        dbContext.Doctors.AddRange(firstDoctor, secondDoctor);
        await dbContext.SaveChangesAsync();

        return (firstDoctor, secondDoctor);
    }
}
