using CitasMedicas.Web.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Appointments.BookAppointment;

public class BookAppointmentService(AppDbContext dbContext)
{
    public async Task<BookAppointmentResult> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PatientName))
        {
            return BookAppointmentResult.InvalidPatientName;
        }

        if (request.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return BookAppointmentResult.InvalidDate;
        }

        if (!await dbContext.Doctors.AnyAsync(doctor => doctor.Id == request.DoctorId, cancellationToken))
        {
            return BookAppointmentResult.DoctorNotFound;
        }

        var availability = await dbContext.DoctorAvailabilities.SingleOrDefaultAsync(availability =>
            availability.Id == request.DoctorAvailabilityId &&
            availability.DoctorId == request.DoctorId &&
            availability.DayOfWeek == request.Date.DayOfWeek,
            cancellationToken);

        if (availability is null)
        {
            return BookAppointmentResult.ScheduleUnavailable;
        }

        var alreadyBooked = await dbContext.Appointments.AnyAsync(appointment =>
            appointment.DoctorId == request.DoctorId &&
            appointment.Date == request.Date &&
            appointment.StartTime == availability.StartTime &&
            appointment.EndTime == availability.EndTime,
            cancellationToken);

        if (alreadyBooked)
        {
            return BookAppointmentResult.ScheduleAlreadyBooked;
        }

        dbContext.Appointments.Add(new Appointment
        {
            DoctorId = request.DoctorId,
            PatientName = request.PatientName.Trim(),
            Date = request.Date,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime
        });

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return BookAppointmentResult.Booked;
        }
        catch (DbUpdateException)
        {
            return BookAppointmentResult.ScheduleAlreadyBooked;
        }
    }
}

public enum BookAppointmentResult
{
    Booked,
    DoctorNotFound,
    InvalidPatientName,
    InvalidDate,
    ScheduleUnavailable,
    ScheduleAlreadyBooked
}
