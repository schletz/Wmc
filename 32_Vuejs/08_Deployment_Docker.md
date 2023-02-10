# Vorbereitung für das Deployment: Erstellen eines Containers

## Orte der Konfiguration: environment variable, appsettings.json und appsettings.Development.json

### Was sind Umgebungsvariablen?

In einem Programm können wir klarerweise Variablen deklarieren und verwenden. Es gibt aber auch
eine Möglichkeit, im *Betriebssystem* (genauer: in der Shell) eine Variable zu definieren. Diese
kann dann im Programm abgerufen werden.

#### Umgebungsvariablen in Windows

In Windows werden Umgebungsvariablen mit dem *SET* Befehl in der Kommandozeile definiert. Beachte,
dass beim SET Befehl keine Leerstelle vor und nach dem Gleichheitszeichen stehen darf.

```
SET MYVAR=Hello World
echo %MYVAR%
echo %PATH%
```

Die Ausgabe ist:

```
Hello World
C:\Program Files (x86)\Microsoft SDKs\Azure\CLI2\wbin;C:\Python310\Scripts\;C:\Python310\;...
```

In C# können wir mit der Methode *GetEnvironmentVariable()* auf die Variable zugreifen.

```c#
var myvar = System.Environment.GetEnvironmentVariable("MYVAR");
Console.WriteLine(myvar);    // Hello World
```

#### Umgebungsvariablen in der Bash

Unter macOS, Linux oder in der Git Bash unter Windows gibt es natürlich auch einen solchen Mechanismus.

```bash
MYVAR_BASH="Hello World from Bash"
export MYVAR_BASH2="Hello World again from Bash"
echo $MYVAR_BASH
echo $MYVAR_BASH2
```

Die Ausgabe ist

```
Hello World from Bash
Hello World again from Bash
```

Da wir eine Leerstelle im String haben, müssen wir ihn unter Anführungszeichen setzen.
Die Variable *MYVAR_BASH2* wir mit dem Word *export* definiert. Was ist der Unterschied? Mit
*export* kann auch ein von der Bash gestarteter Prozess auf die Variable zugreifen. Wenn wir
in C# nun auf die Variablen zugreifen wollen, sehen wir den Unterschied:

```c#
var myvarBash = System.Environment.GetEnvironmentVariable("MYVAR_BASH");
var myvarBash2 = System.Environment.GetEnvironmentVariable("MYVAR_BASH2");
Console.WriteLine(myvarBash);     // (empty), myvarBash is null!
Console.WriteLine(myvarBash2);    // Hello World again from Bash
```

### Lesen der Konfiguration in ASP.NET Core

Wir haben in unserem Programmcode bereits mit einigen Funktionen gearbeitet, die z. B. den Connection
String aus der Datei *appsettings.json* laden:

```c#
var secret = builder.Configuration["Secret"];
```

*builder.Configuration* bzw. *app.Configuration* liest allerdings nicht nur aus der Datei
*appsettings.json*. Die Konfiguration wird aus mehreren Stellen geladen. Schreiben wir z. B.
`builder.Configuration["Secret"]` wird an verschiedenen Stellen nachgesehen:

- Gibt es einen Key *Secret* in *appsettings.json*?
- Gibt es einen Key *Secret* in *appsettings.(Environment).json*, z. B. *appsettings.Development.json*?
- Wurde eine *Umgebungsvariable* mit dem Namen *Secret* definiert?
- Andere Orte, siehe <small>https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0</small>.

Die Reihenfolge ist hier wichtig. Die letzte Fundstelle gewinnt. Wird also *Secret* in *appsettings.json*
definiert und gibt es auch eine *Umgebungsvariable* mit dem Namen *SECRET* (nicht case sensitive), wird
der Wert der *Umgebungsvariable* verwendet.

Folgendes Beispiel zeigt das Verhalten: Wir definieren in der Datei *appsettings.json* ein Secret:

```javascript
{ "Secret": "ABCD" }
```

Danach setzen wir in der Konsole eine Umgebungsvariable:

```
SET SECRET=Secret_from_environment_variable
```

Der folgende Code in der Datei *Program.cs* gibt den Wert der Umgebungsvariable aus:

```c#
var secret = builder.Configuration["Secret"];
Console.WriteLine(secret);   // Secret_from_environment_variable
```

#### Eine besondere Variable: ASPNETCORE_ENVIRONMENT

Wir haben bereits Code verwendet, der bestimmte Anweisungen nur im sogenannten *Development*
Mode ausführt. Sie wurden in ein if Statement geschrieben, das *Environment.IsDevelopment()*
verwendet:

```c#
if (app.Environment.IsDevelopment()) { /* ... */}
```

