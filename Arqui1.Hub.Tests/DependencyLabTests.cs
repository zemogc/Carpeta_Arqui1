using Arqui1.Hub.Services;

namespace Arqui1.Hub.Tests;

public class DependencyLabTests
{
    private static readonly WriterTarget[] AllWriters = [WriterTarget.Console, WriterTarget.Debug, WriterTarget.File];

    [Fact]
    public void Con_los_tres_writers_cada_destino_recibe_el_mensaje_una_vez()
    {
        var result = DependencyLab.Run(AllWriters, reproduceOriginalBug: false, "Demo", "Azúcar");

        Assert.Equal(3, result.ResolvedWriters);
        Assert.Single(result.For(WriterTarget.Console));
        Assert.Single(result.For(WriterTarget.Debug));
        Assert.Single(result.For(WriterTarget.File));
        Assert.StartsWith("FileWriter:", result.For(WriterTarget.File).Single().Text);
        Assert.EndsWith("Demo, Created: Azúcar", result.For(WriterTarget.Debug).Single().Text);
    }

    [Fact]
    public void El_bug_original_triplica_la_consola_y_deja_vacio_el_archivo()
    {
        var result = DependencyLab.Run(AllWriters, reproduceOriginalBug: true, "Demo", "Azúcar");

        Assert.Equal(3, result.ResolvedWriters);
        Assert.Equal(3, result.For(WriterTarget.Console).Count());
        Assert.Empty(result.For(WriterTarget.Debug));
        Assert.Empty(result.For(WriterTarget.File));
    }

    [Fact]
    public void Sin_writers_el_servicio_corre_pero_nadie_escribe()
    {
        var result = DependencyLab.Run([], reproduceOriginalBug: false, "Demo", "Azúcar");

        Assert.Equal(0, result.ResolvedWriters);
        Assert.Empty(result.Lines);
    }

    [Fact]
    public void El_codigo_generado_refleja_los_writers_elegidos()
    {
        var result = DependencyLab.Run([WriterTarget.File, WriterTarget.Console], false, "Demo", "Azúcar");

        Assert.Contains("Builder.Services.AddConsoleWriter();", result.RegistrationCode);
        Assert.Contains("Builder.Services.AddFileWriter();", result.RegistrationCode);
        Assert.DoesNotContain("AddDebugWriter", result.RegistrationCode);
    }
}
