using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace CitasMedicas.Web.Modules.Appointments.BookAppointment;

public class BookAppointmentController(BookAppointmentService bookAppointmentService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookAppointmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["BookingError"] = "Complete los datos de la reserva correctamente.";
            return RedirectToAvailability(request);
        }

        var response = await bookAppointmentService.BookAsync(request, cancellationToken);
        if (response.Result == BookAppointmentResult.DoctorNotFound)
            return NotFound();

        if (response.Result == BookAppointmentResult.Booked && response.Confirmation is not null)
        {
            var confirmation = response.Confirmation;
            var date = confirmation.Date.ToString("dddd, d 'de' MMMM 'de' yyyy", new CultureInfo("es-CO"));
            TempData["BookingSuccess"] = $"Reserva confirmada con {confirmation.DoctorFullName} para el {date} de {confirmation.StartTime:HH\\:mm} a {confirmation.EndTime:HH\\:mm}.";
        }
        else
        {
            TempData["BookingError"] = response.Result switch
            {
                BookAppointmentResult.InvalidPatientName => "El nombre del paciente es obligatorio.",
                BookAppointmentResult.InvalidDate => "Solo se pueden reservar citas de lunes a viernes.",
                BookAppointmentResult.ScheduleAlreadyBooked => "El horario seleccionado ya fue reservado.",
                _ => "El horario seleccionado no está disponible."
            };
        }

        return RedirectToAvailability(request);
    }

    private RedirectToActionResult RedirectToAvailability(BookAppointmentRequest request) =>
        RedirectToAction("Index", "ViewDoctorAvailability", new { doctorId = request.DoctorId, date = request.Date });
}
