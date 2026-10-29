using System.Text;
using NorthWind.Entities.Interfaces;
using NorthWind.Entities.ValueObjects;

namespace Arqui1.Hub.Services;

public enum WriterTarget { Console, Debug, File }

public sealed record LabLine(WriterTarget Target, string Text);

public sealed record LabResult(string RegistrationCode, int ResolvedWriters, IReadOnlyList<LabLine> Lines)
{
    public IEnumerable<LabLine> For(WriterTarget target) => Lines.Where(l => l.Target == target);
}

/// <summary>
/// Reproduce la solución NorthWind con el contenedor real de Microsoft.Extensions.DependencyInjection.
/// Los writers escriben en memoria para poder mostrar en pantalla qué salida produciría cada uno.
/// </summary>
public static class DependencyLab
{
    public static LabResult Run(IReadOnlyCollection<WriterTarget> selected, bool reproduceOriginalBug, string user, string productName)
    {
        var services = new ServiceCollection();
        var sink = new LabSink();
        services.AddSingleton(sink);

        foreach (var target in selected.Order())
        {
            // El bug original: los tres métodos Add*Writer registraban ConsoleWriter.
            var registered = reproduceOriginalBug ? WriterTarget.Console : target;
            switch (registered)
            {
                case WriterTarget.Console: services.AddSingleton<IUserActionWriter, LabConsoleWriter>(); break;
                case WriterTarget.Debug: services.AddSingleton<IUserActionWriter, LabDebugWriter>(); break;
                case WriterTarget.File: services.AddSingleton<IUserActionWriter, LabFileWriter>(); break;
            }
        }
        services.AddSingleton<LabProductService>();

        using var provider = services.BuildServiceProvider();
        var writers = provider.GetServices<IUserActionWriter>().Count();
        provider.GetRequiredService<LabProductService>().Add(user, productName);

        return new LabResult(BuildCode(selected), writers, sink.Lines);
    }

    private static string BuildCode(IReadOnlyCollection<WriterTarget> selected)
    {
        var sb = new StringBuilder("var Builder = Host.CreateApplicationBuilder();\n");
        foreach (var target in selected.Order())
            sb.AppendLine($"Builder.Services.Add{target}Writer();");
        sb.AppendLine("Builder.Services.AddSingleton<ProductService>();");
        sb.AppendLine("using var AppHost = Builder.Build();");
        return sb.ToString();
    }

    private sealed class LabSink
    {
        private readonly List<LabLine> _lines = [];
        public IReadOnlyList<LabLine> Lines => _lines;
        public void Add(WriterTarget target, string text) => _lines.Add(new(target, text));
    }

    private sealed class LabProductService(IEnumerable<IUserActionWriter> writers)
    {
        public void Add(string user, string productName)
        {
            UserAction Action = new UserAction(user, $"Created: {productName}");
            foreach (var Writer in writers)
                Writer.Write(Action);
        }
    }

    // Mismo formato que las clases de NorthWind.Writers.
    private sealed class LabConsoleWriter(LabSink sink) : IUserActionWriter
    {
        public void Write(UserAction action) =>
            sink.Add(WriterTarget.Console, $"ConsoleWriter: {action.CreatedDateTime}, {action.User}, {action.Description}");
    }

    private sealed class LabDebugWriter(LabSink sink) : IUserActionWriter
    {
        public void Write(UserAction action) =>
            sink.Add(WriterTarget.Debug, $"DebugWriter: {action.CreatedDateTime}, {action.User}, {action.Description}");
    }

    private sealed class LabFileWriter(LabSink sink) : IUserActionWriter
    {
        public void Write(UserAction action) =>
            sink.Add(WriterTarget.File, $"FileWriter: {action.CreatedDateTime}, {action.User}, {action.Description}");
    }
}
