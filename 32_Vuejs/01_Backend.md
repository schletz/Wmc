# Eine ASP.NET Core Webapi Backend für eine VueJS Applikation erstellen

## Anlegen der Projektstruktur

Um das Backend zu erzeugen, lege mit folgendem Skript im Repository die Struktur an. Ersetze
*SpengernewsProject* durch deinen Projektnamen

```
md SpengernewsProject
cd SpengernewsProject
md SpengernewsProject.Application
md SpengernewsProject.Webapi
cd SpengernewsProject.Application
dotnet new classlib
dotnet add package Microsoft.EntityFrameworkCore --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 6.*
dotnet add package Bogus --version 34.*
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.*
cd ..\SpengernewsProject.Webapi
dotnet new webapi
dotnet add reference ..\SpengernewsProject.Application
cd ..
dotnet new sln
dotnet sln add SpengernewsProject.Webapi
dotnet sln add SpengernewsProject.Application
npm init vue@3 SpengernewsProject.Client
```

Beim Anlegen des Vue Projektes (letzter Befehl) verwende die folgenden Einstellungen:

```
Vue.js - The Progressive JavaScript Framework

√ Project name: ... spengernewsProject-client
√ Add TypeScript? ... No
√ Add JSX Support? ... No
√ Add Vue Router for Single Page Application development? ... Yes
√ Add Pinia for state management? ... No
√ Add Vitest for Unit Testing? ... No
√ Add Cypress for both Unit and End-to-End testing? ... No
√ Add ESLint for code quality? ... Yes
√ Add Prettier for code formatting? ... No
```

Lege danach direkt im Verzeichnis deines Repositories eine Datei *.gitignore* an. Achte auf den
Punkt am Anfang des Namens!

**.gitignore**
```
**/bin
**/obj
**/.vs
**/.vscode
**/node_modules
*.db*
```

### Konfiguration der csproj Dateien

Konfiguriere das Application Projekt, indem du auf das Projekt in Visual Studio doppelklickst oder
die csproj Datei in einem Editor öffnest. Ersetze den Inhalt der Datei durch den folgenden Inhalt:

**SpengernewsProject.Application.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk">
	<PropertyGroup>
		<TargetFramework>net6.0</TargetFramework>
		<Nullable>enable</Nullable>
		<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Include="Bogus" Version="34.*" />	
		<PackageReference Include="Microsoft.EntityFrameworkCore" Version="6.*" />
		<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="6.*" />
		<PackageReference Include="Microsoft.EntityFrameworkCore.Proxies" Version="6.*" />
		<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.*" />
	</ItemGroup>
</Project>
```

Bei der ASP Projektdatei (Webapi) tausche **nur den Block PropertyGroup** durch den Inhalt des folgenden
Blocks **PropertyGroup**. Die Referenzen danach dürfen nicht verändert werden.

**SpengernewsProject.Webapi.csproj**
```xml
<PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

### Konfiguration von ASP.NET Core (Webapi)

Ersetze nun die Datei *appsettings.json* im Webapi Projekt durch den folgenden Inhalt. Ersetze
den Datenbanknamen durch einen passenderen Namen.

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "Sqlite": "DataSource=WerDiesenNamenVerwendetBekommtEinNichtGenuegend.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Ändere nun die launchsettings für ASP.NET Core. Der Server hört durch diese Konfiguration nun auf
Port 5000 (HTTP) und 5001 (HTTPS). Starte beim ersten Mal den Server in Visual Studio, sodass
ein Zertifikat generiert und als vertrauenswürdig installiert wird.

**Properties/launchsettings.json**
```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "webapi": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "",
      "applicationUrl": "http://localhost:5000;https://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

```

## Erste Schritte in ASP.NET Core

Lösche im Webapi Projekt den Mustercontroller (WeatherForecast) und alle Dateien, die mit
WeatherForecast beginnen. Ersetze danach die Datei *Program.cs* durch den folgenden Inhalt.
Eventuell sind einige *using* Anweisungen zu ergänzen.

**Program.cs**
```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
// SpengernewsContext ist der DbContext, der im Application Project angelegt wurde.
// Aktiviere diese Zeile, wenn du den DB Context definiert hat.
// builder.Services.AddDbContext<SpengernewsContext>(opt =>
//     opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

// Wir wollen automatisch nach Controllern im Ordner Controllers suchen.
builder.Services.AddControllers();
// Der Vue.JS Devserver läuft auf einem anderen Port, deswegen brauchen wir diese Konfiguration
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
}

// *************************************************************************************************
// APPLICATION
// *************************************************************************************************
var app = builder.Build();
// Leitet http auf https weiter (http Port 5000 auf https Port 5001)
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
// Im Development Mode erstellen wir bei jedem Serverstart die Datenbank neu.
// Aktiviere diese Zeilen, wenn du den DB Context erstellt hat.
//     using (var scope = app.Services.CreateScope())
//        using (var db = scope.ServiceProvider.GetRequiredService<SpengernewsContext>())
//        {
//            db.Database.EnsureDeleted();
//            db.Database.EnsureCreated();
//            db.Seed();  // TODO: Implementiere diese Methode im Datenbankcontext.
//        }
    app.UseCors();
}
// Liefert die statischen Dateien, die von VueJS generiert werden, aus.
app.UseStaticFiles();
// Bearbeitet die Routen, für die wir Controller geschrieben haben.
app.MapControllers();
// Wichtig für das clientseitige Routing, damit wir direkt an eine URL in der Client App steuern können.
app.MapFallbackToFile("index.html");
app.Run();

```

### Eine bat Datei für den Serverstart

Damit der Server bequemer gestartet werden kann, lege eine Datei *startServer.cmd* **im selben
Ordner wie die sln Datei** an. Die Pfade müssen natürlich angepasst werden:

**startServer.cmd**
```
rd /S /Q .vs 2> nul
rd /S /Q SpengernewsProject.Application/.vs 2> nul
rd /S /Q SpengernewsProject.Application/bin 2> nul
rd /S /Q SpengernewsProject.Application/obj 2> nul
rd /S /Q SpengernewsProject.Webapi/.vs 2> nul
rd /S /Q SpengernewsProject.Webapi/bin 2> nul
rd /S /Q SpengernewsProject.Webapi/obj 2> nul


cd SpengernewsProject.Webapi
:start
dotnet watch run -c Debug
goto start
```
