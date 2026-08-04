using CitasMedicas.Web.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Doctors.ViewDoctorAvailability;

public class ViewDoctorAvailabilityService(AppDbContext dbContext)
{
    public async Task<ViewDoctorAvailabilityResult?> GetAsync(int doctorId, DateOnly date, CancellationToken cancellationToken)
    {
        var doctor = await dbContext.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.Specialty)
            .Include(doctor => doctor.Availabilities)
            .SingleOrDefaultAsync(doctor => doctor.Id == doctorId, cancellationToken);

        if (doctor is null)
        {
            return null;
        }

        var availabilities = doctor.Availabilities
            .Where(availability => availability.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday)
            .Where(availability => availability.DayOfWeek == date.DayOfWeek)
            .Where(availability => !dbContext.Appointments.Any(appointment =>
                appointment.DoctorId == doctorId &&
                appointment.Date == date &&
                appointment.StartTime == availability.StartTime &&
                appointment.EndTime == availability.EndTime))
            .OrderBy(availability => availability.DayOfWeek)
            .Select(availability => new AvailableSchedule(
                availability.Id,
                availability.DayOfWeek,
                availability.StartTime,
                availability.EndTime))
            .ToList();

        return new ViewDoctorAvailabilityResult(doctor.FullName, doctor.Specialty.Name, availabilities);
    }
}

public record ViewDoctorAvailabilityResult(
    string DoctorFullName,
    string SpecialtyName,
    IReadOnlyList<AvailableSchedule> Availabilities);

public record AvailableSchedule(int Id, DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);
