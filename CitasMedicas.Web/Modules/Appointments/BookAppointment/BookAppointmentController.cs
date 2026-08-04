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

        var result = await bookAppointmentService.BookAsync(request, cancellationToken);
        if (result == BookAppointmentResult.DoctorNotFound)
        {
            return NotFound();
        }

        if (result != BookAppointmentResult.Booked)
        {
            TempData["BookingError"] = result switch
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
