# Eine Datenbank mit EF Core erstellen

In der Datei *appsettings.json* haben wir bereits einen Connection String für eine Datenbank
konfiguriert:

```json
{
  "ConnectionStrings": {
    "Sqlite": "DataSource=wer_ohne_hirn_kopiert_ist_negativ.db"
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
Die Anweisung *"Microsoft.EntityFrameworkCore.Database.Command": "Warning"* ist neu hinzugekommen.
Sie verhindert, dass EF Core alle SQL Anweisungen über den Logger (das ist standardmäßig eine
Konsolenausgabe) ausgibt.

Nun wollen wir Modelklassen anlegen und die Datenbank beim Start des Servers erstellen. Dafür
erstelle im *Application Projekt* 2 Ordner: *Infrastructure* und *Model*. Erstelle im Ordner
*Infrastructure* eine Contextklasse. Für unsere Spengernews nennen wir sie SpengernewsContext:

```c#
public class SpengernewsContext : DbContext
{
    // TODO: Add your DbSets

    public SpengernewsContext(DbContextOptions<SpengernewsContext> opt): base(opt) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Additional config

        // Generic config for all entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // ON DELETE RESTRICT instead of ON DELETE CASCADE
            foreach (var key in entityType.GetForeignKeys())
                key.DeleteBehavior = DeleteBehavior.Restrict;

