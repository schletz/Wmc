# Datenbank container mit .NET automatisch starten

Für .NET gibt es das Paket [Docker.DotNet](https://www.nuget.org/packages/Docker.DotNet), mit dem
sich Docker steuern lässt. Konkret wird über die API mit dem Docker Daemon kommuniziert.

Wenn eine Applikation wie unsere ASP.NET Core Webapp Abhängigkeiten zu anderen Containern hat,
kann man mit dem Befehl [docker-compose](https://docs.docker.com/compose/compose-file/) und einer
Konfigurationsdatei den Applikations- und den Datenbankcontainer gleichzeitig starten.

Zum Testen (in der WebAPI oder bei Unittests) kann es aber hilfreich sein, mit dem Serverstart
den entsprechenden Datenbankcontainer zu laden. Die Datei
[WebApplicationDockerExtensions.cs](Spengernews.Webapi/WebApplicationDockerExtensions.cs)
implementiert eine sogenannte *extension Methode* für die *WebApplication* Klasse. Die 
Klasse WebApplication ist unter dem Instanznamen *app* in der Datei *Program.cs* jedem vertraut.

Da wir diese Klasse erweitern, können wir nun folgenden Aufruf in unsere Datei *Program.cs* des
WebApi Projektes schreiben:

**Program.cs**
```c#
var app = builder.Build();
// Other code
if (app.Environment.IsDevelopment())
{
    // We will create a fresh sql server container in development mode. For performance reasons,
    // you can disable deleteAfterShutdown because in development mode the database is deleted
    // before it is generated.
    try
    {
        await app.UseSqlServerContainer(
            containerName: "spengernews_sqlserver", version: "latest",
            connectionString: app.Configuration.GetConnectionString("Default"),
            deleteAfterShutdown: true);
    }
    catch (Exception e)
    {
        app.Logger.LogError(e.Message);
        return;
    }
}
```

Die Methode *UseSqlServerContainer* ist in [WebApplicationDockerExtensions.cs](Spengernews.Webapi/WebApplicationDockerExtensions.cs)
implementiert. Sie lädt bei Bedarf das SQL Server Image und wartet, bis der Server bereit für
Verbindungen ist. Letzteres ist wichtig, da nach dem Starten des Containers noch Initialisierungsskripts
laufen. Verbinden wir uns zu früh, lehnt SQL Server die Verbindung ab.

In [Spengernews.Webapi/Program.cs](Spengernews.Webapi/Program.cs) wird diese Methode verwendet.
