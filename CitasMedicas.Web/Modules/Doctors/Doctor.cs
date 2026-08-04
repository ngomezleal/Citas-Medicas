namespace CitasMedicas.Web.Modules.Doctors;

using CitasMedicas.Web.Modules.Appointments;
using CitasMedicas.Web.Modules.Specialties;
using CitasMedicas.Web.Modules.Appointments;

public class Doctor
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public int SpecialtyId { get; set; }

    public Specialty Specialty { get; set; } = null!;

    public ICollection<DoctorAvailability> Availabilities { get; set; } = [];

    public ICollection<Appointment> Appointments { get; set; } = [];
}
