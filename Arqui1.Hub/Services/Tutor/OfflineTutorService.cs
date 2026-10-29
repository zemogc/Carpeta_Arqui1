using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Arqui1.Hub.Data;

namespace Arqui1.Hub.Services.Tutor;

/// <summary>
/// Tutor sin conexión: busca la lección más relacionada con la pregunta y responde con sus puntos clave.
/// Permite usar el hub sin una API key.
/// </summary>
public sealed class OfflineTutorService : ITutorService
{
    public string EngineName => "Modo sin conexión";

    public bool IsOnline => false;

    public async IAsyncEnumerable<string> AskAsync(
        IReadOnlyList<ChatTurn> conversation,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var question = conversation.LastOrDefault(t => t.Role == ChatRole.User)?.Text ?? string.Empty;
        var answer = BuildAnswer(question);

        // Se entrega por palabras para que se vea igual que la respuesta en streaming de Claude.
        foreach (var word in answer.Split(' '))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return word + " ";
            await Task.Delay(8, cancellationToken);
        }
    }

    public static string BuildAnswer(string question)
    {
        var lesson = FindBestLesson(question);
        if (lesson is null)
        {
            return "No encontré una lección relacionada con esa pregunta. Prueba con temas como **herencia**, " +
                   "**inyección de dependencias**, **async/await**, **NuGet** o **Blazor**.\n\n" +
                   "_Para respuestas abiertas configura una API key de Anthropic (ver README)._";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Esto se ve en la lección **{lesson.Icon} {lesson.Title}** (carpeta `{lesson.Folder}`).");
        sb.AppendLine();
        sb.AppendLine(lesson.Summary);
        sb.AppendLine();
        foreach (var point in lesson.KeyPoints)
            sb.AppendLine($"- {point}");
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine(lesson.Snippet);
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine($"Pruébalo con `{lesson.RunCommand}`.");
        sb.AppendLine();
        sb.Append("_Respuesta del modo sin conexión. Con una API key de Anthropic el tutor responde cualquier pregunta._");
        return sb.ToString();
    }

    public static Lesson? FindBestLesson(string question)
    {
        var normalized = Normalize(question);
        if (normalized.Length == 0)
            return null;

        var best = LessonCatalog.All
            .Select(lesson => (lesson, score: Score(lesson, normalized)))
            .OrderByDescending(x => x.score)
            .First();

        return best.score > 0 ? best.lesson : null;
    }

    private static int Score(Lesson lesson, string question)
    {
        var words = question.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries);
        int score = lesson.Keywords.Count(k => Matches(Normalize(k), question, words)) * 3;
        score += lesson.Concepts.Count(c => question.Contains(Normalize(c))) * 2;
        return score;
    }

    private static readonly char[] WordSeparators = [' ', ',', '.', ';', ':', '?', '¿', '!', '¡', '(', ')', '"', '\'', '\n', '\r', '\t', '/', '<', '>'];

    // Frases: se buscan tal cual. Palabras: por prefijo ("asincron" → "asincronía"),
    // salvo las muy cortas como "di", que deben coincidir completas.
    private static bool Matches(string keyword, string question, string[] words) =>
        keyword.Contains(' ') ? question.Contains(keyword)
        : keyword.Length <= 3 ? words.Contains(keyword)
        : words.Any(w => w.StartsWith(keyword, StringComparison.Ordinal));

    // Minúsculas y sin tildes, para que "inyección" y "inyeccion" coincidan.
    private static string Normalize(string text)
    {
        var decomposed = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Trim();
    }
}
