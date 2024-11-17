using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentChat.Chats;
using FluentChat.Chats.Dtos;
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
    private List<ChatMessageDto> chatMessages = [];
    private IReadOnlyList<ChatSessionDto> chatSessions = [];
    private string curChatSessionTitle = "";
    private Guid? _sessionId;

    protected override async void OnInitialized()
    {
        base.OnInitialized();
        chatService = Kernel.GetRequiredService<IChatCompletionService>("Ollama");
        chatSessions = (
            await ChatAppService.GetPagedAsync(
                new GetSessionPagedRequestDto { MaxResultCount = 1000 }
            )
        ).Items;

        curChatSessionTitle = chatSessions.FirstOrDefault()?.Title;
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

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Guid.TryParse(Id, out Guid sessionId))
        {
            _sessionId = sessionId;
        }
        else
        {
            Navigation.NavigateTo("/chat", false);
        }
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
        chatMessages.Add(user);
        question = null;
        //var assistant = new ChatMessageDto { IsAssistant = true, Content = "思考中..." };
        //chatMessages.Add(assistant);
        chatHistory.AddUserMessage(user.Content);

        //receiveTask.Start();
        Task.Factory.StartNew(async () => await ReceiveChatContentAsync());

        //answer.Clear();
        //await foreach (
        //    var message in chatService.GetStreamingChatMessageContentsAsync(
        //        chatHistory,
        //        executionSettings,
        //        Kernel
        //    )
        //)
        //{
        //    answer.Append(message.Content);
        //    assistant.Content = answer.ToString();
        //    StateHasChanged();
        //    await JS.InvokeVoidAsync("scrollToBottom", "dataList");
        //}

        //chatHistory.AddAssistantMessage(answer.ToString());
    }

    async Task ReceiveChatContentAsync()
    {
        var assistant = new ChatMessageDto
        {
            Role = AuthorRole.Assistant.Label,
            Content = "思考中..."
        };
        chatMessages.Add(assistant);
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
    }
}
