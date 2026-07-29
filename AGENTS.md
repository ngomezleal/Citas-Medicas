# Repository Guidelines

## Instrucciones Generales
Nunca modificar la base de datos directamente. Debes hacerlo a traves de comandos de EF Core,
como crear una migracion y actualizar la base de datos via dicha migracion

## Project Structure & Architecture

`CitasMedicas.slnx` contains the .NET 10 solution. The MVC application lives in
`CitasMedicas.Web/`: `Program.cs` configures the application, `Views/` contains
Razor UI, `wwwroot/` holds static CSS and JavaScript, and `Properties/` holds
launch settings. Place new business code under `Modules/<Module>/<UseCase>/`;
`Infrastructure/` is for shared technical concerns such as persistence.

This is an ASP.NET Core MVC modular monolith backed by Entity Framework Core and
SQL Server. Keep each business flow as a vertical slice, for example
`Modules/Reservations/BookAppointment/BookAppointmentService.cs`. Do not add
projects, services, or architectural patterns unless the requirement calls for
them.

## Build, Test, and Development Commands

- `dotnet restore` - restore solution dependencies.
- `dotnet build CitasMedicas.slnx` - compile the solution and surface warnings.
- `dotnet run --project CitasMedicas.Web` - start the local MVC application.
- `dotnet test` - run all test projects once they are added.

Run `dotnet build` before submitting changes. The project currently has no test
project or configured formatter/linter; do not claim test coverage that does not
exist.

## Coding Style & Naming Conventions

Use the existing C# conventions: four-space indentation, file-scoped namespaces,
nullable reference types, and clear PascalCase names. Name entities in singular
form (`Doctor`, `Appointment`) and types after their use case
(`BookAppointmentRequest`, `BookAppointmentController`). Avoid vague names such
as `Helper`, `Manager`, or `CommonService`.

Controllers receive and validate requests, call a use-case service, and return a
response. Put business rules in the service and access EF Core through the shared
`AppDbContext`; do not introduce repositories, generic CRUD bases, or unit-of-work
wrappers.

## Testing Guidelines

When adding tests, create a dedicated `*.Tests` project, name test files after the
subject (for example, `BookAppointmentServiceTests.cs`), and name tests by expected
behavior. Cover acceptance criteria in `user-stories.md`, especially weekday-only
availability and prevention of double booking. Run `dotnet test` locally.

## Commits & Pull Requests

Recent history uses short imperative subjects (for example, `Add initial solution
file`). Keep commits focused and phrased the same way. Pull requests should explain
the user-story impact, list validation performed, link the relevant issue when one
exists, and include screenshots for UI changes. Preserve unrelated working-tree
changes.
