# Eine Datenbank mit EF Core erstellen

In der Datei *appsettings.json* haben wir bereits einen Connection String für eine Datenbank
konfiguriert:

```json
{
  "ConnectionStrings": {
    "Default": "DataSource=wer_ohne_hirn_kopiert_ist_negativ.db"
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
    /// <summary>
    /// Initialize the database with some values (holidays, ...).
    /// Unlike Seed, this method is also called in production.
    /// </summary>
    private void Initialize()
    {
        // Seed logic.
    }
    /// <summary>
    /// Generates random values for testing the application. This method is only called in development mode.
    /// </summary>    
    private void Seed()
    {
        // Seed logic.
    }
    /// <summary>
    /// Creates the database. Called once at application startup.
    /// </summary>    
    public void CreateDatabase(bool isDevelopment)
    {
        if (isDevelopment) { Database.EnsureDeleted(); }
        // EnsureCreated only creates the model if the database does not exist or it has no
        // tables. Returns true if the schema was created.  Returns false if there are
        // existing tables in the database. This avoids initializing multiple times.
        if (Database.EnsureCreated()) { Initialize(); }
        if (isDevelopment) Seed();
    }        
}
```

In der Methode *OnModelCreating()* sind einige Standardeinstellungen, die für alle Entities
gelten, definiert. Die Methode *CreateDatabase()* erstellt die Datanbank automatisch nach den
definierten Modelklassen. Sie ist sozusagen ein großer *CREATE TABLE* Generator. Die Methode
hat folgenden Aufbau:

- Im *Development Mode* (wird durch die Variable *ASPNETCORE_ENVIRONMENT* in
  Properties/launchSettings.json festgelegt) löschen wir eine bestehende Datenbank. EF Core legt
  nämlich nur die Tabellen an, wenn die Datenbank leer ist. Das ist natürlich brutal, deswegen
  führen wir diesen Schritt nur im Development Mode durch. Für Szenarien, wo ein Modell in
  Production geändert werden soll, gibt es *Migrations*.
- *EnsureCreated()* wird immer ausgeführt. Falls schon Tabellen in der Datenbank sind, macht diese
  Methode einfach gar nichts. Legt die Methode Tabellen an, liefert sie true zurück. Dann rufen
  wir die Methode *Initialize()* auf. Sie hat den Sinn, fixe Werte, die wir auch in Produktion schon
  in der Datenbank haben wollen, einzutragen.
- Nur im Development Mode rufen wir *Seed()* auf. Sie generiert Musterdaten, die das Testen der App
  erleichtern soll. In Produktion brauchen wir diese Methode natürlich nicht.


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
//     opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));
```

Aktiviere nun diese Zeile und passe den Klassennamen (*SpengernewsContext*) an deine Klasse an.
Weiter unten in *Program.cs* ist eine etwas seltsame Logik auskommentiert:

```c#
// Im Development Mode erstellen wir bei jedem Serverstart die Datenbank neu.
// Aktiviere diese Zeilen, wenn du den DB Context erstellt hat.
// using (var scope = app.Services.CreateScope())
// {
//     using (var db = scope.ServiceProvider.GetRequiredService<SpengernewsContext>())
//     {
//         db.CreateDatabase(isDevelopment: app.Environment.IsDevelopment());
//     }
// }
```

Der *using* Block bedeutet, dass beim Starten über den Service Provider der Datenbank Context
angefordert wird. Durch diese Vorgehensweise kann die Datenbank zentral konfiguriert werden.
Ob der Server im Development Mode läuft, erfragen wir über *app.Environment.IsDevelopment()*.
So können wir zwischen lokalem Aufrufen und dem Ausführen in Produktionsumgebungen unterscheiden.

## Seeden der Datenbank

Wie vorher erklärt werden in *CreateDatabase()* die Methoden *Initialize()* und *Seed()* aufgerufen.
Zum Erstellen von Musterdaten ist das NuGet Paket [Bogus](https://www.nuget.org/packages/Bogus)
schon im Application Projekt inkludiert.

Unsere *Initialize()* Methode könnte so aussehen. Sie schreibt einen Admin User in die Datenbank und
legt 3 fixe Kategorien an. **Beachte:** Dieser Code wird auch in Produktion beim erstmaligen Starten
der Applikation ausgeführt. Der "erste Admin" wird später durch eine Umgebungsvariable übergeben.
Schreibe in Produktion niemals Kennwörter in den Code!

```c#
private void Initialize()
{
    var author = new Author(
            firstname: "Max",
            lastname: "Mustermann",
            email: "mustermann@spengergasse.at",
            username: "admin",
            initialPassword: "1111",
            phone: "+4369912345678");
    Authors.Add(author);
    SaveChanges();

    var categories = new Category[]{
        new Category("Wissenschaft"),
        new Category("Kultur"),
        new Category("Sport")
    };
    Categories.AddRange(categories);
    SaveChanges();
}
```
Unsere Seed() Methode im SpengernewsContext könnte so aussehen:

```c#
public void Seed()
{

    Randomizer.Seed = new Random(1039);    // (1)
    var faker = new Faker("de");

    var authors = new Faker<Author>("de").CustomInstantiator(f =>    // (2)
    {
        var lastname = f.Name.LastName();   // (3)
        return new Author(
            firstname: f.Name.FirstName(),
            lastname: lastname,
            email: $"{lastname.ToLower()}@spengergasse.at",
            username: lastname.ToLower(),
            initialPassword: "1111",
            phone: $"{+43}{f.Random.Int(1, 9)}{f.Random.String2(9, "0123456789")}".OrNull(f, 0.25f))    // (4)
        { Guid = f.Random.Guid() };   // (5)
    })
    .Generate(10)
    .GroupBy(a => a.Email).Select(g => g.First())   // (6)
    .ToList();
    Authors.AddRange(authors);    // (7)
    SaveChanges();                // (8)

    // Read categories written in Initialize().
    // Use OrderBy with PK to read in a deterministic sort order!
    var categories = Categories.OrderBy(c => c.Id).ToList();

    var articles = new Faker<Article>("de").CustomInstantiator(f =>
    {
        return new Article(
            headline: f.Lorem.Sentence(f.Random.Int(2, 4)),
            content: f.Lorem.Paragraphs(10, 20),
            created: f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2022, 1, 1)),
            imageUrl: f.Image.PicsumUrl(),
            author: f.Random.ListItem(authors),    // (9)
            category: f.Random.ListItem(categories))
        { Guid = f.Random.Guid() };
    })
    .Generate(6)
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

