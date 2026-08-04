using System.ComponentModel.DataAnnotations;

namespace CitasMedicas.Web.Modules.Appointments.BookAppointment;

public class BookAppointmentRequest
{
    public int DoctorId { get; set; }

    public int DoctorAvailabilityId { get; set; }

    [Required(ErrorMessage = "El nombre del paciente es obligatorio.")]
    [StringLength(200)]
    public string PatientName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateOnly Date { get; set; }
}
