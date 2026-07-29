using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Doctors.ConfigureAvailability;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Tests;

public class ConfigureAvailabilityServiceTests
{
    [Fact]
    public async Task ConfigureAsync_WithWeekdayRange_PersistsAvailability()
    {
        await using var dbContext = CreateDbContext();
        var doctor = await AddDoctorAsync(dbContext);

        var result = await new ConfigureAvailabilityService(dbContext).ConfigureAsync(new ConfigureAvailabilityRequest
        {
            DoctorId = doctor.Id,
            Days = [Day(DayOfWeek.Monday, 8, 12), Day(DayOfWeek.Friday, 13, 17)]
        }, CancellationToken.None);

        Assert.Equal(ConfigureAvailabilityResult.Saved, result);
        Assert.Equal(2, await dbContext.DoctorAvailabilities.CountAsync());
    }

    [Theory]
    [InlineData(DayOfWeek.Saturday, 8, 12)]
    [InlineData(DayOfWeek.Sunday, 8, 12)]
    [InlineData(DayOfWeek.Monday, 12, 12)]
    [InlineData(DayOfWeek.Monday, 13, 12)]
    public async Task ConfigureAsync_WithInvalidDayOrRange_DoesNotPersist(DayOfWeek day, int startHour, int endHour)
    {
        await using var dbContext = CreateDbContext();
        var doctor = await AddDoctorAsync(dbContext);

        var result = await new ConfigureAvailabilityService(dbContext).ConfigureAsync(new ConfigureAvailabilityRequest
        {
            DoctorId = doctor.Id,
            Days = [Day(day, startHour, endHour)]
        }, CancellationToken.None);

        Assert.Equal(ConfigureAvailabilityResult.InvalidSchedule, result);
        Assert.Empty(dbContext.DoctorAvailabilities);
    }

    [Fact]
    public async Task ConfigureAsync_WithDuplicateDay_DoesNotPersist()
    {
        await using var dbContext = CreateDbContext();
        var doctor = await AddDoctorAsync(dbContext);

        var result = await new ConfigureAvailabilityService(dbContext).ConfigureAsync(new ConfigureAvailabilityRequest
        {
            DoctorId = doctor.Id,
            Days = [Day(DayOfWeek.Monday, 8, 12), Day(DayOfWeek.Monday, 13, 17)]
        }, CancellationToken.None);

        Assert.Equal(ConfigureAvailabilityResult.InvalidSchedule, result);
        Assert.Empty(dbContext.DoctorAvailabilities);
    }

    [Fact]
    public async Task ConfigureAsync_ReplacesExistingSchedule()
    {
        await using var dbContext = CreateDbContext();
        var doctor = await AddDoctorAsync(dbContext);
        dbContext.DoctorAvailabilities.Add(new DoctorAvailability { DoctorId = doctor.Id, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0) });
        await dbContext.SaveChangesAsync();

        await new ConfigureAvailabilityService(dbContext).ConfigureAsync(new ConfigureAvailabilityRequest
        {
            DoctorId = doctor.Id,
            Days = [Day(DayOfWeek.Monday, 9, 13)]
        }, CancellationToken.None);

        var availability = Assert.Single(dbContext.DoctorAvailabilities);
        Assert.Equal(DayOfWeek.Monday, availability.DayOfWeek);
    }

    [Fact]
    public async Task ConfigureAsync_WithUnknownDoctor_DoesNotPersist()
    {
        await using var dbContext = CreateDbContext();

        var result = await new ConfigureAvailabilityService(dbContext).ConfigureAsync(new ConfigureAvailabilityRequest
        {
            DoctorId = 99,
            Days = [Day(DayOfWeek.Monday, 8, 12)]
        }, CancellationToken.None);

        Assert.Equal(ConfigureAvailabilityResult.DoctorNotFound, result);
        Assert.Empty(dbContext.DoctorAvailabilities);
    }

    private static AvailabilityDayRequest Day(DayOfWeek day, int startHour, int endHour) => new()
    {
        DayOfWeek = day,
        IsAvailable = true,
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
