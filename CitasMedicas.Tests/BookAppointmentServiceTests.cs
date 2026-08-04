using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Appointments.BookAppointment;
using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Tests;

public class BookAppointmentServiceTests
{
    [Fact]
    public async Task BookAsync_WithAvailableWeekday_PersistsAppointmentAndReturnsConfirmation()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorAndAvailabilityAsync(dbContext);

        var response = await new BookAppointmentService(dbContext).BookAsync(Request(doctor.Id, availability.Id), CancellationToken.None);

        Assert.Equal(BookAppointmentResult.Booked, response.Result);
        Assert.Equal(doctor.FullName, response.Confirmation!.DoctorFullName);
        Assert.Equal(new DateOnly(2026, 8, 3), response.Confirmation.Date);
        Assert.Equal(new TimeOnly(8, 0), response.Confirmation.StartTime);
        Assert.Single(dbContext.Appointments);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task BookAsync_WithMissingPatientName_DoesNotPersist(string patientName)
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorAndAvailabilityAsync(dbContext);

        var response = await new BookAppointmentService(dbContext).BookAsync(Request(doctor.Id, availability.Id, patientName), CancellationToken.None);

        Assert.Equal(BookAppointmentResult.InvalidPatientName, response.Result);
        Assert.Empty(dbContext.Appointments);
    }

    [Fact]
    public async Task BookAsync_WithWeekend_DoesNotPersist()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorAndAvailabilityAsync(dbContext);
        var request = Request(doctor.Id, availability.Id);
        request.Date = new DateOnly(2026, 8, 2);

        var response = await new BookAppointmentService(dbContext).BookAsync(request, CancellationToken.None);

        Assert.Equal(BookAppointmentResult.InvalidDate, response.Result);
        Assert.Empty(dbContext.Appointments);
    }

    [Fact]
    public async Task BookAsync_WithAlreadyBookedSchedule_DoesNotPersistAgain()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, availability) = await AddDoctorAndAvailabilityAsync(dbContext);
        var service = new BookAppointmentService(dbContext);
        await service.BookAsync(Request(doctor.Id, availability.Id), CancellationToken.None);

        var response = await service.BookAsync(Request(doctor.Id, availability.Id, "Carlos Ruiz"), CancellationToken.None);

        Assert.Equal(BookAppointmentResult.ScheduleAlreadyBooked, response.Result);
        Assert.Single(dbContext.Appointments);
    }

    [Fact]
    public async Task BookAsync_WithAvailabilityFromAnotherDoctor_DoesNotPersist()
    {
        await using var dbContext = CreateDbContext();
        var (doctor, _) = await AddDoctorAndAvailabilityAsync(dbContext);
        var (_, otherAvailability) = await AddDoctorAndAvailabilityAsync(dbContext, "Dr. Luis Gomez");

        var response = await new BookAppointmentService(dbContext).BookAsync(Request(doctor.Id, otherAvailability.Id), CancellationToken.None);

        Assert.Equal(BookAppointmentResult.ScheduleUnavailable, response.Result);
        Assert.Empty(dbContext.Appointments);
    }

    private static BookAppointmentRequest Request(int doctorId, int availabilityId, string patientName = "Ana Perez") => new()
    {
        DoctorId = doctorId,
        DoctorAvailabilityId = availabilityId,
        PatientName = patientName,
        Date = new DateOnly(2026, 8, 3)
    };

    private static AppDbContext CreateDbContext() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task<(Doctor Doctor, DoctorAvailability Availability)> AddDoctorAndAvailabilityAsync(AppDbContext dbContext, string name = "Dra. Ana Lopez")
    {
        if (!await dbContext.Specialties.AnyAsync())
            dbContext.Specialties.Add(new Specialty { Id = 1, Name = "Medicina general" });

        var doctor = new Doctor { FullName = name, SpecialtyId = 1 };
        dbContext.Doctors.Add(doctor);
        await dbContext.SaveChangesAsync();
        var availability = new DoctorAvailability { DoctorId = doctor.Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0) };
        dbContext.DoctorAvailabilities.Add(availability);
        await dbContext.SaveChangesAsync();
        return (doctor, availability);
    }
}
