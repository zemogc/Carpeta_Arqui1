using System.Runtime.CompilerServices;
using Anthropic;
using Anthropic.Models.Beta.Messages;
using Microsoft.Extensions.Options;

namespace Arqui1.Hub.Services.Tutor;

/// <summary>Tutor que responde con Claude a través del SDK oficial de Anthropic, en streaming.</summary>
public sealed class ClaudeTutorService(AnthropicClient client, IOptions<TutorOptions> options) : ITutorService
{
    // Si un clasificador de seguridad rechaza la petición, la API la reintenta en el modelo recomendado.
    private const string ServerSideFallbackBeta = "server-side-fallback-2026-07-01";

    private readonly string _model = options.Value.Model;

    public string EngineName => _model;

    public bool IsOnline => true;

    public async IAsyncEnumerable<string> AskAsync(
        IReadOnlyList<ChatTurn> conversation,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = new MessageCreateParams
        {
            Model = _model,
            MaxTokens = 16000,
            Betas = [ServerSideFallbackBeta],
            Fallbacks = new Default(),
            OutputConfig = new BetaOutputConfig { Effort = Effort.Medium },
            System = new List<BetaTextBlockParam>
            {
                new() { Text = TutorPrompt.System, CacheControl = new BetaCacheControlEphemeral() },
            },
            Messages = conversation.Select(ToMessageParam).ToList(),
        };

        await foreach (var streamEvent in client.Beta.Messages.CreateStreaming(parameters, cancellationToken))
        {
            if (streamEvent.TryPickContentBlockDelta(out var blockDelta) &&
                blockDelta.Delta.TryPickText(out var text))
            {
                yield return text.Text;
            }
            else if (streamEvent.TryPickDelta(out var messageDelta) &&
                     messageDelta.Delta.StopReason == "refusal")
            {
                yield return "\n\n> No puedo ayudar con esa solicitud. Intenta reformularla en el contexto del curso.";
            }
        }
    }

    private static BetaMessageParam ToMessageParam(ChatTurn turn) => turn.Role switch
    {
        ChatRole.User => new BetaUserMessageParam { Content = turn.Text },
        _ => new BetaAssistantMessageParam { Content = turn.Text },
    };
}
