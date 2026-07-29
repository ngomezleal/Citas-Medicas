namespace CitasMedicas.Web.Modules.Doctors;

using CitasMedicas.Web.Modules.Specialties;

public class Doctor
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public int SpecialtyId { get; set; }

    public Specialty Specialty { get; set; } = null!;
}