## Verwenden von SQL Server statt SQLite

Wenn du Docker Desktop installiert hast, kannst du einfach einen Container für SQL Server laden.
Das root Password wird als *environment Variable* angegeben. Die Angabe des Ports geschieht
mit der Option *-p host_port:container_port*.

```
docker run -d -p 11433:1433 --name spengernews_sqlserver -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" mcr.microsoft.com/azure-sql-edge:latest
```

Es wird der Port im Container (1433) auf den Port 11433 gelegt, um Konflikte mit einer vorhandenen
SQL Server Datenbank zu vermeiden. Nun kannst du in der Datei *appsettings.json* unter *ConnectionStrings*
den Connection String für SqlServer eintragen. Achte auf den richtigen Port und das richtige root Password. Es
muss deinen Angaben bei *docker run* entsprechen.

**appsettings.json**
```javascript
// ...
"ConnectionStrings": {
    "Default": "Server=127.0.0.1,11433;Initial Catalog=SpengernewsDb;User Id=sa;Password=SqlServer2019;TrustServerCertificate=true"
},
// ...
```

Danach musst du in der *csproj* Datei *des Application Projektes* den SQL Server Treiber hinzufügen:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.*" />
```

In der Datei *Program.cs* kannst du nun statt auf den SQLite auf den SQL Server Connection String
referenzieren.

```c#
builder.Services.AddDbContext<SpengernewsContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));
```

## Verwenden von mariaDB

MariaDB kann natürlich auch als Docker Image geladen werden. Um einen mariaDB Container zu starten,
musst du die Umgebungsvariablen *MARIADB_USER* und *MARIADB_ROOT_PASSWORD* setzen. Der Standardport
wird hier auch umgelegt (auf 13306) um Konflikte mit lokalen Servern zu vermeiden.

```
docker run -d -p 13306:3306 --name spengernews_mariadb -e MARIADB_USER=root -e MARIADB_ROOT_PASSWORD=MariaDbPassword mariadb:10.10.3
```

Der entsprechende Connection String zum oben beschriebenen *docker run* Statement wird in die Datei
*appsettings.json* eingetragen.

**appsettings.json**
```javascript
// ...
"ConnectionStrings": {
    "Default": "server=localhost;port=13306;database=SpengernewsDb;user=root;password=MariaDbPassword"
},
// ...
```

Danach musst du in der *csproj* Datei *des Application Projektes* den SQL Server Treiber hinzufügen:

```xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="6.*" />
```

Nun kann in der Datei *Program.cs* der Datenbank Kontext registriert werden:

```c#
builder.Services.AddDbContext<SpengernewsContext>(opt =>
{
    opt.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        new MariaDbServerVersion("10.10.3"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
});
```

## Verwenden von PostgreSQL

PostgreSQL kann natürlich auch als Docker Image geladen werden. Um einen mariaDB Container zu starten,
musst du die Umgebungsvariablen *POSTGRES_USER* und *POSTGRES_PASSWORD* setzen. Der Standardport
wird hier auch umgelegt (von 5432 auf 15432) um Konflikte mit lokalen Servern zu vermeiden.

```
docker run -d -p 15432:5432 --name spengernews_postgres -e POSTGRES_USER=spengernews POSTGRES_PASSWORD=PostgresPassword postgres:15.1
```

Der entsprechende Connection String zum oben beschriebenen *docker run* Statement wird in die Datei
*appsettings.json* eingetragen.

**appsettings.json**
```javascript
// ...
"ConnectionStrings": {
    "Default": "Host=127.0.0.1;Port=15432;Database=spengernews;Username=spengernews;Password=PostgresPassword"
},
// ...
```

Danach musst du in der *csproj* Datei *des Application Projektes* den SQL Server Treiber hinzufügen:

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="6.*" />
```

Nun kann in der Datei *Program.cs* der Datenbank Kontext registriert werden:

```c#
builder.Services.AddDbContext<SpengernewsContext>(opt =>
{
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
});
// Allow unspecified DateTime values in DateTime Properties.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```

## Für Profis: Automatischer Start des Containers beim Programmstart

In [07_DockerStartDotnet.md](07_DockerStartDotnet.md) wird eine Extension Methode vorgestellt,
die den Container beim Programmstart automatisch lädt.