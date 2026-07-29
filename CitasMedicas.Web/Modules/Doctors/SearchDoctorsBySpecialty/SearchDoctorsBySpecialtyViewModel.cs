using CitasMedicas.Web.Modules.Doctors;
using CitasMedicas.Web.Modules.Specialties;

namespace CitasMedicas.Web.Modules.Doctors.SearchDoctorsBySpecialty;

public class SearchDoctorsBySpecialtyViewModel
{
    public int? SelectedSpecialtyId { get; set; }

    public IReadOnlyList<Specialty> Specialties { get; set; } = [];

    public IReadOnlyList<Doctor> Doctors { get; set; } = [];
}
