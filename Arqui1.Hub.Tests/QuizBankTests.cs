using Arqui1.Hub.Data;
using Arqui1.Hub.Services.Tutor;

namespace Arqui1.Hub.Tests;

public class QuizBankTests
{
    [Fact]
    public void Cada_leccion_tiene_al_menos_tres_preguntas()
    {
        foreach (var lesson in LessonCatalog.All)
            Assert.True(QuizBank.ForLesson(lesson.Slug).Count() >= 3, $"{lesson.Slug} tiene menos de 3 preguntas");
    }

    [Fact]
    public void Las_preguntas_estan_bien_formadas()
    {
        foreach (var q in QuizBank.All)
        {
            Assert.NotNull(LessonCatalog.Find(q.LessonSlug));
            Assert.InRange(q.CorrectIndex, 0, q.Options.Count - 1);
            Assert.True(q.Options.Count >= 3, q.Prompt);
            Assert.Equal(q.Options.Count, q.Options.Distinct().Count());
            Assert.False(string.IsNullOrWhiteSpace(q.Explanation));
        }
    }

    [Fact]
    public void El_tutor_sin_conexion_reconoce_la_leccion_de_cada_pregunta()
    {
        // El botón "Explícamelo con el tutor" manda la pregunta al tutor: sin API key debe caer en la lección correcta.
        var misses = QuizBank.All
            .Where(q => OfflineTutorService.FindBestLesson(q.Prompt)?.Slug != q.LessonSlug)
            .Select(q => $"{q.LessonSlug}: {q.Prompt}")
            .ToList();

        Assert.True(misses.Count <= QuizBank.All.Count / 5, "Demasiadas preguntas sin reconocer:\n" + string.Join("\n", misses));
    }
}
