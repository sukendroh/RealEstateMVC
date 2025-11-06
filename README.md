# README

## 1) Big-picture
- This is an ASP.NET Core MVC web app (TargetFramework: net9.0).
- Server-side components:
  - Controllers: `Controllers/` (MVC controllers use constructor DI for `RealEstateContext`).
  - Data layer: `Data/RealEstateContext.cs` — EF Core DbContext with DbSets for `Agent`, `License`, `AuditLog`.
  - Models: `Models/` — POCOs with DataAnnotations for validation (e.g. `Agent.cs`).
  - Views: `Views/` — Razor views use classic Html helpers (Html.BeginForm, Html.TextBoxFor, Validation helpers).
  - Static assets under `wwwroot/` and `lib/`.

## 2) How the app is wired
- `Program.cs` registers services and the DbContext using `builder.Configuration.GetConnectionString("DefaultConnection")`.
- Controllers call EF Core synchronously (e.g. `db.SaveChanges()`), and use `ModelState` for validation.
- Anti-forgery: POST actions use `[ValidateAntiForgeryToken]` and views include `@Html.AntiForgeryToken()`.
- Note: `Program.cs` calls `app.MapStaticAssets()` / `.WithStaticAssets()` — these are extension calls used by the static asset pipeline; if you need to update static wiring search generated files under `obj/` or the build output.

## 3) Build or run or debug
- Build locally: `dotnet build` (repo targets .NET 9). Use `dotnet run` to start.
- App uses `appsettings.json` for the connection: `ConnectionStrings:DefaultConnection` (currently points to local SQLExpress).
- There are no EF migrations checked in; to create migrations you likely need the EF Design package or `dotnet-ef` tool. If adding migrations, update the project or instruct the user.
- Use Visual Studio / VS Code launch profiles from `Properties/launchSettings.json` for debugging.

## 4) Project-specific conventions / patterns
- Synchronous EF usage: controllers call `db.SaveChanges()` and `db.Agents.ToList()` (no async/await). Follow this style when making minimal edits unless refactoring thoroughly.
- Validation: rely on DataAnnotations + `ModelState.IsValid` in controller POST handlers and server-side error display using `ValidationSummary`/`ValidationMessageFor` in views.
- Logging: controllers sometimes use `Console.WriteLine` rather than `ILogger<>`. When adding new logs, prefer `ILogger<T>` only if you follow-through and wire it in; otherwise be consistent with existing Console-based traces.
- Error handling: basic try/catch around DB operations with ModelState.AddModelError. Delete methods swallow exceptions silently — be cautious when changing behavior.

## 5) Common edit examples (concrete)
- Add a new property to `Agent`: update `Models/Agent.cs`, update `Views/Agent/Create.cshtml` and `Edit.cshtml`, update any `AgentController` binding logic if needed, and add a DB migration if the database must change.
- Change DB provider or connection: update `Program.cs` registration and `appsettings.json` connection string; remember to add required EF packages to `RealEstateMVC.csproj`.

## 6) Integration points / external deps
- EF Core SQL Server is used (`Microsoft.EntityFrameworkCore.SqlServer` in the csproj).
- Front-end validation references jQuery Validate via CDN inside views; static frontend code lives under `wwwroot/js` and `wwwroot/css`.

## 7) Where to look for more context
- `Program.cs`, `Data/RealEstateContext.cs`, `Controllers/AgentController.cs`, `Models/Agent.cs`, `Views/Agent/*.cshtml`, and `appsettings.json`.
- Generated/build-time artifacts in `obj/` and `bin/` may contain helpful wiring for static assets.