            foreach (var prop in entityType.GetDeclaredProperties())
            {
                // Define Guid as alternate key. The database can create a guid fou you.
                if (prop.Name == "Guid")
                {
                    modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                    prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                }
                // Default MaxLength of string Properties is 255.
                if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
                // Seconds with 3 fractional digits.
                if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
                if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
            }
        }

    }

    public void Seed()
    {
        // Seed logic.
    }
}
```

In der Methode *OnModelCreating()* sind einige Standardeinstellungen, die für alle Entities
gelten, definiert.

Erstelle nun im Ordner *Model* deine Modelklassen. Im Kapitel
[Code first](https://github.com/schletz/Pos3xhif/blob/master/03%20EF%20Core/02_CodeFirstEfCore5/README.md)
und [Enhanced code first](https://github.com/schletz/Pos3xhif/blob/master/03%20EF%20Core/03_EnhancedCodeFirst/README.md)
im Kurs *Pos3xhif* ist das Erstellen der Modelklassen erklärt.

## Registrieren der Datenbank in ASP.NET Core

Im Teil 1 wurde bereits die Datenbank als Service in *Program.cs* auskommentiert eingefügt:

```c#
// SpengernewsContext ist der DbContext, der im Application Project angelegt wurde.
// Aktiviere diese Zeile, wenn du den DB Context definiert hat.
// builder.Services.AddDbContext<SpengernewsContext>(opt =>
//     opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
```

Aktiviere nun diese Zeile und passe den Klassennamen (*SpengernewsContext*) an deine Klasse an.
Weiter unten in *Program.cs* ist eine etwas seltsame Logik auskommentiert:

```c#
// Im Development Mode erstellen wir bei jedem Serverstart die Datenbank neu.
// Aktiviere diese Zeilen, wenn du den DB Context erstellt hat.
//     using (var scope = app.Services.CreateScope())
//        using (var db = scope.ServiceProvider.GetRequiredService<SpengernewsContext>())
//        {
//            db.Database.EnsureDeleted();
//            db.Database.EnsureCreated();
//            db.Seed();  // TODO: Implementiere diese Methode im Datenbankcontext.
//        }
```

Was bedeuten diese Zeilen? Wenn der Server startet, soll EF Core aus den Modelklassen eine leere
Datenbank anlegen. Das funktioniert allerdings (ohne Migrations) nur, wenn die Datenbank leer ist.
Dafür wird mit *db.Database.EnsureDeleted()* die Datenbank gelöscht. Natürlich ist das im
Produktionsbetrieb keine Option, deswegen sind diese Zeilen auch in einer *if* Abfrage, dass diese
Logik nur im *Development* Modus aktiv ist.

Der *using* Block bedeutet, dass beim Starten über den Service Provider der Datenbank Context
angefordert wird. Durch diese Vorgehensweise kann die Datenbank zentral konfiguriert werden.

## Seeden der Datenbank

Es wird auch eine Methode *Seed()* aufgerufen, die im Moment noch leer ist. Zum Erstellen von
Musterdaten ist das NuGet Paket [Bogus](https://www.nuget.org/packages/Bogus) schon im Application
Projekt inkludiert.

Unsere Seed() Methode im SpengernewsContext könnte so aussehen:

```c#
public void Seed()
{
    string[] images = new string[]
    {
        "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA13UCsv.img?w=612&h=304&q=90&m=6&f=jpg&u=t",
        "https://www.bing.com/th?id=ORMS.c64be9536fb2ebb5673dfc61d8142abe&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0",
        "https://www.bing.com/th?id=ORMS.805cf20c3f313d9d74bf2cfc96fc7e00&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0",
        "https://www.bing.com/th?id=ORMS.430a52f4ed5a6e63b0a376680541e024&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0"
    };
    Randomizer.Seed = new Random(1039);    // (1)
    var faker = new Faker("de"); 

    var authors = new Faker<Author>("de").CustomInstantiator(f =>    // (2)
    {
        var lastname = f.Name.LastName();    // (3)
        return new Author(
            firstname: f.Name.FirstName(),
            lastname: lastname,
            email: $"{lastname.ToLower()}@spengergasse.at",
            phone: $"{+43}{f.Random.Int(1, 9)}{f.Random.String2(9, "0123456789")}".OrNull(f, 0.25f))    // (4)
        { Guid = f.Random.Guid() };   // (5)
    })
    .Generate(10)
    .GroupBy(a => a.Email).Select(g => g.First())   // (6)
    .ToList();
    Authors.AddRange(authors);    // (7)
    SaveChanges();                // (8)

    var categories = new Faker<Category>("de").CustomInstantiator(f =>
    {
        return new Category(f.Commerce.ProductAdjective())
        { Guid = f.Random.Guid() };
    })
    .Generate(10)
    .GroupBy(c => c.Name).Select(g => g.First())
    .ToList();
    Categories.AddRange(categories);
    SaveChanges();

    var articles = new Faker<Article>("de").CustomInstantiator(f =>
    {
        return new Article(
            headline: f.Lorem.Sentence(f.Random.Int(2, 4)),
            content: f.Lorem.Paragraphs(10,20),
            created: f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2022, 1, 1)),
            imageUrl: f.Random.ListItem(images),
            author: f.Random.ListItem(authors),    // (9)
            category: f.Random.ListItem(categories))
        { Guid = f.Random.Guid() };
    })
    .Generate(30)
    .ToList();
    Articles.AddRange(articles);
    SaveChanges();
}
```

- **(1)** Wir möchten bei jedem Programmstart die selben Werte generieren, um Fehler nachvollziehen
  zu können. Ein *fixes Seed* ermöglicht das. Du kannst jeden int Wert verwenden, in diesem Beispiel
  wurde einfach die Uhrzeit eingetragen.
- **(2)** Nun erstellen wir einen *Faker* für einen Entity Type. Der Parameter *de* generiert
  Namen, die auch Umlaute haben. Das ist zum Testen wichtig. Da unsere Modelklassen keinen
  Default Konstruktor haben, arbeiten wir mit *CustomInstantiator()*
- **(3)** Der Faker hat verschiedene Profile, die Daten aus verschiedenen Bereichen generieren können:
  Name, Lorem, Image, Finance, Address, Date, Company,
  Internet, Name, Rant, Vehicle, Music, Commerce, Phone, **Random**, Person,  Hacker. Das wichtigste
  Profil ist *Random*, der Zufallszahlen und zufällige Werte aus Listen und Enums generieren kann.
  Auf [der Bogus Projektseite](https://github.com/bchavez/Bogus#bogus-api-support) sind diese
  Profile genauer erklärt.
- **(4)** Bei *nullable* Feldern müssen wir auch eine gewisse Anzahl von *null* Werten generieren,
  um die Logik testen zu können. Du kannst den Anteil der *null* Werte als float Zahl angeben.
- **(5)** Unsere Modelklassen haben auch ein Property *Guid*, um nicht über den Auto Increment
  Wert zugreifen zu müssen. Beim Seeden wollen wir diesen Wert aber immer gleich vergeben, deswegen
  verwenden wir den Initializer. Die Guid ist nicht im Konstruktor der Modelklassen, da sie normalerweise
  von der Datenbank generiert wird.
- **(6)** Manche Felder haben ein *unique* Constraint. Der Zufallszahlengenerator arbeitet mit
  *ziehen mit zurücklegen*. Das bedeutet, dass Werte mehrmals vorkommen können. Wir gruppieren
  also nach diesem Wert und lassen uns pro Gruppe den ersten Wert zurückgeben. Die Anzahl wird
  dann zwar verringert, aber das ist in den meisten Fällen nicht wichtig.
- **(7)** Nun werden die generierten Werte, *die mit ToList() in den Speicher geschrieben wurden*,
  in die Datenbanktabelle eingefügt.
- **(8)** Erst mit *SaveChanges()* fügt EF Core die Werte in die physische Datenbank ein.
- **(9)** Um eine 1:n Beziehung seeden zu könenn, arbeiten wir mit *ListItem()*. Wir wählen einfach
  eine zufällige Kategorie und einen zufälligen Autor für unsere Zuordnung aus. *Wichtig: verwende
  die Speicherliste (authors, ...) und nicht die Datenbankliste (Authors, ...).*

## Verwenden von MariaDb statt SQLite

Wenn du Docker Desktop installiert hast, kannst du einfach einen Container für MariaDb laden.
Das root Password wird als *environment Variable* angegeben. Die Angabe des Ports geschieht
mit der Option *-p host_port:container_port*.

```
docker run --name mariadb -d -p 13306:3306 -e MARIADB_USER=root -e MARIADB_ROOT_PASSWORD=mariadb_root_password mariadb:10.10.2
```

Es wird der Port im Container (3306) auf den Port 13306 gelegt, um Konflikte mit einer vorhandenen
MySQL Datenbank zu vermeiden. Nun kannst du in der Datei *appsettings.json* unter *ConnectionStrings*
einen Wert für MySQL hinzufügen. Achte auf den richtigen Port und das richtige root Password. Es
muss deinen Angaben bei *docker run* entsprechen.

**appsettings.json**
```json
// ...
"ConnectionStrings": {
    "Sqlite": "DataSource=Spengernews.db",
    "MySql": "server=localhost;port=13306;database=wer_ohne_hirn_kopiert_ist_negativ;user=root;password=mariadb_root_password"
},
// ...
```

Danach musst du in der *csproj* Datei *des Application Projektes* den MySQL Treiber hinzufügen. MariaDb
ist protokollkompatibel, deswegen wird er auch für MariaDB verwendet.

```xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="6.*" />		
```

In der Datei *Program.cs* kannst du nun statt auf den SQLite auf den MySQL Connection String
referenzieren.

```c#
builder.Services.AddDbContext<SpengernewsContext>(opt =>
    opt.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        new MariaDbServerVersion("10.10.2")));
