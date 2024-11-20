using Microsoft.SemanticKernel;

namespace FluentChat.AI;

public static class KernelFactory
{
    public static Kernel Get(string provider, string model)
    {
        var builder = Kernel.CreateBuilder();

        switch (provider)
        {
            case "Ollama":
#pragma warning disable SKEXP0070
                builder.Services.AddOllamaChatCompletion(
                    model,
                    new Uri("http://localhost:11434"),
                    provider
                );
#pragma warning restore SKEXP0070
                break;
            default:
                break;
        }

        return builder.Build();
    }
}
