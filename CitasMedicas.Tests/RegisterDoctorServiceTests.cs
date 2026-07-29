using CitasMedicas.Web.Infrastructure.Persistence;
using CitasMedicas.Web.Modules.Doctors.RegisterDoctor;
using CitasMedicas.Web.Modules.Doctors.SearchDoctorsBySpecialty;
using CitasMedicas.Web.Modules.Specialties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.Tests;

public class RegisterDoctorServiceTests
{
    [Fact]
    public async Task RegisterAsync_WithExistingSpecialty_PersistsDoctorAndAssociation()
    {
        await using var dbContext = CreateDbContext();
        await SeedSpecialtiesAsync(dbContext);
        var service = new RegisterDoctorService(dbContext);

        var registered = await service.RegisterAsync(new RegisterDoctorRequest
        {
            FullName = "Dra. Ana López",
            SpecialtyId = 2
        }, CancellationToken.None);

        Assert.True(registered);
        var doctor = await dbContext.Doctors.SingleAsync();
        Assert.Equal("Dra. Ana López", doctor.FullName);
        Assert.Equal(2, doctor.SpecialtyId);
    }

    [Fact]
    public async Task RegisterAsync_WithoutSpecialty_DoesNotPersistDoctor()
    {
        await using var dbContext = CreateDbContext();
        await SeedSpecialtiesAsync(dbContext);
        var service = new RegisterDoctorService(dbContext);

        var registered = await service.RegisterAsync(new RegisterDoctorRequest
        {
            FullName = "Dra. Ana López",
            SpecialtyId = 0
        }, CancellationToken.None);

        Assert.False(registered);
        Assert.Empty(dbContext.Doctors);
    }

    [Fact]
    public async Task RegisterAsync_WithUnknownSpecialty_DoesNotPersistDoctor()
    {
        await using var dbContext = CreateDbContext();
        await SeedSpecialtiesAsync(dbContext);
        var service = new RegisterDoctorService(dbContext);

        var registered = await service.RegisterAsync(new RegisterDoctorRequest
        {
            FullName = "Dra. Ana López",
            SpecialtyId = 99
        }, CancellationToken.None);

        Assert.False(registered);
        Assert.Empty(dbContext.Doctors);
    }

    [Fact]
    public async Task Search_WithSelectedSpecialty_ReturnsOnlyAssociatedDoctors()
    {
        await using var dbContext = CreateDbContext();
        await SeedSpecialtiesAsync(dbContext);
        dbContext.Doctors.AddRange(
            new() { FullName = "Dra. Ana López", SpecialtyId = 1 },
            new() { FullName = "Dr. Carlos Ruiz", SpecialtyId = 2 });
        await dbContext.SaveChangesAsync();
        var controller = new SearchDoctorsBySpecialtyController(dbContext);

        var result = await controller.Index(1, CancellationToken.None);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<SearchDoctorsBySpecialtyViewModel>(viewResult.Model);
        var doctor = Assert.Single(model.Doctors);
        Assert.Equal("Dra. Ana López", doctor.FullName);
    }

    [Fact]
    public async Task Search_WithoutAssociatedDoctors_ReturnsEmptyResults()
    {
        await using var dbContext = CreateDbContext();
        await SeedSpecialtiesAsync(dbContext);
        var controller = new SearchDoctorsBySpecialtyController(dbContext);

        var result = await controller.Index(2, CancellationToken.None);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<SearchDoctorsBySpecialtyViewModel>(viewResult.Model);
        Assert.Empty(model.Doctors);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task SeedSpecialtiesAsync(AppDbContext dbContext)
    {
        dbContext.Specialties.AddRange(
            new Specialty { Id = 1, Name = "Medicina general" },
            new Specialty { Id = 2, Name = "Cardiología" });
        await dbContext.SaveChangesAsync();
    }
}
