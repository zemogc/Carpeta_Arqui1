using Anthropic;
using Arqui1.Hub.Components;
using Arqui1.Hub.Services.Tutor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<TutorOptions>(builder.Configuration.GetSection(TutorOptions.Section));

// El tutor usa Claude si hay una API key (user-secrets "Anthropic:ApiKey" o la variable ANTHROPIC_API_KEY);
// si no, arranca en modo sin conexión para que el hub funcione igual.
var tutorMode = builder.Configuration[$"{TutorOptions.Section}:Mode"] ?? "auto";
var apiKey = builder.Configuration["Anthropic:ApiKey"];
var hasCredentials = !string.IsNullOrWhiteSpace(apiKey)
    || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY"))
    || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ANTHROPIC_AUTH_TOKEN"));

var useClaude = tutorMode.ToLowerInvariant() switch
{
    "claude" => true,
    "offline" => false,
    _ => hasCredentials,
};

if (useClaude)
{
    builder.Services.AddSingleton(_ => string.IsNullOrWhiteSpace(apiKey)
        ? new AnthropicClient()
        : new AnthropicClient { ApiKey = apiKey });
    builder.Services.AddSingleton<ITutorService, ClaudeTutorService>();
}
else
{
    builder.Services.AddSingleton<ITutorService, OfflineTutorService>();
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
