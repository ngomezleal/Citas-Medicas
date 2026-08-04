namespace CitasMedicas.Web.Modules.Doctors.ViewDoctorAvailability;

public class ViewDoctorAvailabilityViewModel
{
    public int DoctorId { get; set; }

    public string DoctorFullName { get; set; } = string.Empty;

    public string SpecialtyName { get; set; } = string.Empty;

    public DateOnly SelectedDate { get; set; }

    public IReadOnlyList<AvailableSchedule> Availabilities { get; set; } = [];
}
