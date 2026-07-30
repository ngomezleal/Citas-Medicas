using Microsoft.AspNetCore.Mvc;

namespace CitasMedicas.Web.Modules.Doctors.ViewDoctorAvailability;

public class ViewDoctorAvailabilityController(ViewDoctorAvailabilityService viewDoctorAvailabilityService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int doctorId, CancellationToken cancellationToken)
    {
        var availability = await viewDoctorAvailabilityService.GetAsync(doctorId, cancellationToken);
        return availability is null ? NotFound() : View(new ViewDoctorAvailabilityViewModel
        {
            DoctorFullName = availability.DoctorFullName,
            SpecialtyName = availability.SpecialtyName,
            Availabilities = availability.Availabilities
        });
    }
}
