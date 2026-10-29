namespace Arqui1.Hub.Services.Tutor;

public enum ChatRole { User, Assistant }

public sealed record ChatTurn(ChatRole Role, string Text);

public interface ITutorService
{
    /// <summary>Nombre visible del motor que responde (por ejemplo, el modelo de Claude).</summary>
    string EngineName { get; }

    bool IsOnline { get; }

    /// <summary>Devuelve la respuesta por fragmentos para poder mostrarla mientras se genera.</summary>
    IAsyncEnumerable<string> AskAsync(IReadOnlyList<ChatTurn> conversation, CancellationToken cancellationToken = default);
}

public sealed class TutorOptions
{
    public const string Section = "Tutor";

    /// <summary>auto: usa Claude si hay credenciales; claude: siempre Claude; offline: nunca llama a la API.</summary>
    public string Mode { get; set; } = "auto";

    public string Model { get; set; } = "claude-opus-5-5";
}
