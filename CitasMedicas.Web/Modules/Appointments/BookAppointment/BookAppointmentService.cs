using CitasMedicas.Web.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Appointments.BookAppointment;

public class BookAppointmentService(AppDbContext dbContext)
{
    public async Task<BookAppointmentResponse> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PatientName))
            return new(BookAppointmentResult.InvalidPatientName);

        if (request.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return new(BookAppointmentResult.InvalidDate);

        var doctor = await dbContext.Doctors.SingleOrDefaultAsync(doctor => doctor.Id == request.DoctorId, cancellationToken);
        if (doctor is null)
            return new(BookAppointmentResult.DoctorNotFound);

        var availability = await dbContext.DoctorAvailabilities.SingleOrDefaultAsync(availability =>
            availability.Id == request.DoctorAvailabilityId &&
            availability.DoctorId == request.DoctorId &&
            availability.DayOfWeek == request.Date.DayOfWeek,
            cancellationToken);
        if (availability is null)
            return new(BookAppointmentResult.ScheduleUnavailable);

        var alreadyBooked = await dbContext.Appointments.AnyAsync(appointment =>
            appointment.DoctorId == request.DoctorId && appointment.Date == request.Date &&
            appointment.StartTime == availability.StartTime && appointment.EndTime == availability.EndTime,
            cancellationToken);
        if (alreadyBooked)
            return new(BookAppointmentResult.ScheduleAlreadyBooked);

        dbContext.Appointments.Add(new Appointment
        {
            DoctorId = doctor.Id,
            PatientName = request.PatientName.Trim(),
            Date = request.Date,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime
        });

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return new(BookAppointmentResult.Booked, new BookingConfirmation(doctor.FullName, request.Date, availability.StartTime, availability.EndTime));
        }
        catch (DbUpdateException)
        {
            return new(BookAppointmentResult.ScheduleAlreadyBooked);
        }
    }
}

public record BookAppointmentResponse(BookAppointmentResult Result, BookingConfirmation? Confirmation = null);
public record BookingConfirmation(string DoctorFullName, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime);

public enum BookAppointmentResult
{
    Booked,
    DoctorNotFound,
    InvalidPatientName,
    InvalidDate,
    ScheduleUnavailable,
    ScheduleAlreadyBooked
}
