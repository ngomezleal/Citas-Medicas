namespace CitasMedicas.Web.Modules.Doctors.ViewDoctorAvailability;

public class ViewDoctorAvailabilityViewModel
{
    public string DoctorFullName { get; set; } = string.Empty;

    public string SpecialtyName { get; set; } = string.Empty;

    public IReadOnlyList<AvailableSchedule> Availabilities { get; set; } = [];
}
