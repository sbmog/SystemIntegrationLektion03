# Opgaver: Controller-baseret Web API i .NET

Velkommen til disse øvelser, der er designet til at introducere dig til grundlæggende koncepter i udviklingen af controller-baserede Web API'er med .NET.


## Opgave 1: Din første controller

Målet med denne opgave er at oprette den mest simple Web API-controller og få et endpoint til at returnere en statisk besked.

1.  **Opret et nyt Web API-projekt:**
    Højreklik på Solution og vælg "Add" > "New Project". Vælg "ASP.NET Core Web API" og navngiv det f.eks. "MyFirstApi". Alternativt kan du bruge CLI:
    ```bash
    dotnet new webapi -n MyFirstApi
    cd MyFirstApi
    ```
2.  **Opret en ny Controller:**
    Opret en ny fil i `Controllers`-mappen med navnet `HelloController.cs`.
3.  **Implementer Controlleren:**
    Tilføj følgende kode til `HelloController.cs`:

    ```csharp
    using Microsoft.AspNetCore.Mvc;

    namespace MyFirstApi.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hej fra min første controller!";
        }
    }
    ```    
4. **Tilføj Scalar testtool**
    Tilføj `Scalar` til dit projekt via NuGet Package Manager eller CLI:
    ```bash
    dotnet add package Scalar
    ```
    I Nuget skal du installere **Scalar.AspNetCore**

    Tilføj følgende kode i `Program.cs` for at aktivere Scalar:
    ```csharp
    app.MapScalarApiReference();
    ```
    lige efter `app.MapControllers();`

5.  **Kør applikationen:**
    Enten via Visual Studio (F5) eller via terminalen:
    ```bash
    dotnet run
    ```
6.  **Test dit endpoint:**
    Åbn en browser eller brug dit API-testværktøj til at lave en GET-anmodning til `https://localhost:<port>/hello`. Du skal se beskeden "Hej fra min første controller!".

---

## Opgave 2: GET med parametre

Nu skal vi udvide vores API, så det kan tage imod input fra brugeren via URL'en.

1.  **Tilføj et nyt endepunkt:**
    Tilføj en ny metode til din `HelloController.cs`, der kan tage et navn som parameter.

    ```csharp
    [HttpGet("{name}")]
    public string Get(string name)
    {
        return $"Hej, {name}!";
    }
    ```
    *Bemærk: Da vi nu har to `HttpGet`-metoder, skal den ene have en "route template" for at undgå konflikter. `"{name}"` angiver, at en del af URL'en skal bruges som en parameter.*

2.  **Genstart og test:**
    Stop og genstart din applikation (`dotnet run`). Test det nye endepunkt ved at lave en GET-anmodning til `https://localhost:<port>/hello/dit-navn`. Forventet output er "Hej, dit-navn!".

---

## Opgave 3: POST med en simpel model

GET-anmodninger er gode til at hente data. Til at sende data til serveren bruger vi typisk POST.

1.  **Opret en Datamodel:**
    Opret en ny fil, f.eks. `Models/Person.cs`, og definer en simpel klasse.

    ```csharp
    namespace MyFirstApi.Models;

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    ```

2.  **Opret en ny POST-metode:**
    Tilføj en ny controller eller udvid den eksisterende med en metode, der håndterer POST-anmodninger.

    ```csharp
    // I HelloController.cs eller en ny controller
    [HttpPost]
    public string Post([FromBody] Person person)
    {
        return $"Modtaget person: {person.Name}, som er {person.Age} år gammel.";
    }
    ```
    *`[FromBody]`-attributten fortæller .NET, at den skal forsøge at læse `Person`-objektet fra body'en af HTTP-anmodningen.*

3.  **Test med API-værktøj:**
    *   Genstart din applikationen.
    *   Brug Postman eller Thunder Client til at lave en **POST**-anmodning til `https://localhost:<port>/hello`.
    *   I "Body"-sektionen af din anmodning skal du vælge `raw` og `JSON`.
    *   Indtast et JSON-objekt, der matcher din `Person`-model:
        ```json
        {
            "name": "Anders",
            "age": 30
        }
        ```
    *   Send anmodningen. Du bør modtage strengen "Modtaget person: Anders, som er 30 år gammel." som svar.

---

## Opgave 4: Datavalidering

Det er vigtigt at sikre, at de data, dit API modtager, er gyldige.

1.  **Tilføj valideringsattributter:**
    Opdater din `Person`-model med `System.ComponentModel.DataAnnotations`.

    ```csharp
    using System.ComponentModel.DataAnnotations;

    namespace MyFirstApi.Models;

    public class Person
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Range(0, 120)]
        public int Age { get; set; }
    }
    ```

2.  **Opdater din POST-metode:**
    .NET's `[ApiController]`-attribut sørger automatisk for at validere modellen og opdatere `ModelState`. Vi skal blot tjekke den.

    ```csharp
    [HttpPost]
    public IActionResult Post([FromBody] Person person)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok($"Modtaget person: {person.Name}, som er {person.Age} år gammel.");
    }
    ```
    *Ved at returnere `IActionResult` kan vi sende forskellige HTTP-statuskoder tilbage (f.eks. `BadRequest` for fejl og `Ok` for succes).*