In ASP.NET Core gibt es sogenannte *Environments*. In der Datei *Properties/launchSettings.json*
findet sich ein entsprechender Eintrag:

```javascript
{
  "profiles": {
    "webapi": {
      // ...
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      // ...
    }
  }
}
```

Wenn wir mit *dotnet run* oder in Visual Studio den Webserver starten, ist also keine Magie dahinter,
dass der Server den Development Mode verwendet. Er ist in der Datei *Properties/launchSettings.json*
definiert. Erstellen wir ein Docker Image oder laden die Applikation direkt in die Cloud, wird diese
Datei nicht mit hochgeladen. Der Effekt ist dann, dass der Server in der Umgebung *Production*
läuft. So können wir diese Umgebungen unterscheiden.

#### appsettings.json und appsettings.Development.json

Bis jetzt haben wir alle Einstellungen in der Datei *appsettings.json* definiert und in das
Repository geladen. Das ist allerdings kritisch, wenn wir Connection Strings zu öffentlich
erreichbaren Datenbanken oder Secrets hinterlegen.

Wir gehen nun so vor: In der Datei *appsettings.json* wird die Grundkonfiguration hinterlegt:

**appsettings.json**

```javascript
{
  "ConnectionStrings": {
    "Default": ""
  },
  "Secret": "",
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

Nun legen wir eine Datei *appsettings.Development.json* an. Ist *ASPNETCORE_ENVIRONMENT*
auf *Development* gesetzt, werden die oberen Einstellungen durch die nachfolgenden Einstellungen
*überschrieben bzw. ergänzt*:

**appsettings.Development.json**

```javascript
{
  "ConnectionStrings": {
    "Default": "Server=127.0.0.1,11433;Initial Catalog=SpengernewsDb;User Id=sa;Password=SqlServer2019;TrustServerCertificate=true"
  },
  "Secret": "4UO5FmFW6wqj6PtWTXWRRiCvfdKq2dx+gsCM6d+eWR6++LrfKCP6jRvpMkw95KbYr9u1/VS1/fPWRg/XBmYjeQrR6knknq3w0TaDNOaU2QC8qP/CpTgdB5u3aHdIfpI1Tzn/5dx2fWYp0JCqYNhxzMDLGRifddA7JKUlhxVrx3E="
}
```

Diese Datei können wir nun in *.gitignore* ausschließen, sodass sie nicht mehr in das Repository
geladen wird.

> **Wichtig:** Schreibe den Eintrag *appsettings.Development.json* in die Datei *.gitignore* bevor du
> ein Commit machst. Ist diese Datei bereits unter Versionsverwaltung, werden Änderungen sonst
> übertragen!

## Erstellen eines Dockerfiles

Die Bedeutung der Datei *Dockerfile* ist einfach erklärt: Wenn du schon einmal Linux installiert
hast, beginnst du mit dem ISO Image in einer virtuellen Maschine oder einem USB Stick. Ist die
Installation beendet, wirst du vermutlich viele Befehle eingeben, die z. B. den Webserver, die
Datenbank, ... installieren.

Das Dockerfile kann man mit einer *gescripteten Installation* vergleichen. Es wird mit *FROM*
ein Grundimage geladen. Dann folgen *RUN* Anweisungen. Diese Anweisungen würdest du händisch
in die Shell eintippen, wenn du z. B. einen Server installieren willst.

Das Dockerfile für unsere ASP.NET Core Applikation hat folgenden Aufbau:

**Dockerfile**
```dockerfile
# Build container. Load full SDK as base image.
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# TODO: Adapt the directories!
COPY Spengernews.Application Spengernews.Application
COPY Spengernews.Webapi      Spengernews.Webapi

# Compile the app
RUN dotnet restore "Spengernews.Webapi"
RUN dotnet build   "Spengernews.Webapi" -c Release -o /app/build
RUN dotnet publish "Spengernews.Webapi" -c Release -o /app/publish /p:UseAppHost=false

# App container. Only needs runtime (smaller image)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
EXPOSE 80
EXPOSE 443
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Spengernews.Webapi.dll"]
```

Was hier passiert ist durch den Ablauf erkennbar. Zuerst wird ein Image mit der .NET 6 SDK geladen.
Dann werden mit *COPY* vom lokalen Rechner die Ordner mit der Applikation in den Container
kopiert. Danach wird das Programm kompiliert.

Die zweite *FROM* Anweisung verwendet nur die Runtime von .NET 6. Da wir die Applikation schon in
der vorigen Stage kompiliert haben, genügt die Runtime für die Ausführung. So etwas nennt sich
*multistage* und ermöglicht es, ein kleineres Image zu produzieren.

Die Pfade bei *COPY* werden relativ zur Datei *Dockerfile* gesehen. Lege daher die Datei *Dockerfile*
so an, dass sie im selben Verzeichnis wie die Ordner für Application und Webapi ist:

```
📁 (Your repo)
    ├── .gitignore
    ├── .dockerignore
    ├── Dockerfile
    ├── Spengernews.sln
    └──📂 Spengernews.Application
        └── Spengernews.Application.csproj
    └──📂 Spengernews.Webapi
        └── Spengernews.Webapi.csproj
