namespace Arqui1.Hub.Data;

public sealed record Lesson(
    string Slug,
    string Title,
    string Folder,
    string Icon,
    string Summary,
    IReadOnlyList<string> Concepts,
    IReadOnlyList<string> KeyPoints,
    string Snippet,
    string RunCommand,
    IReadOnlyList<string> Keywords);

/// <summary>
/// Las lecciones del curso, tal como están en las carpetas del repositorio.
/// El tutor IA usa este mismo catálogo como contexto.
/// </summary>
public static class LessonCatalog
{
    public const string RepositoryUrl = "https://github.com/zemogc/Carpeta_Arqui1";

    public static IReadOnlyList<Lesson> All { get; } =
    [
        new(
            Slug: "hola-mundo",
            Title: "Hola mundo: dos estilos de Program.cs",
            Folder: "HelloWorld · HelloWorldOld",
            Icon: "👋",
            Summary: "El mismo programa escrito con instrucciones de nivel superior y con la clase Program clásica con Main.",
            Concepts: ["Instrucciones de nivel superior", "Método Main", "ImplicitUsings", "Nullable"],
            KeyPoints:
            [
                "Desde C# 9 el compilador genera la clase Program y el método Main a partir de las instrucciones de nivel superior.",
                "Solo un archivo del proyecto puede tener instrucciones de nivel superior.",
                "ImplicitUsings agrega automáticamente using System, System.IO, System.Linq, etc.",
                "Nullable=enable hace que el compilador advierta cuando una referencia puede ser null.",
            ],
            Snippet: """
                // HelloWorld/Program.cs (nivel superior)
                Console.WriteLine("Hello, World!");

                // HelloWorldOld/Program.cs (clásico)
                namespace HelloWorldOld;

                class Program
                {
                    static void Main(string[] args)
                    {
                        Console.WriteLine("Hello, World!");
                    }
                }
                """,
            RunCommand: "dotnet run --project HelloWorld",
            Keywords: ["hola", "hello", "main", "top-level", "nivel superior", "program", "consola", "implicit", "nullable"]),

        new(
            Slug: "paquetes-nuget",
            Title: "Paquetes NuGet",
            Folder: "MyConsole",
            Icon: "📦",
            Summary: "Agregar una dependencia externa (Figgle) con NuGet y usarla para generar arte ASCII.",
            Concepts: ["NuGet", "PackageReference", "dotnet add package"],
            KeyPoints:
            [
                "dotnet add package Figgle agrega un <PackageReference> al .csproj.",
                "dotnet restore descarga los paquetes a la caché global (~/.nuget/packages).",
                "Los paquetes no se versionan en el repositorio: se restauran al compilar.",
            ],
            Snippet: """
                // MyConsole/Program.cs
                string Message = Figgle.FiggleFonts.Standard.Render("Hola, mundo!");
                Console.WriteLine(Message);
                """,
            RunCommand: "dotnet run --project MyConsole",
            Keywords: ["nuget", "paquete", "package", "figgle", "dependencia", "ascii", "restore"]),

        new(
            Slug: "bibliotecas",
            Title: "Soluciones y bibliotecas de clases",
            Folder: "Demo01",
            Icon: "📚",
            Summary: "Una solución con una app de consola que referencia una biblioteca de clases propia (MessageWriter y FiggleWriter).",
            Concepts: ["Solución .sln", "Biblioteca de clases", "ProjectReference", "Reutilización"],
            KeyPoints:
            [
                "Una solución agrupa varios proyectos; cada proyecto genera su propio ensamblado (.dll).",
                "La consola usa la biblioteca con <ProjectReference Include=\"..\\MyLibrary\\MyLibrary.csproj\" />.",
                "Las dependencias NuGet de la biblioteca (Figgle) llegan de forma transitiva a la consola.",
                "Este mismo FiggleWriter se reutiliza en el laboratorio de arte ASCII del hub.",
            ],
            Snippet: """
                // Demo01/MyConsoleApp/Program.cs
                MessageWriter MyWriter = new MessageWriter();
                MyWriter.Write("Hello, Writer");

                FiggleWriter FWriter = new FiggleWriter();
                FWriter.Write("Hello, Figgle!");
                """,
            RunCommand: "dotnet run --project Demo01/MyConsoleApp",
            Keywords: ["biblioteca", "library", "solucion", "solution", "referencia", "reference", "proyecto", "dll", "ensamblado", "writer"]),

        new(
            Slug: "poo",
            Title: "Programación orientada a objetos",
            Folder: "Demo02",
            Icon: "🧱",
            Summary: "Clases con propiedades y constructores primarios, herencia, clases abstractas y polimorfismo con empleados y productos.",
            Concepts: ["Constructores primarios", "Propiedades", "Herencia", "Clases abstractas", "Polimorfismo", "virtual / override"],
            KeyPoints:
            [
                "Un constructor primario (class Product(int id, ...)) deja los parámetros disponibles en toda la clase.",
                "Los parámetros no deben llamarse igual que las propiedades: UnitPrice => UnitPrice se llamaría a sí misma.",
                "{ get; private set; } permite leer desde fuera pero solo modificar desde la propia clase.",
                "Employee es abstracta: obliga a cada tipo de empleado a implementar CalculateMonthlyPayment().",
                "GetPaymentDays() es virtual: Developer la sobrescribe con override para cobrar quincenal.",
                "Una variable Employee puede apuntar a un Secretary o un Manager: eso es polimorfismo.",
            ],
            Snippet: """
                internal abstract class Employee
                {
                    public string Name { get; set; } = string.Empty;
                    public abstract decimal CalculateMonthlyPayment();
                    public virtual int GetPaymentDays() => 30;
                }

                internal class Developer : Employee
                {
                    public override decimal CalculateMonthlyPayment() => 5_000_000m;
                    public override int GetPaymentDays() => 15;
                }

                Employee Maria = new Secretary();
                """,
            RunCommand: "dotnet run --project Demo02/MyLibrary",
            Keywords: ["poo", "objeto", "clase", "class", "herencia", "abstract", "abstracta", "polimorfismo", "virtual", "override", "propiedad", "constructor", "primario", "employee", "product", "book", "encapsulamiento"]),

        new(
            Slug: "inyeccion-dependencias",
            Title: "Inyección de dependencias",
            Folder: "NorthWind",
            Icon: "🧩",
            Summary: "Una solución en capas donde los servicios dependen de la interfaz IUserActionWriter y el contenedor de Microsoft.Extensions.DependencyInjection decide qué implementaciones entregar.",
            Concepts: ["Interfaces", "Value Objects", "Métodos de extensión", "IServiceCollection", "IEnumerable<T> de servicios", "Generic Host"],
            KeyPoints:
            [
                "NorthWind.Entities define el contrato (IUserActionWriter) y el objeto de valor UserAction; no depende de nadie.",
                "NorthWind.Writers implementa el contrato (consola, Debug y archivo) y expone métodos de extensión AddConsoleWriter(), AddDebugWriter(), AddFileWriter().",
                "Si se registran varias implementaciones de la misma interfaz, pedir IUserActionWriter devuelve solo la última; pedir IEnumerable<IUserActionWriter> devuelve todas.",
                "Las clases concretas son internal: la app solo conoce la interfaz, así se puede cambiar la implementación sin tocar los servicios.",
                "Host.CreateApplicationBuilder() arma el contenedor, la configuración y el logging.",
            ],
            Snippet: """
                var Builder = Host.CreateApplicationBuilder();
                Builder.Services.AddConsoleWriter();
                Builder.Services.AddDebugWriter();
                Builder.Services.AddFileWriter();
                Builder.Services.AddSingleton<ProductService>();
                using var AppHost = Builder.Build();

                internal class ProductService(IEnumerable<IUserActionWriter> writers)
                {
                    public void Add(string user, string productName)
                    {
                        UserAction Action = new UserAction(user, $"Created: {productName}");
                        foreach (var Writer in writers)
                            Writer.Write(Action);
                    }
                }
                """,
            RunCommand: "dotnet run --project NorthWind/NorthWind.ConsoleApp",
            Keywords: ["inyeccion", "dependencia", "dependency", "injection", "di", "contenedor", "container", "servicio", "service", "productservice", "ienumerable", "iuseractionwriter", "useraction", "applogger", "interfaz", "interface", "singleton", "scoped", "transient", "host", "northwind", "writer", "extension", "solid", "capas"]),

        new(
            Slug: "tareas",
            Title: "Tareas y programación asíncrona",
            Folder: "WPFApp",
            Icon: "⚡",
            Summary: "Una ventana WPF que muestra cuatro formas de crear tareas con la TPL y cómo volver al hilo de la interfaz.",
            Concepts: ["Task", "Delegado Action", "Métodos anónimos", "Expresiones lambda", "async / await", "Dispatcher"],
            KeyPoints:
            [
                "new Task(action) crea la tarea pero no la ejecuta: hay que llamar a Start().",
                "Action es un delegado sin parámetros ni retorno; se puede crear desde un método, un método anónimo (delegate { }) o una lambda (() => ...).",
                "Task.Run(...) crea e inicia la tarea en el ThreadPool en un solo paso.",
                "await libera el hilo de UI mientras la tarea trabaja; al terminar, el código continúa en el hilo de UI.",
                "Desde otro hilo no se pueden tocar controles WPF directamente: se usa Dispatcher.Invoke.",
            ],
            Snippet: """
                Task T2 = new Task(delegate
                {
                    WriteLog("Ejecutando una tarea en un método anónimo.");
                });
                T2.Start();

                int Primes = await Task.Run(() => CountPrimes(2_000_000));
                """,
            RunCommand: "dotnet run --project WPFApp/WPFApp",
            Keywords: ["task", "tarea", "hilo", "thread", "async", "await", "asincron", "delegado", "delegate", "action", "lambda", "anonimo", "wpf", "dispatcher", "paralelo", "tpl", "threadpool"]),

        new(
            Slug: "blazor",
            Title: "Blazor WebAssembly",
            Folder: "LabScript",
            Icon: "🌐",
            Summary: "La plantilla de Blazor WebAssembly: componentes Razor que corren en el navegador con .NET compilado a WebAssembly.",
            Concepts: ["Componentes Razor", "@code", "Eventos @onclick", "Enrutamiento @page", "HttpClient", "WebAssembly"],
            KeyPoints:
            [
                "Cada archivo .razor es un componente; @page \"/counter\" lo convierte en una página con ruta.",
                "El bloque @code contiene el estado y los métodos del componente.",
                "@onclick=\"IncrementCount\" enlaza un evento del DOM con un método de C#.",
                "En WebAssembly todo el runtime de .NET se descarga y corre en el navegador.",
                "Este hub, en cambio, usa Blazor con renderizado interactivo en el servidor.",
            ],
            Snippet: """
                @page "/counter"

                <p role="status">Current count: @currentCount</p>
                <button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

                @code {
                    private int currentCount = 0;
                    private void IncrementCount() => currentCount++;
                }
                """,
            RunCommand: "dotnet run --project LabScript/LabScript",
            Keywords: ["blazor", "razor", "componente", "component", "webassembly", "wasm", "web", "pagina", "navegador", "onclick", "httpclient"]),
    ];

    public static Lesson? Find(string slug) =>
        All.FirstOrDefault(l => string.Equals(l.Slug, slug, StringComparison.OrdinalIgnoreCase));
}
