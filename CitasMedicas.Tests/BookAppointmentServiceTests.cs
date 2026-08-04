using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Appointments;
using CitasMedicas.Web.Modules.Appointments.BookAppointment;
using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Tests;

public class BookAppointmentServiceTests
{
    [Fact]
    public async Task BookAsync_WithAvailableWeekdaySchedule_PersistsAppointment()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorWithMondayAvailabilityAsync(dbContext);

        var result = await new BookAppointmentService(dbContext).BookAsync(Request(doctor.Id, availability.Id), CancellationToken.None);

        Assert.Equal(BookAppointmentResult.Booked, result);
        var appointment = Assert.Single(await dbContext.Appointments.ToListAsync());
        Assert.Equal("Paciente Uno", appointment.PatientName);
        Assert.Equal(new DateOnly(2026, 8, 3), appointment.Date);
        Assert.Equal(new TimeOnly(8, 0), appointment.StartTime);
        Assert.Equal(new TimeOnly(12, 0), appointment.EndTime);
    }

    [Fact]
    public async Task BookAsync_WithUnknownDoctor_ReturnsDoctorNotFound()
    {
        await using var dbContext = CreateDbContext();

        var result = await new BookAppointmentService(dbContext).BookAsync(Request(99, 1), CancellationToken.None);

        Assert.Equal(BookAppointmentResult.DoctorNotFound, result);
    }

    [Fact]
    public async Task BookAsync_WithBlankPatientName_ReturnsInvalidPatientName()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorWithMondayAvailabilityAsync(dbContext);

        var result = await new BookAppointmentService(dbContext).BookAsync(new BookAppointmentRequest
        {
            DoctorId = doctor.Id,
            DoctorAvailabilityId = availability.Id,
            PatientName = " ",
            Date = new DateOnly(2026, 8, 3)
        }, CancellationToken.None);

        Assert.Equal(BookAppointmentResult.InvalidPatientName, result);
    }

    [Fact]
    public async Task BookAsync_WithWeekendDate_ReturnsInvalidDate()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorWithMondayAvailabilityAsync(dbContext);

        var request = Request(doctor.Id, availability.Id);
        request.Date = new DateOnly(2026, 8, 8);
        var result = await new BookAppointmentService(dbContext).BookAsync(request, CancellationToken.None);

        Assert.Equal(BookAppointmentResult.InvalidDate, result);
    }

    [Fact]
    public async Task BookAsync_WithScheduleFromAnotherDay_ReturnsScheduleUnavailable()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorWithMondayAvailabilityAsync(dbContext);

        var request = Request(doctor.Id, availability.Id);
        request.Date = new DateOnly(2026, 8, 4);
        var result = await new BookAppointmentService(dbContext).BookAsync(request, CancellationToken.None);

        Assert.Equal(BookAppointmentResult.ScheduleUnavailable, result);
    }

    [Fact]
    public async Task BookAsync_WithPreviouslyBookedSchedule_ReturnsScheduleAlreadyBooked()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorWithMondayAvailabilityAsync(dbContext);
        var service = new BookAppointmentService(dbContext);
        await service.BookAsync(Request(doctor.Id, availability.Id), CancellationToken.None);

        var result = await service.BookAsync(Request(doctor.Id, availability.Id), CancellationToken.None);

        Assert.Equal(BookAppointmentResult.ScheduleAlreadyBooked, result);
    }

    private static BookAppointmentRequest Request(int doctorId, int availabilityId) => new()
    {
        DoctorId = doctorId,
        DoctorAvailabilityId = availabilityId,
        PatientName = "Paciente Uno",
        Date = new DateOnly(2026, 8, 3)
    };

    private static AppDbContext CreateDbContext() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task<(Doctor Doctor, DoctorAvailability Availability)> AddDoctorWithMondayAvailabilityAsync(AppDbContext dbContext)
    {
        dbContext.Specialties.Add(new Specialty { Id = 1, Name = "Medicina general" });
        var doctor = new Doctor { FullName = "Dra. Ana López", SpecialtyId = 1 };
        dbContext.Doctors.Add(doctor);
        await dbContext.SaveChangesAsync();

        var availability = new DoctorAvailability
        {
            DoctorId = doctor.Id,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0)
        };
        dbContext.DoctorAvailabilities.Add(availability);
        await dbContext.SaveChangesAsync();

        return (doctor, availability);
    }
}
