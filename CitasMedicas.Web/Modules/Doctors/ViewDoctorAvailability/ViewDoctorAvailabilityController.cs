using Microsoft.AspNetCore.Mvc;

namespace CitasMedicas.Web.Modules.Doctors.ViewDoctorAvailability;

public class ViewDoctorAvailabilityController(ViewDoctorAvailabilityService viewDoctorAvailabilityService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int doctorId, DateOnly? date, CancellationToken cancellationToken)
    {
        var selectedDate = date ?? NextWeekday(DateOnly.FromDateTime(DateTime.Today));

        if (selectedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            TempData["DateWarning"] = "Las citas se reservan de lunes a viernes. Se seleccionó el próximo día hábil.";
            return RedirectToAction(nameof(Index), new { doctorId, date = NextWeekday(selectedDate) });
        }

        var availability = await viewDoctorAvailabilityService.GetAsync(doctorId, selectedDate, cancellationToken);
        return availability is null ? NotFound() : View(new ViewDoctorAvailabilityViewModel
        {
            DoctorId = doctorId,
            DoctorFullName = availability.DoctorFullName,
            SpecialtyName = availability.SpecialtyName,
            SelectedDate = selectedDate,
            Availabilities = availability.Availabilities
        });
    }

    private static DateOnly NextWeekday(DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Saturday => date.AddDays(2),
        DayOfWeek.Sunday => date.AddDays(1),
        _ => date
    };
}
