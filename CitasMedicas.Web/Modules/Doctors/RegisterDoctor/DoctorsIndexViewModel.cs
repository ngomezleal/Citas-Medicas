using CitasMedicas.Web.Modules.Doctors;

namespace CitasMedicas.Web.Modules.Doctors.RegisterDoctor;

public class DoctorsIndexViewModel
{
    public RegisterDoctorRequest NewDoctor { get; set; } = new();

    public IReadOnlyList<Doctor> Doctors { get; set; } = [];
}
