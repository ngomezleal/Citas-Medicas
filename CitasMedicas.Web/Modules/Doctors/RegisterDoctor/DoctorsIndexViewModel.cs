using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Specialties;

namespace CitasMedicas.Web.Modules.Doctors.RegisterDoctor;

public class DoctorsIndexViewModel
{
    public RegisterDoctorRequest NewDoctor { get; set; } = new();

    public IReadOnlyList<Doctor> Doctors { get; set; } = [];

    public IReadOnlyList<Specialty> Specialties { get; set; } = [];
}