3.  **Test valideringen:**
    *   Genstart applikationen.
    *   Prøv at sende en POST-anmodning med ugyldige data:
        *   Uden `name`-felt.
        *   Med et `name`, der er for kort (f.eks. "A").
        *   Med en `age`, der er uden for det tilladte interval (f.eks. 150).
    *   Observer, hvordan API'et nu returnerer en `400 Bad Request` med detaljer om valideringsfejlene.

---

## Opgave 5: Bonus - Refaktorering til en Service

God praksis er at holde controllere "tynde". Det betyder, at de kun skal håndtere HTTP-relaterede opgaver. Forretningslogik bør ligge i separate serviceklasser.

1.  **Opret en Service Interface og Klasse:**
    *   `Services/IGreetingService.cs`
        ```csharp
        namespace MyFirstApi.Services;
        public interface IGreetingService {
            string CreateGreeting(string name);
        }
        ```
    *   `Services/GreetingService.cs`
        ```csharp
        namespace MyFirstApi.Services;
        public class GreetingService : IGreetingService {
            public string CreateGreeting(string name) {
                return $"Hej fra service, {name}!";
            }
        }
        ```

2.  **Registrer Servicen til Dependency Injection:**
    I `Program.cs`, tilføj din service til DI-containeren.

    ```csharp
    // ... før builder.Build()
    builder.Services.AddScoped<IGreetingService, GreetingService>();
    ```

3.  **Injicer og brug Servicen i din Controller:**
    Modificer din `HelloController` til at bruge den nye service.

    ```csharp
    // ... i HelloController.cs
    private readonly IGreetingService _greetingService;

    public HelloController(IGreetingService greetingService)
    {
        _greetingService = greetingService;
    }

    [HttpGet("{name}")]
    public string Get(string name)
    {
        return _greetingService.CreateGreeting(name);
    }
    ```

4.  **Test igen:**
    Genstart og test dit `GET /hello/{name}`-endepunkt. Funktionaliteten skal være den samme, men din kode er nu bedre struktureret og nemmere at vedligeholde og teste.

---
---

# Projekt: Todo API

I de følgende opgaver skal du bygge et komplet Todo API, der kan oprette, læse, opdatere og slette opgaver. Vi bruger en "in-memory database" til at gemme data, så de kun eksisterer, mens applikationen kører.

## Opgave 6: Projektopsætning og Model

1.  **Opret et nyt Web API-projekt:**
    Højreklik på Solution og vælg "Add" > "New Project". Vælg "ASP.NET Core Web API" og navngiv det f.eks. "TodoApi". Alternativt kan du bruge CLI:
    ```bash
    dotnet new webapi -n TodoApi
    cd TodoApi
    ```
2.  **Opret en `TodoItem`-model:**
    Opret en fil `Models/TodoItem.cs` med følgende indhold:
    ```csharp
    namespace TodoApi.Models;

    public class TodoItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
    ```

## Opgave 7: In-Memory Database og Service

Vi simulerer en database ved hjælp af en simpel liste i en "context"-klasse.

1.  **Opret en `TodoContext`-klasse:**
    Denne klasse vil holde på vores data. Opret `Data/TodoContext.cs`:
    ```csharp
    using Microsoft.EntityFrameworkCore;
    using TodoApi.Models;

    namespace TodoApi.Data;

    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; } = null!;
    }
    ```
    *Vi bruger `DbContext` fra Entity Framework Core, da det gør det meget nemt at skifte til en rigtig database senere.*

2.  **Registrer `TodoContext` til Dependency Injection:**
    I `Program.cs`, konfigurer In-Memory databasen.
    ```csharp
    // Lige efter var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDbContext<TodoContext>(opt =>
        opt.UseInMemoryDatabase("TodoList"));
    ```
    Husk at tilføje `using TodoApi.Data;` og `using Microsoft.EntityFrameworkCore;` øverst i `Program.cs`.

## Opgave 8: TodoController og de første endepunkter

Nu er det tid til at lave controlleren, der skal eksponere vores data via HTTP.

1.  **Opret `TodoController`:**
    Opret filen `Controllers/TodoController.cs`. Start med denne kode:
    ```csharp
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using TodoApi.Data;
    using TodoApi.Models;

    namespace TodoApi.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // Her mangler vi stadig GET by id, PUT, og DELETE
    }
    ```
2.  **Test GET og POST:**
    *   Kør applikationen (`dotnet run`).
    *   Brug Postman/Thunder Client:
        *   **GET** `api/todo`: Du bør få et tomt array `[]` tilbage.
        *   **POST** til `api/todo` med en body som denne (bemærk at `id` bliver sat af databasen):
          ```json
          {
             "name": "Gå en tur med hunden",
             "isComplete": false
          }
          ```
          Du bør få et `201 Created` svar med det nye item.
        *   **GET** `api/todo` igen. Nu bør du se det item, du lige har oprettet.

