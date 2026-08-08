using CitasMedicas.Web.Modules.Doctors.ViewDoctorAvailability;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CitasMedicas.Tests;

public class ViewDoctorAvailabilityControllerTests
{
    [Fact]
    public async Task Index_WithWeekendDate_RedirectsToNextWeekday()
    {
        var controller = new ViewDoctorAvailabilityController(null!)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), new TestTempDataProvider())
        };

        var result = await controller.Index(7, new DateOnly(2026, 8, 8), CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal(7, redirect.RouteValues!["doctorId"]);
        Assert.Equal(new DateOnly(2026, 8, 10), redirect.RouteValues["date"]);
        Assert.Equal("Las citas se reservan de lunes a viernes. Se seleccionó el próximo día hábil.", controller.TempData["DateWarning"]);
    }

    private sealed class TestTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
        }
    }
}
