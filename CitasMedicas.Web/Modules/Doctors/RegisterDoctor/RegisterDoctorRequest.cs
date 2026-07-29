using System.ComponentModel.DataAnnotations;

namespace CitasMedicas.Web.Modules.Doctors.RegisterDoctor;

public class RegisterDoctorRequest
{
    [Display(Name = "Nombre completo")]
    [Required(ErrorMessage = "El nombre completo del médico es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre completo no puede superar los 200 caracteres.")]
    public string FullName { get; set; } = string.Empty;
}