## Opgave 9: Færdiggør CRUD-operationer

Fuldfør API'et ved at tilføje de resterende metoder til `TodoController.cs`.

1.  **Implementer GET by ID:**
    Metoden skal kunne finde et specifikt `TodoItem` baseret på dets `id`.
    ```csharp
    // GET: api/Todo/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        return todoItem;
    }
    ```

2.  **Implementer PUT (Update):**
    Denne metode opdaterer et eksisterende item.
    ```csharp
    // PUT: api/Todo/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return BadRequest();
        }

        _context.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.TodoItems.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }
    ```

3.  **Implementer DELETE:**
    Fjern et item fra databasen.
    ```csharp
    // DELETE: api/Todo/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    ```

## Opgave 10: Fuld Test af API'et

Test hele flowet i dit API.

1.  **POST** et par nye todo-items til `api/todo`.
2.  **GET** `api/todo` for at se alle de items, du har oprettet. Notér `id` for et af dem.
3.  **GET** `api/todo/{id}` med det `id`, du noterede, for at hente et specifikt item.
4.  **PUT** til `api/todo/{id}` for at opdatere item. Ændr f.eks. `isComplete` til `true`. Body'en skal indeholde hele det opdaterede objekt.
    ```json
    {
       "id": 1,
       "name": "Gå en tur med hunden",
       "isComplete": true
    }
    ```
5.  **GET** `api/todo/{id}` igen for at bekræfte, at ændringen er gemt.
6.  **DELETE** `api/todo/{id}` for at slette det.
7.  **GET** `api/todo` en sidste gang for at se, at det er blevet fjernet fra listen.

Tillykke! Du har nu bygget et fuldt funktionelt REST API med .NET.

---
---

# Projekt: Forfatter- og Blog Post API

I denne opgave skal du designe og implementere et API, der håndterer forfattere og deres blogindlæg. Fokus er på selvstændigt at implementere logikken baseret på et sæt af endepunkter og hints til datalaget.

## Opgave 11: Implementer et Forfatter- og Blog Post API

Målet er at bygge et fuldt funktionelt API til at administrere forfattere og deres blogindlæg. Du skal selv implementere controllere, modeller og et in-memory datalag.

**Datamodeller:**
Du skal som minimum bruge disse to modeller:
*   **Author:** `Id` (long), `Name` (string)
*   **BlogPost:** `Id` (long), `Title` (string), `Content` (string), `AuthorId` (long)

**Nødvendige Endepunkter:**
Implementer controllere, der eksponerer følgende endepunkter:

*   **Forfatter-håndtering:**
    *   `GET /api/authors` - Henter alle forfattere.
    *   `GET /api/authors/{id}` - Henter en specifik forfatter.
    *   `POST /api/authors` - Opretter en ny forfatter.
    *   `PUT /api/authors/{id}` - Opdaterer en forfatter.
    *   `DELETE /api/authors/{id}` - Sletter en forfatter.

*   **Blog Post-håndtering:**
    *   `GET /api/posts/{id}` - Henter et specifikt blogindlæg.
    *   `PUT /api/posts/{id}` - Opdaterer et blogindlæg.
    *   `DELETE /api/posts/{id}` - Sletter et blogindlæg.

*   **Relationelle Endepunkter:**
    *   `GET /api/authors/{authorId}/posts` - Henter alle blogindlæg for en specifik forfatter.
    *   `POST /api/authors/{authorId}/posts` - Opretter et nyt blogindlæg tilknyttet en forfatter.

**Hints til Implementering af Datalag (In-Memory):**

1.  **Opret en Data Service:**
    *   Lav en ny klasse, f.eks. `BlogDataService.cs`.
    *   Inde i denne klasse kan du bruge to statiske lister til at gemme data:
        ```csharp
        private static List<Author> _authors = new List<Author>();
        private static List<BlogPost> _posts = new List<BlogPost>();
        private static long _nextAuthorId = 1;
        private static long _nextPostId = 1;
        ```
2.  **Singleton Registrering:**
    *   Registrer din `BlogDataService` som en singleton i `Program.cs`:
        ```csharp
        builder.Services.AddSingleton<BlogDataService>();
        ```
    *   Ved at registrere den som en singleton sikrer du, at den samme instans af servicen (og dermed de samme statiske lister) bliver genbrugt på tværs af alle HTTP-anmodninger.

3.  **CRUD-metoder i Servicen:**
    *   Implementer offentlige metoder i `BlogDataService` til at håndtere data, f.eks.:
        *   `GetAuthors()`, `GetAuthorById(long id)`
        *   `CreateAuthor(Author author)` (husk at tildele et nyt ID og inkrementere `_nextAuthorId`)
        *   `GetPostsForAuthor(long authorId)`
        *   `CreatePostForAuthor(long authorId, BlogPost post)`
        *   osv...

4.  **Brug Servicen i Controllerne:**
    *   Injicer `BlogDataService` i dine controllere (`AuthorsController` og `PostsController`).
    *   Kald metoderne på din service for at udføre de nødvendige handlinger i dine action-metoder.