```

### Automatisches Starten des Servers und des Containers (Skript für den Lehrer)

Im Kapitel *Backend* wurde bereits eine Datei *startDevServer.cmd* im Verzeichnis der sln Datei angelegt.
Sie läuft in Endlosschleife, damit mit CTRL+C der Server neu kompiliert und gestartet werden kann.

Für *Lehrer*, die das Repo klonen und den Server starten wollen, brauchen wir andere Anforderungen:
- Der Container für die Datenbank - falls einer verwendet wird - muss geladen werden.
- Das Skript wird nicht in Endlosschleife ausgeführt, sondern startet 1x den Server.
- Die *startDevServer.cmd* im sln Ordner verwendet *dotnet watch run*, um bei Änderungen von Dateien neu
  zu kompilieren. Hier genügt ein normales *dotnet run*.

Lege eine Datei **startServer.cmd** (für Windows) und **startServer.sh** (für macOS/Linux)
*im Root Verzeichnis des Repositories an*.
*<your_container_name>* muss natürlich angepasst werden, z. B. auf *mariadb_employeemanager*.
Wenn du andere Ports oder ein anderes Datenbanksystem verwenden möchtest, muss *docker run* entsprechend
angepasst werden.

Ersetze dann *<relative_path_to_your_webapi>* durch den relativen Pfad zur WebAPI,
z. B. *EmployeeManager/EmployeeManager.WebAPI*

**startServer.cmd** (Windows)

```
docker rm -f <your_container_name> 2> nul
docker run --name <your_container_name> -d -p 13306:3306 -e MARIADB_USER=root -e MARIADB_ROOT_PASSWORD=mariadb_root_password mariadb:10.10.2
dotnet build <relative_path_to_your_webapi> --no-incremental --force
dotnet run -c Debug --project <relative_path_to_your_webapi>
```

**startServer.sh** (macOS, Linux)

```
docker rm -f mariadb_<yourAppName> &> /dev/null
docker run --name mariadb -d -p 13306:3306 -e MARIADB_USER=root -e MARIADB_ROOT_PASSWORD=mariadb_root_password mariadb:10.10.2
dotnet build <relative_path_to_your_webapi_with_slash> --no-incremental --force
dotnet watch run -c Debug --project <relative_path_to_your_webapi_with_slash>
```

Natürlich können die Dockerbefehle auch in der *startDevServer.cmd* Datei für die Entwicklung
(im Ordner der sln Datei) ergänzt werden.

## Verwenden einer anderen Datenbank für Production und Development

In der Datei Program.cs haben wir oft mit *IsDevelopment()* abgefragt, ob der Server im Development
Mode läuft. Unter *Properties/launchSettings.json* wird der Modus über *ASPNETCORE_ENVIRONMENT*
angegeben.

Wir können auch eine Datei *appsettings.(Profile).json* anlegen. Die Einstellungen dort ergänzen
bzw. überschreiben die Einstellungen von *appsettings.json*. Es ist also folgendes möglich:

**appsettings.json**
```json
// ...
"ConnectionStrings": {
    "MySql": "server=my_production_db;..."
},
// ...
```

**appsettings.Development.json**
```json
// ...
"ConnectionStrings": {
    "MySql": "server=localhost;port=13306;database=Spengernews;user=root;password=mariadb_root_password"
},
// ...
```

