using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Doctors;

namespace CitasMedicas.Web.Modules.Doctors.RegisterDoctor;

public class RegisterDoctorService(AppDbContext dbContext)
{
    public async Task RegisterAsync(RegisterDoctorRequest request, CancellationToken cancellationToken)
    {
        var doctor = new Doctor { FullName = request.FullName.Trim() };

        dbContext.Doctors.Add(doctor);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
