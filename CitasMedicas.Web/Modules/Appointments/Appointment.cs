using CitasMedicas.Web.Modules.Doctors;

namespace CitasMedicas.Web.Modules.Appointments;

public class Appointment
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public string PatientName { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}
