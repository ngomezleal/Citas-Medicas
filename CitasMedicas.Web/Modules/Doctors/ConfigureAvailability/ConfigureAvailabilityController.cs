using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Doctors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Doctors.ConfigureAvailability;

public class ConfigureAvailabilityController(AppDbContext dbContext, ConfigureAvailabilityService configureAvailabilityService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int doctorId, CancellationToken cancellationToken)
    {
        var doctor = await GetDoctorAsync(doctorId, cancellationToken);
        return doctor is null ? NotFound() : View(CreateViewModel(doctor));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([Bind(Prefix = "Availability")] ConfigureAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var doctor = await GetDoctorAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return NotFound();
        }

        var result = await configureAvailabilityService.ConfigureAsync(request, cancellationToken);
        if (result == ConfigureAvailabilityResult.InvalidSchedule)
        {
            ModelState.AddModelError(string.Empty, "Configure rangos únicos de lunes a viernes con una hora de inicio anterior a la hora de fin.");
            return View("Index", new ConfigureAvailabilityViewModel { Doctor = doctor, Availability = request });
        }

        TempData["SuccessMessage"] = "Los horarios del médico fueron configurados correctamente.";
        return RedirectToAction(nameof(Index), new { doctorId = request.DoctorId });
    }

    private async Task<Doctor?> GetDoctorAsync(int doctorId, CancellationToken cancellationToken) =>
        await dbContext.Doctors.AsNoTracking().Include(doctor => doctor.Specialty).Include(doctor => doctor.Availabilities)
            .SingleOrDefaultAsync(doctor => doctor.Id == doctorId, cancellationToken);

    private static ConfigureAvailabilityViewModel CreateViewModel(Doctor doctor)
    {
        var availabilityByDay = doctor.Availabilities.ToDictionary(availability => availability.DayOfWeek);
        return new ConfigureAvailabilityViewModel
        {
            Doctor = doctor,
            Availability = new ConfigureAvailabilityRequest
            {
                DoctorId = doctor.Id,
                Days = Enumerable.Range((int)DayOfWeek.Monday, 5).Select(dayNumber => (DayOfWeek)dayNumber)
                    .Select(day => availabilityByDay.TryGetValue(day, out var availability)
                        ? new AvailabilityDayRequest { DayOfWeek = day, IsAvailable = true, StartTime = availability.StartTime, EndTime = availability.EndTime }
                        : new AvailabilityDayRequest { DayOfWeek = day })
                    .ToList()
            }
        };
    }
}
