using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentChat.Chats;
using FluentChat.Chats.Dtos;
using FluentChat.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using OpenAI.Assistants;

namespace FluentChat.Blazor.Components.Pages;

public partial class Chat
{
    [Inject]
    public Kernel Kernel { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public IChatAppService ChatAppService { get; set; } = default!;

    [Parameter]
    public string? Id { get; set; }

    IChatCompletionService chatService = default!;
    string? question = "";
    private StringBuilder answer = new();
    private ChatHistory chatHistory = new();
    private OpenAIPromptExecutionSettings executionSettings = new() { Temperature = 0.1 };
    //private List<ChatMessageDto> chatMessages = [];
    private IReadOnlyList<ChatSessionDto> chatSessions = [];
    private Guid? _sessionId;
    private ChatSessionDto _chatSession = new();

    protected override async void OnInitialized()
    {
        base.OnInitialized();
        chatService = Kernel.GetRequiredService<IChatCompletionService>("Ollama");
        chatSessions = (
            await ChatAppService.GetPagedAsync(
                new GetSessionPagedRequestDto { MaxResultCount = 1000 }
            )
        ).Items;

        _chatSession = chatSessions.FirstOrDefault() ?? new();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            await JS.InvokeVoidAsync("scrollToBottom", "datalist");
        }
    }

    void NewChatSession()
    {
        Navigation.NavigateTo($"/chat/{Guid.NewGuid()}", false);
    }

    protected override async Task OnParametersSetAsync()
    {
        base.OnParametersSet();

        if (Guid.TryParse(Id, out Guid sessionId))
        {
            _sessionId = sessionId;
            _chatSession = await ChatAppService.SaveSessionAsync(new SaveSessionDto
            {
                Id = sessionId,
                Model = "llama3.2",
                Service = "ollama",
                Title = "新聊天",
                PromptSettings = new FluentChat.Chat.PromptSettings
                {
                    Temperature = executionSettings.Temperature
                }
            });
        }
        //else
        //{
        //    Navigation.NavigateTo("/chat", false);
        //}
    }

    void HandleInput(ChangeEventArgs e)
    {
        question = e.Value?.ToString();
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
        if (string.IsNullOrEmpty(question))
        {
            return;
        }

        var user = new ChatMessageDto { Role = AuthorRole.User.Label, Content = question };
        _chatSession.Messages.Add(user);
        question = null;
        chatHistory.AddUserMessage(user.Content);
        await ChatAppService.CreateMessageAsync(new CreateMessageDto
        {
            SessionId = _chatSession.Id,
            Role = user.Role,
            Content = user.Content
        });
        Task.Factory.StartNew(async () => await ReceiveChatContentAsync());
    }

    async Task ReceiveChatContentAsync()
    {
        var assistant = new ChatMessageDto
        {
            Role = AuthorRole.Assistant.Label,
            Content = "思考中..."
        };
        _chatSession.Messages.Add(assistant);
        answer.Clear();
        await foreach (
            var message in chatService.GetStreamingChatMessageContentsAsync(
                chatHistory,
                executionSettings,
                Kernel
            )
        )
        {
            answer.Append(message.Content);
            assistant.Content = answer.ToString();
            await InvokeAsync(StateHasChanged);
            await JS.InvokeVoidAsync("scrollToBottom", "dataList");
        }

        chatHistory.AddAssistantMessage(assistant.Content);
        await ChatAppService.CreateMessageAsync(new CreateMessageDto
        {
            SessionId = _chatSession.Id,
            Role = assistant.Role,
            Content = assistant.Content
        });
    }
}
