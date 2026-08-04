using CitasMedicas.Web.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Appointments.ViewAppointments;

public class ViewAppointmentsService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<AppointmentListItem>> GetAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Appointments
            .AsNoTracking()
            .OrderBy(appointment => appointment.Date)
            .ThenBy(appointment => appointment.StartTime)
            .Select(appointment => new AppointmentListItem(
                appointment.PatientName,
                appointment.Doctor.FullName,
                appointment.Date,
                appointment.StartTime,
                appointment.EndTime))
            .ToListAsync(cancellationToken);
    }
}

public record AppointmentListItem(
    string PatientName,
    string DoctorFullName,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime);