```

Damit wir mit dem Befehl `COPY Spengernews.Application Spengernews.Application` nicht wahllos
alle Dateien kopieren, gibt es auch die Möglichkeit, eine Datei *.dockerignore* anzulegen. Sie kann
folgenden Aufbau haben:

**.dockerignore**

```
**/.vs
**/.vscode
**/bin
**/obj
**/appsettings.Development.json
```

### docker build: Erstellen des Images

Mit dem Befehl *docker build* kannst du ein Docker Image erstellen. Hier werden die Befehle abgearbeitet
und der Stand wird - wie ein "Snapshot" einer virtuellen Maschine, in ein sogenanntes *Image*
geschrieben. Der Parameter *-t* (Tag) gibt den Namen des Images an. Der Punkt bedeutet, dass im
lokalen Verzeichnis nach der Datei *Dockerfile* gesucht werden soll.

```
docker build -t spengernews_app .
```

Ein Docker Image ist allerdings noch kein laufender Container. Erst mit *docker create* wird
ein Container basierend auf einem Image erstellt. Der bekannte Befehl *docker run* ist nichts
anderes als ein *docker create* in Kombination mit *docker start*.

Nun kannst du mit *docker run* dein erstelltes Image starten. In Docker Desktop siehst du nun
einen Eintrag *spengernews_app*.

```
docker run -d -p 5000:80 --name spengernews_app spengernews_app
```

Der Container wird allerdings sofort gestoppt, da das Programm nicht starten kann. Wir haben nämlich
den Connection String in die Datei *appsettings.Development.json* geschrieben, die hier nicht
verarbeitet wird. Der Server im Container läuft nämlich im Production mode. Er wäre auch
nutzlos, da unter *localhost* im Container der App kein Datenbankserver zu erreichen ist.

## Testen des Containers im Production mode

Um das Problem zu lösen, verwenden wir ein kleines Shellscript für die Bash. **Starte dieses
Skript in der Git Bash und nicht in der Windows Kommandozeile.**

Es startet einen SQL Server Container und legt ein eigenes Netzwerk in Docker an. Danach geben
wir mit Hilfe der Umgebungsvariablen (Parameter *-e*) 2 Variablen mit:

- **CONNECTIONSTRINGS__DEFAULT:** Entspricht dem Eintrag `{ ConnectionStrings { Default: "..."} }`.
  Achte auf die zwei Underscores (__).
- **SECRET:** Entspricht dem Eintrag `{ Secret: "xxx" }`.

Prüfe vorher, ob in deiner Datei *appsettings.Development.json* die Namen auch wirklich so definiert
sind (*ConnectionStrings:Default* für den ConnectionString und *Secret*).

Du kannst mit den folgenden Befehlen eine Datei *start_container.sh* anlegen. Du kannst sie dann
**in der Git Bash** mit *./start_container.sh* ausführen. Die App ist dann im Browser unter
*http://localhost:5000* abrufbar.

**start_container.sh**

```bash
DOCKER_IMAGE=spengernews_webapp
SQL_IMAGE=spengernews_sqlserver
# Use INTERNAL port for the communication inside the docker network (1433 not 11433)
CONN_STR="Server=10.0.38.3,1433;Initial Catalog=SpengernewsDb;User Id=sa;Password=SqlServer2019;TrustServerCertificate=true"
# Generate random secret (the secret in appsettings.json is empty)
SECRET=$(dd if=/dev/random bs=128 count=1 2> /dev/null | base64)

# Cleanup
docker rm -f $DOCKER_IMAGE
docker rm -f $SQL_IMAGE
docker volume prune -f
docker image prune -f
docker network prune -f
docker network rm sqlserver_network

# Create a docker network.
docker network create --subnet=10.0.38.0/24 sqlserver_network
# Run SQL Server container with assigned ip in docker network.
docker run -d -p 11433:1433 --network=sqlserver_network --ip=10.0.38.3 --name $SQL_IMAGE \
    -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" \
    mcr.microsoft.com/azure-sql-edge:latest
# Build and run app container.
docker build -t $DOCKER_IMAGE . 
docker run -d -p 5000:80 --network=sqlserver_network --ip=10.0.38.2 --name $DOCKER_IMAGE \
    -e "CONNECTIONSTRINGS__DEFAULT=$CONN_STR" \
    -e "SECRET=$SECRET" \
    $DOCKER_IMAGE
```
