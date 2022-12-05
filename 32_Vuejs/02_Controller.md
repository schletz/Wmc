# Controller in ASP.NET Core anlegen

In der Datei *Program.cs* im Webapi Projekt haben wir mit der folgenden Zeile Middleware aktiviert,
die automatisch nach bestimmten Klassen, den Controllern, sucht.

```c#
app.MapControllers();
```

Wir legen nun im Ordner *Controllers* eine Datei mit dem Namen *NewsController.cs* an.

```c#
namespace SpengernewsProject.Webapi.Controllers
{
    [ApiController]               // Muss bei jedem Controller stehen
    [Route("/api/[controller]")]  // Muss bei jedem Controller stehen
    public class NewsController : ControllerBase
    {
        private readonly SpengernewsContext _db;

        public NewsController(SpengernewsContext db)
        {
            _db = db;
        }
        // Reagiert auf GET /api/news
        [HttpGet]
        public IActionResult GetAllNews()
        {
            return Ok(new string[]{"News 1", "News 2"});
        }
        // Reagiert z. B. auf /api/news/14
        [HttpGet("{id:int}")]
        public IActionResult GetNewsDetail(int id)
        {
            if (id < 1000) { return NotFound(); }
            return Ok($"News {id}");
        }
    }
}
``` 

Wir erkennen einige Annoatations:

- **ApiController** wird benötigt, damit ASP.NET Core überhaupt diese Klasse als Controller
  registriert.
- **Route** verbindet die Klasse mit einer Route (also einer URL). Schreiben wir z. B.
  `[Route("/api/[controller]")]`, so reagiert der Controller auf Requests mit der Url */api/news*.
  `[controller]` bedeutet, dass der Name der Klasse ohne das Wort Controller verwendet wird. Aus
  der Klasse *NewsController* wird also die Route */api/news* Das Wort Controller wird weggestrichen.
- **HttpGet** ohne Parameter kennzeichnet die Methode, die bei einem GET Request auf die URL des
  Controllers aufgerufen wird. Sie darf nur ein Mal pro Controller verwendet werden.
- **HttpGet("{id:int}")** bedeutet, dass nach der Route des Controllers noch Daten folgen, z. B.
  /api/news/14. Die geschweiften Klammern bedeuten, dass dieser Wert in eine Variable geschrieben
  wird. Achte darauf, dass der Parameter der Methode, die diese Annotation verwendet, auch exakt
  diesen Parameternamen verwendet.

Der Konstruktor erwartet sich eine Instanz vom Typ *SpengernewsContext*. In der Datei *Program.cs*
wurde ein sogenanntes *Service* registriert:

```c#
builder.Services.AddDbContext<SpengernewsContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
```

ASP.NET Core ist ein *dependency injection framework*. Es erkennt, dass der Controller eine Instanz
vom Typ *SpengernewsContext* braucht. Danach wird im *Service Provider* nachgesehen, ob das Service
registriert wurde. Falls ja, wird es automatisch instanziert und übergeben. Wird auf die Registrierung
des Services vergessen, entsteht ein Laufzeitfehler.
