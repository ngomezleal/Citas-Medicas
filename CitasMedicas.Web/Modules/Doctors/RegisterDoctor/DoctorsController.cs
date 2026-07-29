using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Doctors.RegisterDoctor;

public class DoctorsController(AppDbContext dbContext, RegisterDoctorService registerDoctorService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await GetIndexViewModelAsync(new RegisterDoctorRequest(), cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "NewDoctor")] RegisterDoctorRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", await GetIndexViewModelAsync(request, cancellationToken));
        }

        var registered = await registerDoctorService.RegisterAsync(request, cancellationToken);
        if (!registered)
        {
            ModelState.AddModelError("NewDoctor.SpecialtyId", "La especialidad seleccionada no existe.");
            return View("Index", await GetIndexViewModelAsync(request, cancellationToken));
        }

        TempData["SuccessMessage"] = "El médico fue registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<Doctor>> GetDoctorsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.Specialty)
            .OrderBy(doctor => doctor.FullName)
            .ToListAsync(cancellationToken);
    }

    private async Task<DoctorsIndexViewModel> GetIndexViewModelAsync(RegisterDoctorRequest request, CancellationToken cancellationToken)
    {
        var specialties = await dbContext.Specialties
            .AsNoTracking()
            .OrderBy(specialty => specialty.Name)
            .ToListAsync(cancellationToken);

        return new DoctorsIndexViewModel
        {
            NewDoctor = request,
            Doctors = await GetDoctorsAsync(cancellationToken),
            Specialties = specialties
        };
    }
}
