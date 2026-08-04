using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Appointments;
using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Doctors.ViewDoctorAvailability;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Tests;

public class ViewDoctorAvailabilityServiceTests
{
    [Fact]
    public async Task GetAsync_WithExistingDoctor_ReturnsSchedulesForSelectedWeekday()
    {
        await using var dbContext = CreateDbContext();
        var doctor = await AddDoctorAsync(dbContext);
        dbContext.DoctorAvailabilities.AddRange(
            Availability(doctor.Id, DayOfWeek.Friday, 13, 17),
            Availability(doctor.Id, DayOfWeek.Monday, 8, 12),
            Availability(doctor.Id, DayOfWeek.Saturday, 9, 11));
        await dbContext.SaveChangesAsync();

        var result = await new ViewDoctorAvailabilityService(dbContext).GetAsync(doctor.Id, new DateOnly(2026, 8, 3), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Dra. Ana López", result.DoctorFullName);
        var availability = Assert.Single(result.Availabilities);
        Assert.Equal(DayOfWeek.Monday, availability.DayOfWeek);
    }

    [Fact]
    public async Task GetAsync_WithUnknownDoctor_ReturnsNull()
    {
        await using var dbContext = CreateDbContext();

        var result = await new ViewDoctorAvailabilityService(dbContext).GetAsync(99, new DateOnly(2026, 8, 3), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_WithDoctorWithoutAvailabilities_ReturnsEmptyCollection()
    {
        await using var dbContext = CreateDbContext();
        var doctor = await AddDoctorAsync(dbContext);

        var result = await new ViewDoctorAvailabilityService(dbContext).GetAsync(doctor.Id, new DateOnly(2026, 8, 3), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Availabilities);
    }

    [Fact]
    public async Task GetAsync_ExcludesBookedScheduleOnlyForTheSelectedDate()
    {
        await using var dbContext = CreateDbContext();
        var doctor = await AddDoctorAsync(dbContext);
        var date = new DateOnly(2026, 8, 3);
        dbContext.DoctorAvailabilities.Add(Availability(doctor.Id, DayOfWeek.Monday, 8, 12));
        dbContext.Appointments.Add(new Appointment
        {
            DoctorId = doctor.Id,
            PatientName = "Paciente Uno",
            Date = date,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0)
        });
        await dbContext.SaveChangesAsync();

        var bookedDate = await new ViewDoctorAvailabilityService(dbContext).GetAsync(doctor.Id, date, CancellationToken.None);
        var nextMonday = await new ViewDoctorAvailabilityService(dbContext).GetAsync(doctor.Id, date.AddDays(7), CancellationToken.None);

        Assert.Empty(bookedDate!.Availabilities);
        Assert.Single(nextMonday!.Availabilities);
    }

    private static DoctorAvailability Availability(int doctorId, DayOfWeek dayOfWeek, int startHour, int endHour) => new()
    {
        DoctorId = doctorId,
        DayOfWeek = dayOfWeek,
        StartTime = new TimeOnly(startHour, 0),
        EndTime = new TimeOnly(endHour, 0)
    };

    private static AppDbContext CreateDbContext() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task<Doctor> AddDoctorAsync(AppDbContext dbContext)
    {
        dbContext.Specialties.Add(new Specialty { Id = 1, Name = "Medicina general" });
        var doctor = new Doctor { FullName = "Dra. Ana López", SpecialtyId = 1 };
        dbContext.Doctors.Add(doctor);
        await dbContext.SaveChangesAsync();
        return doctor;
    }
}
