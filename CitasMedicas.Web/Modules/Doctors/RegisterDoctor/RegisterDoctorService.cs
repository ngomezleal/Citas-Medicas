using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Doctors;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Web.Modules.Doctors.RegisterDoctor;

public class RegisterDoctorService(AppDbContext dbContext)
{
    public async Task<bool> RegisterAsync(RegisterDoctorRequest request, CancellationToken cancellationToken)
    {
        var specialtyExists = await dbContext.Specialties
            .AnyAsync(specialty => specialty.Id == request.SpecialtyId, cancellationToken);

        if (!specialtyExists)
        {
            return false;
        }

        var doctor = new Doctor
        {
            FullName = request.FullName.Trim(),
            SpecialtyId = request.SpecialtyId
        };

        dbContext.Doctors.Add(doctor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
