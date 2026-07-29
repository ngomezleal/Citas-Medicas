using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Doctors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Doctors.RegisterDoctor;

public class DoctorsController(AppDbContext dbContext, RegisterDoctorService registerDoctorService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var doctors = await GetDoctorsAsync(cancellationToken);
        return View(new DoctorsIndexViewModel { Doctors = doctors });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "NewDoctor")] RegisterDoctorRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", new DoctorsIndexViewModel
            {
                NewDoctor = request,
                Doctors = await GetDoctorsAsync(cancellationToken)
            });
        }

        await registerDoctorService.RegisterAsync(request, cancellationToken);
        TempData["SuccessMessage"] = "El médico fue registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<Doctor>> GetDoctorsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Doctors.AsNoTracking().OrderBy(doctor => doctor.FullName).ToListAsync(cancellationToken);
    }
}
