using Arqui1.Hub.Data;
using Arqui1.Hub.Services.Tutor;
using MyLibrary;

namespace Arqui1.Hub.Tests;

public class LessonCatalogTests
{
    [Fact]
    public void Los_slugs_son_unicos()
    {
        var slugs = LessonCatalog.All.Select(l => l.Slug).ToList();
        Assert.Equal(slugs.Count, slugs.Distinct().Count());
    }

    [Fact]
    public void Cada_leccion_apunta_a_una_carpeta_que_existe_en_el_repositorio()
    {
        var root = FindRepositoryRoot();
        foreach (var lesson in LessonCatalog.All)
        {
            foreach (var folder in lesson.Folder.Split('·', StringSplitOptions.TrimEntries))
                Assert.True(Directory.Exists(Path.Combine(root, folder)), $"No existe la carpeta {folder}");
        }
    }

    [Fact]
    public void El_prompt_del_tutor_incluye_todas_las_lecciones()
    {
        foreach (var lesson in LessonCatalog.All)
            Assert.Contains(lesson.Title, TutorPrompt.System);
    }

    [Fact]
    public void FiggleWriter_de_Demo01_se_puede_reutilizar_fuera_de_la_consola()
    {
        var art = new FiggleWriter().Render("Hi", Figgle.FiggleFonts.Standard);

        Assert.Contains("\n", art);
        Assert.True(art.Length > 20);
    }

    private static string FindRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Arqui1.slnx")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("No se encontró Arqui1.slnx");
    }
}
