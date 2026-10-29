using System.Text;
using Arqui1.Hub.Data;

namespace Arqui1.Hub.Services.Tutor;

public static class TutorPrompt
{
    /// <summary>
    /// Se construye una sola vez y no cambia entre peticiones, así el prefijo se puede cachear.
    /// </summary>
    public static string System { get; } = Build();

    private static string Build()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""
            Eres el tutor del repositorio Carpeta_Arqui1, las prácticas de un curso de arquitectura de software con C# y .NET 10.
            Respondes siempre en español, con un tono cercano y claro, a estudiantes que están aprendiendo.

            Cómo responder:
            - Ve al punto. Explica el concepto en pocas frases y después muéstralo con un ejemplo corto en C#.
            - Cuando la pregunta tenga que ver con una lección del repositorio, apóyate en su código y nombra la carpeta donde está.
            - Si el estudiante pega código con errores, señala la línea, explica por qué falla y muestra la versión corregida.
            - Usa Markdown: bloques ```csharp para el código y listas cortas cuando ayuden.
            - Si algo no está en el repositorio, dilo y responde con lo que sabes de .NET en general.

            Lecciones del repositorio:
            """);

        foreach (var lesson in LessonCatalog.All)
        {
            sb.AppendLine();
            sb.AppendLine($"## {lesson.Title} (carpeta {lesson.Folder})");
            sb.AppendLine(lesson.Summary);
            sb.AppendLine($"Conceptos: {string.Join(", ", lesson.Concepts)}.");
            foreach (var point in lesson.KeyPoints)
                sb.AppendLine($"- {point}");
            sb.AppendLine("```csharp");
            sb.AppendLine(lesson.Snippet);
            sb.AppendLine("```");
            sb.AppendLine($"Para ejecutarla: {lesson.RunCommand}");
        }

        return sb.ToString();
    }
}
