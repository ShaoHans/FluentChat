using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentChat.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FluentChat.Blazor.Components.Pages;

public partial class Chat
{
    [Inject]
    public Kernel Kernel { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [SupplyParameterFromQuery]
    public Guid? SessionId { get; set; }

    IChatCompletionService chatService = default!;
    string? msg = "";
    private StringBuilder content = new();
    private ChatHistory chatHistory = new();
    private OpenAIPromptExecutionSettings executionSettings = new() { Temperature = 0.1 };
    private List<ChatMessageDto> chatMessages = [];

    

    protected override void OnInitialized()
    {
        base.OnInitialized();
        chatService = Kernel.GetRequiredService<IChatCompletionService>("Ollama");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            await JS.InvokeVoidAsync("scrollToBottom", "datalist");
        }
    }

    void HandleInput(ChangeEventArgs e)
    {
        msg = e.Value?.ToString();
    }

    Task HandleKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            return SendAsync();
        }
        return Task.CompletedTask;
    }

    async Task SendAsync()
    {
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }

        var user = new ChatMessageDto { IsAssistant = false, Content = msg };
        chatMessages.Add(user);
        msg = null;
        var assistant = new ChatMessageDto { IsAssistant = true, Content = "Ë¼¿¼ÖÐ..." };
        chatMessages.Add(assistant);
        chatHistory.AddUserMessage(user.Content);
        content.Clear();
        await foreach (
            var message in chatService.GetStreamingChatMessageContentsAsync(
                chatHistory,
                executionSettings,
                Kernel
            )
        )
        {
            content.Append(message.Content);
            assistant.Content = content.ToString();
            StateHasChanged();
            await JS.InvokeVoidAsync("scrollToBottom", "dataList");
        }

        chatHistory.AddAssistantMessage(content.ToString());
    }
}
