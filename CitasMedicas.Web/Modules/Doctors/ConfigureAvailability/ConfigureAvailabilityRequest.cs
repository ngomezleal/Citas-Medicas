using System.ComponentModel.DataAnnotations;

namespace CitasMedicas.Web.Modules.Doctors.ConfigureAvailability;

public class ConfigureAvailabilityRequest
{
    public int DoctorId { get; set; }

    public List<AvailabilityDayRequest> Days { get; set; } = [];
}

public class AvailabilityDayRequest
{
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsAvailable { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly? StartTime { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly? EndTime { get; set; }
}
