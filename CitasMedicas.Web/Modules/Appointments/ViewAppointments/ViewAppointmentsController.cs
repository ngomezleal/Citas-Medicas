using Microsoft.AspNetCore.Mvc;

namespace CitasMedicas.Web.Modules.Appointments.ViewAppointments;

public class ViewAppointmentsController(ViewAppointmentsService viewAppointmentsService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var appointments = await viewAppointmentsService.GetAsync(cancellationToken);
        return View(new ViewAppointmentsViewModel { Appointments = appointments });
    }
}
