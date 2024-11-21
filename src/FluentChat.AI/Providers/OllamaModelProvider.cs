using FluentChat.Chat;

using OllamaSharp;

namespace FluentChat.AI.Providers;

public class OllamaModelProvider : IModelProvider
{
    public string Name => ModelProviderNames.Ollama;

    private readonly OllamaApiClient _client;

    public OllamaModelProvider()
    {
        _client = new OllamaApiClient(new Uri("http://localhost:11434"));
    }

    public async Task<IReadOnlyList<string>> GetModelsAsync()
    {
        var models = await _client.ListLocalModelsAsync();
        return models.Select(m => m.Name).ToList();
    }
}
