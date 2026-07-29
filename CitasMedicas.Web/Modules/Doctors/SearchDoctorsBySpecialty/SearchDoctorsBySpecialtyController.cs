using CitasMedicas.Web.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Doctors.SearchDoctorsBySpecialty;

public class SearchDoctorsBySpecialtyController(AppDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? specialtyId, CancellationToken cancellationToken)
    {
        var specialties = await dbContext.Specialties
            .AsNoTracking()
            .OrderBy(specialty => specialty.Name)
            .ToListAsync(cancellationToken);

        var viewModel = new SearchDoctorsBySpecialtyViewModel
        {
            SelectedSpecialtyId = specialtyId,
            Specialties = specialties
        };

        if (!specialtyId.HasValue)
        {
            return View(viewModel);
        }

        if (!specialties.Any(specialty => specialty.Id == specialtyId.Value))
        {
            ModelState.AddModelError(nameof(specialtyId), "La especialidad seleccionada no existe.");
            return View(viewModel);
        }

        viewModel.Doctors = await dbContext.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.Specialty)
            .Where(doctor => doctor.SpecialtyId == specialtyId.Value)
            .OrderBy(doctor => doctor.FullName)
            .ToListAsync(cancellationToken);

        return View(viewModel);
    }
}
