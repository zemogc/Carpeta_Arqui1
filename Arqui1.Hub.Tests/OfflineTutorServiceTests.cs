using Arqui1.Hub.Services.Tutor;

namespace Arqui1.Hub.Tests;

public class OfflineTutorServiceTests
{
    [Theory]
    [InlineData("¿Cómo funciona la inyección de dependencias?", "inyeccion-dependencias")]
    [InlineData("no entiendo la inyeccion de dependencias", "inyeccion-dependencias")]
    [InlineData("¿Por qué ProductService recibe IEnumerable<IUserActionWriter>?", "inyeccion-dependencias")]
    [InlineData("Dame un ejercicio de herencia con Product y Book", "poo")]
    [InlineData("Explícame la diferencia entre virtual y abstract", "poo")]
    [InlineData("¿Qué pasa si toco un control WPF desde una Task?", "tareas")]
    [InlineData("¿cómo uso async/await?", "tareas")]
    [InlineData("¿Cómo agrego un paquete NuGet?", "paquetes-nuget")]
    [InlineData("¿Qué es un componente de Blazor?", "blazor")]
    public void Encuentra_la_leccion_relacionada(string question, string expectedSlug)
    {
        var lesson = OfflineTutorService.FindBestLesson(question);

        Assert.NotNull(lesson);
        Assert.Equal(expectedSlug, lesson.Slug);
    }

    [Theory]
    [InlineData("")]
    [InlineData("¿cuál es la diferencia?")] // "di" no debe coincidir dentro de "diferencia"
    [InlineData("receta de arepas")]
    public void No_inventa_una_leccion_si_no_hay_coincidencias(string question)
    {
        Assert.Null(OfflineTutorService.FindBestLesson(question));
    }

    [Fact]
    public async Task Responde_por_fragmentos_con_la_carpeta_de_la_leccion()
    {
        var tutor = new OfflineTutorService();
        var chunks = new List<string>();

        await foreach (var chunk in tutor.AskAsync([new ChatTurn(ChatRole.User, "herencia")]))
            chunks.Add(chunk);

        Assert.True(chunks.Count > 10);
        Assert.Contains("Demo02", string.Concat(chunks));
        Assert.False(tutor.IsOnline);
    }
}
