using CitasMedicas.Web.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Doctors.ConfigureAvailability;

public class ConfigureAvailabilityService(AppDbContext dbContext)
{
    public async Task<ConfigureAvailabilityResult> ConfigureAsync(ConfigureAvailabilityRequest request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Doctors.AnyAsync(doctor => doctor.Id == request.DoctorId, cancellationToken))
        {
            return ConfigureAvailabilityResult.DoctorNotFound;
        }

        if (!IsValidSchedule(request.Days))
        {
            return ConfigureAvailabilityResult.InvalidSchedule;
        }

        var existingAvailabilities = await dbContext.DoctorAvailabilities
            .Where(availability => availability.DoctorId == request.DoctorId)
            .ToListAsync(cancellationToken);

        dbContext.DoctorAvailabilities.RemoveRange(existingAvailabilities);
        dbContext.DoctorAvailabilities.AddRange(request.Days.Where(day => day.IsAvailable).Select(day => new DoctorAvailability
        {
            DoctorId = request.DoctorId,
            DayOfWeek = day.DayOfWeek,
            StartTime = day.StartTime!.Value,
            EndTime = day.EndTime!.Value
        }));

        await dbContext.SaveChangesAsync(cancellationToken);
        return ConfigureAvailabilityResult.Saved;
    }

    private static bool IsValidSchedule(IReadOnlyCollection<AvailabilityDayRequest> days)
    {
        if (days.Any(day => day.DayOfWeek is < DayOfWeek.Monday or > DayOfWeek.Friday) ||
            days.GroupBy(day => day.DayOfWeek).Any(group => group.Count() > 1))
        {
            return false;
        }

        return days.Where(day => day.IsAvailable).All(day =>
            day.StartTime.HasValue && day.EndTime.HasValue && day.StartTime.Value < day.EndTime.Value);
    }
}

public enum ConfigureAvailabilityResult
{
    Saved,
    DoctorNotFound,
    InvalidSchedule
}
