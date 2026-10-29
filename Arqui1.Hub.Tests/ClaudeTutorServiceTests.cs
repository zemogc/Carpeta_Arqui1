using System.Net;
using System.Text;
using System.Text.Json;
using Anthropic;
using Arqui1.Hub.Services.Tutor;
using Microsoft.Extensions.Options;

namespace Arqui1.Hub.Tests;

public class ClaudeTutorServiceTests
{
    private const string StreamBody = """
        event: message_start
        data: {"type":"message_start","message":{"id":"msg_test","type":"message","role":"assistant","model":"claude-opus-5-5","content":[],"stop_reason":null,"stop_sequence":null,"usage":{"input_tokens":10,"output_tokens":1}}}

        event: content_block_start
        data: {"type":"content_block_start","index":0,"content_block":{"type":"text","text":""}}

        event: content_block_delta
        data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":"Hola, "}}

        event: content_block_delta
        data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":"estudiante."}}

        event: content_block_stop
        data: {"type":"content_block_stop","index":0}

        event: message_delta
        data: {"type":"message_delta","delta":{"stop_reason":"end_turn","stop_sequence":null},"usage":{"output_tokens":5}}

        event: message_stop
        data: {"type":"message_stop"}


        """;

    [Fact]
    public async Task Envia_la_conversacion_con_el_modelo_y_los_fallbacks_y_devuelve_el_texto_en_streaming()
    {
        var handler = new CapturingHandler(StreamBody);
        var client = new AnthropicClient
        {
            ApiKey = "sk-ant-test",
            HttpClient = new HttpClient(handler),
            MaxRetries = 0,
        };
        var tutor = new ClaudeTutorService(client, Options.Create(new TutorOptions()));

        var conversation = new List<ChatTurn>
        {
            new(ChatRole.User, "¿Qué es una interfaz?"),
            new(ChatRole.Assistant, "Un contrato."),
            new(ChatRole.User, "¿Y una clase abstracta?"),
        };

        var answer = new StringBuilder();
        await foreach (var chunk in tutor.AskAsync(conversation))
            answer.Append(chunk);

        Assert.Equal("Hola, estudiante.", answer.ToString());

        using var body = JsonDocument.Parse(handler.RequestBody!);
        var root = body.RootElement;
        Assert.Equal("claude-opus-5-5", root.GetProperty("model").GetString());
        Assert.True(root.GetProperty("stream").GetBoolean());
        Assert.Equal("default", root.GetProperty("fallbacks").GetString());
        Assert.Equal(3, root.GetProperty("messages").GetArrayLength());
        Assert.Contains("Carpeta_Arqui1", root.GetProperty("system")[0].GetProperty("text").GetString());
        Assert.Contains("server-side-fallback-2026-07-01", handler.BetaHeader);
    }

    private sealed class CapturingHandler(string responseBody) : HttpMessageHandler
    {
        public string? RequestBody { get; private set; }
        public string BetaHeader { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
            if (request.Headers.TryGetValues("anthropic-beta", out var values))
                BetaHeader = string.Join(",", values);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "text/event-stream"),
            };
        }
    }
}
