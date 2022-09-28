# Projekt Spengernews Teil 2: Scoped Styles und minimal API

## Definition eigener Styles

Im Template sind im Ordner *src/assets* bereits CSS definitionen enthalten. Da wir von 0 weg beginnen
möchten, löschen wir alle CSS Dateien in diesem Ordner. *Wichtig:* In der Datei *main.js* wird
darauf verwiesen, daher muss das entsprechende import Statement ebenfalls entfernt werden.

### CSS in einer vue Component

Damit wir ein Layout mit 3 Zeilen (Überschrift, Imagepanel und Newsflash) erhalten, definieren
wir in der Datei [App.vue](spengernews/src/App.vue) einen *style* Bereich:
```html
<style>
html,
body {
    margin: 0;
    padding: 0;
    width: 100%;
    background-color: beige;
    font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}
#app {
    width: 100%;
    display: flex;
    flex-direction: column;
}
#header {
    flex: 0 0 4em;
}
#main {
    flex-grow: 1;
}
</style>

```

### scoped Styles

In der Komponente [NewsImage.vue](spengernews/src/components/NewsImage.vue) wollen wir den Text
über dem Bild platzieren. Dafür verwenden wir *scoped styles*. Wenn wir in einer Component z. B.
das Element img mit CSS Attributen definieren, würde sich diese Formatierung auf alle *img* Elemente
der ganzen Seite auswirken. Das möchten wir nicht, wir wollen dass unsere CSS Definition nur in dieser
Komponente gilt.

Im HTML Standard kann kein *style* Element innerhalb von *body* definiert werden. Deswegen wird bei
*scoped styles* automatisch ein data Attribut definiert, und die CSS Definitionen gelten nur für
Elemente mit dem generierten data Attribut der Komponente.

## Anlegen einer API

Wir möchten im nächsten Teil von einem Webserver Daten als JSON laden. Dafür brauchen wir ein erstes
Backend. Dafür erstellen wir ein Verzeichnis *webapi*. Mit dem Befehl 

```
dotnet new webapi
```

kann eine ASP.NET Core WebAPI angelegt werden. Nun kann die Datei *webapi.csproj* in Visual
Studio geöffnet werden. Tausche nun die Datei *Program.cs* mit einer
sogenannten Minimal API aus:

```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// SPA fragt nach /news/{id}
app.MapGet("/news/{id}", (int id) => new
{
    Url= "https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8",
    Headline=$"Headline zu {id}"
});

app.Run();
```

Im Ordner *Properties* gibt es die Datei *launchsettings.json*. Sie konfiguriert, welchen Port
der Webserver verwendet. Tausche die Datei durch folgenden Inhalt:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "webapi": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

