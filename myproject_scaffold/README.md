# myproject (.NET 9, EF Core 9)

This scaffold contains four projects:

- `myproject.Core` — EF Core 9 DbContext, entities, and configurations (SQL Server).
- `myproject.Infrastructure` — Repository implementations / query layer.
- `myproject.Service` — Business logic services.
- `myproject.Web` — ASP.NET Core MVC app (controllers + views), DI wiring.

## Getting started

1. Ensure you have .NET 9 SDK installed.
2. Open a terminal in this folder:
   ```bash
   dotnet new sln -n myproject
   dotnet sln add ./myproject.Core/myproject.Core.csproj
   dotnet sln add ./myproject.Infrastructure/myproject.Infrastructure.csproj
   dotnet sln add ./myproject.Service/myproject.Service.csproj
   dotnet sln add ./myproject.Web/myproject.Web.csproj
   ```
3. Restore & build:
   ```bash
   dotnet restore
   dotnet build
   ```
4. Update the connection string in `myproject.Web/appsettings.Development.json` and/or `appsettings.json`.
5. Create the database & run EF migrations (optional, you can add migrations later):
   ```bash
   # from the solution root
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate -p ./myproject.Core -s ./myproject.Web
   dotnet ef database update -p ./myproject.Core -s ./myproject.Web
   ```
6. Run:
   ```bash
   dotnet run --project ./myproject.Web
   ```

> Note: In this layout, `DbContext` lives in **Core** as requested. Infrastructure contains repository/query implementations
> that depend on Core. Service depends on both Core and Infrastructure. Web depends on Service (and Core for DI wiring).
