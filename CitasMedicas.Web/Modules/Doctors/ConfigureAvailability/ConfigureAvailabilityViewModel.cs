using CitasMedicas.Web.Modules.Doctors;

namespace CitasMedicas.Web.Modules.Doctors.ConfigureAvailability;

public class ConfigureAvailabilityViewModel
{
    public Doctor Doctor { get; set; } = null!;
    public ConfigureAvailabilityRequest Availability { get; set; } = new();
}
