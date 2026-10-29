namespace Arqui1.Hub.Data;

public sealed record QuizQuestion(
    string LessonSlug,
    string Prompt,
    IReadOnlyList<string> Options,
    int CorrectIndex,
    string Explanation);

/// <summary>Preguntas de autoevaluación, tres por lección, basadas en el código del repositorio.</summary>
public static class QuizBank
{
    public static IReadOnlyList<QuizQuestion> All { get; } =
    [
        // Hola mundo
        new("hola-mundo",
            "¿Qué genera el compilador a partir de las instrucciones de nivel superior de HelloWorld/Program.cs?",
            ["Nada: el programa no tiene punto de entrada", "Una clase Program con un método Main", "Un archivo .exe sin clases", "Un método Main dentro de System.Console"],
            1, "Desde C# 9 el compilador envuelve las instrucciones de nivel superior en una clase Program con un Main generado."),
        new("hola-mundo",
            "¿Cuántos archivos de un proyecto pueden tener instrucciones de nivel superior?",
            ["Uno", "Uno por carpeta", "Todos los que quieras", "Ninguno si hay ImplicitUsings"],
            0, "Solo puede haber un punto de entrada, así que solo un archivo puede tener instrucciones de nivel superior."),
        new("hola-mundo",
            "¿Qué hace <Nullable>enable</Nullable> en el .csproj?",
            ["Prohíbe usar null en todo el proyecto", "Hace que el compilador advierta cuando una referencia puede ser null", "Convierte todos los tipos en Nullable<T>", "Activa el recolector de basura"],
            1, "Habilita los tipos de referencia anulables: string? puede ser null y string no, y el compilador avisa los posibles NullReferenceException."),

        // NuGet
        new("paquetes-nuget",
            "¿Qué agrega al .csproj el comando dotnet add package Figgle?",
            ["Una carpeta packages/ con la DLL", "Un <ProjectReference>", "Un <PackageReference Include=\"Figgle\" Version=\"...\" />", "Una línea en global.json"],
            2, "NuGet registra la dependencia como <PackageReference>; los archivos se descargan a la caché global al restaurar."),
        new("paquetes-nuget",
            "¿Por qué no se suben a Git las carpetas bin/ y obj/?",
            ["Porque pesan menos de 1 MB", "Porque se generan de nuevo al compilar y restaurar", "Porque GitHub las bloquea", "Porque contienen el código fuente"],
            1, "Son artefactos de compilación: dotnet build los vuelve a crear. Versionarlos solo agrega ruido y conflictos (este repo tenía más de 1.000)."),
        new("paquetes-nuget",
            "En MyConsole, ¿qué estaba mal en la versión original de Program.cs?",
            ["Figgle no estaba instalado", "Se generaba el banner pero se imprimía \"Hello, World!\"", "Faltaba el método Main", "Usaba una fuente que no existe"],
            1, "La variable Message tenía el arte ASCII, pero Console.WriteLine imprimía otro texto. Se corrigió a Console.WriteLine(Message)."),

        // Bibliotecas
        new("bibliotecas",
            "¿Cómo usa MyConsoleApp las clases de MyLibrary en Demo01?",
            ["Copiando los archivos .cs", "Con un <ProjectReference> al proyecto de la biblioteca", "Instalando MyLibrary desde NuGet", "Con using static"],
            1, "La referencia de proyecto compila la biblioteca y pone su DLL junto a la app."),
        new("bibliotecas",
            "Demo01/MyLibrary depende de Figgle. ¿Qué necesita MyConsoleApp para usar FiggleWriter?",
            ["Instalar Figgle también", "Nada más: la dependencia llega de forma transitiva", "Copiar Figgle.dll a mano", "Cambiar a .NET Framework"],
            1, "Las dependencias NuGet de un proyecto referenciado fluyen hacia quien lo referencia."),
        new("bibliotecas",
            "¿Por qué FiggleWriter ahora tiene Render() además de Write()?",
            ["Por rendimiento", "Para separar el formateo de la salida y reutilizarlo fuera de la consola", "Porque Write() quedó obsoleto", "Porque Figgle lo exige"],
            1, "Render() devuelve el texto y Write() lo imprime. Así el hub web reutiliza la misma clase sin escribir en la consola."),

        // POO
        new("poo",
            "En la versión original de Product, ¿qué pasaba con public decimal UnitPrice => UnitPrice;?",
            ["Devolvía 0", "La propiedad se llamaba a sí misma (recursión infinita)", "Devolvía el parámetro del constructor", "No compilaba por el =>"],
            1, "El parámetro del constructor primario se llamaba igual que la propiedad, así que dentro de la clase UnitPrice era la propia propiedad. Por eso se renombró a unitPrice."),
        new("poo",
            "¿Qué obliga a hacer abstract decimal CalculateMonthlyPayment() en Employee?",
            ["Nada, es opcional", "Que cada clase derivada no abstracta lo implemente con override", "Que Employee lo implemente", "Que se llame desde el constructor"],
            1, "Un método abstracto no tiene cuerpo: cada derivada concreta debe proporcionar su implementación."),
        new("poo",
            "Employee Maria = new Secretary(); y luego Maria.CalculateMonthlyPayment(). ¿Qué versión se ejecuta?",
            ["La de Employee", "La de Secretary", "Da error de compilación", "La primera que encuentre el compilador"],
            1, "Polimorfismo: el despacho es dinámico y se usa el override del tipo real del objeto, no el de la variable."),

        // Inyección de dependencias
        new("inyeccion-dependencias",
            "Si se registran tres implementaciones de IUserActionWriter, ¿qué recibe un constructor que pide IUserActionWriter?",
            ["Las tres", "La primera registrada", "La última registrada", "Lanza una excepción"],
            2, "Pedir el servicio solo devuelve el último registro. Para recibirlos todos hay que pedir IEnumerable<IUserActionWriter>."),
        new("inyeccion-dependencias",
            "¿Cuál era el bug original de DependencyContainer en NorthWind?",
            ["Usaba AddTransient en vez de AddSingleton", "Los tres métodos Add*Writer registraban ConsoleWriter", "No tenía métodos de extensión", "Registraba IUserActionWriter dos veces con el mismo nombre"],
            1, "AddDebugWriter y AddFileWriter registraban ConsoleWriter: el mensaje salía tres veces en consola y nunca llegaba a AppLogs.txt."),
        new("inyeccion-dependencias",
            "¿Por qué ConsoleWriter, DebugWriter y FileWriter son internal?",
            ["Por rendimiento", "Para que la app dependa solo de la interfaz y no de las clases concretas", "Porque el contenedor no acepta clases public", "Por un error del curso"],
            1, "La app conoce IUserActionWriter y los métodos de extensión; las implementaciones se pueden cambiar sin tocar los servicios."),

        // Tareas
        new("tareas",
            "¿Qué pasa con new Task(action) si nunca se llama a Start()?",
            ["Se ejecuta al salir del método", "Nunca se ejecuta", "Se ejecuta en el hilo de UI", "Lanza una excepción"],
            1, "El constructor solo crea la tarea en estado Created. Task.Run, en cambio, la crea e inicia en un paso."),
        new("tareas",
            "Desde una tarea del ThreadPool, ¿cómo se actualiza un control WPF de forma segura?",
            ["Directamente, WPF es thread-safe", "Con Dispatcher.Invoke", "Con lock", "Con Thread.Sleep"],
            1, "Los controles pertenecen al hilo que los creó. Dispatcher.Invoke ejecuta el código en ese hilo."),
        new("tareas",
            "Cinco operaciones de E/S de 900, 700, 500, 800 y 400 ms con Task.WhenAll. ¿Cuánto tarda aproximadamente?",
            ["3.300 ms", "900 ms", "500 ms", "4.500 ms"],
            1, "Las esperas ocurren a la vez, así que el total es aproximadamente lo que tarda la más lenta. Pruébalo en el laboratorio Task.WhenAll."),

        // Blazor
        new("blazor",
            "¿Qué convierte un componente .razor en una página navegable?",
            ["El bloque @code", "La directiva @page \"/ruta\"", "Ponerlo en la carpeta Pages", "Heredar de ComponentBase"],
            1, "@page registra la ruta en el router. La carpeta Pages es solo una convención."),
        new("blazor",
            "En Blazor WebAssembly (LabScript), ¿dónde se ejecuta el código C# de los componentes?",
            ["En el servidor", "En el navegador, sobre WebAssembly", "En un servicio de Windows", "En SQL Server"],
            1, "El runtime de .NET se descarga y corre en el navegador. Este hub usa Interactive Server: ahí el código corre en el servidor."),
        new("blazor",
            "¿Qué hace @onclick=\"IncrementCount\" en Counter.razor?",
            ["Llama a IncrementCount al cargar la página", "Enlaza el clic del botón con el método IncrementCount", "Crea un evento JavaScript", "Nada, es un comentario"],
            1, "Blazor conecta el evento del DOM con el método C# y vuelve a renderizar el componente después."),
    ];

    public static IEnumerable<QuizQuestion> ForLesson(string slug) =>
        All.Where(q => q.LessonSlug == slug);
}
