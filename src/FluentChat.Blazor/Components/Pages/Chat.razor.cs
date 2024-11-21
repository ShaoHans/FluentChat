using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentChat.AI;
using FluentChat.AI.Providers;
using FluentChat.AIModels.Dtos;
using FluentChat.Chat;
using FluentChat.Chats;
using FluentChat.Chats.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Radzen;

namespace FluentChat.Blazor.Components.Pages;

public partial class Chat
{
    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public IChatAppService ChatAppService { get; set; } = default!;

    [Inject]
    public IModelProvider ModelProvider { get; set; } = default!;

    [Parameter]
    public string? Id { get; set; }

    IChatCompletionService chatService = default!;
    string? question = "";
    private StringBuilder answer = new();
    private ChatHistory chatHistory = new();
    private OpenAIPromptExecutionSettings executionSettings = new() { Temperature = 0.1 };
    private List<ChatSessionDto> chatSessions = [];
    private List<AIModelDto> _aiModels = [];
    private ChatSessionDto _chatSession = new();
    private CancellationTokenSource _cts = new();
    private bool _visible = true;
    private Kernel _kernel = default!;
    private string? _selectedModel;

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        _kernel = KernelFactory.Get(ModelProviderNames.Ollama, "llama3.2");
        chatService = _kernel.GetRequiredService<IChatCompletionService>(ModelProviderNames.Ollama);
        chatSessions =
        [
            .. (
                await ChatAppService.GetPagedAsync(
                    new GetSessionPagedRequestDto { MaxResultCount = 1000 }
                )
            ).Items,
        ];

        if (chatSessions.Count > 0 && string.IsNullOrEmpty(Id))
        {
            ChangeUrl(chatSessions.First().Id);
        }

        await LoadAIModels();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            await JS.InvokeVoidAsync("scrollToBottom", "datalist");
        }
    }

    private async Task LoadAIModels()
    {
        var ollamaModels = (await ModelProvider.GetModelsAsync()).Select(x => new AIModelDto
        {
            Provider = ModelProviderNames.Ollama,
            Name = x,
        });

        _aiModels = ollamaModels
            .GroupBy(x => x.Provider)
            .SelectMany(x =>
                new AIModelDto[] { new() { Provider = x.Key } }.Concat(
                    x.Select(i => new AIModelDto { Name = i.Name })
                )
            )
            .ToList();

        _selectedModel = ollamaModels.FirstOrDefault()?.Name;
    }

    void AIModelItemRender(DropDownItemRenderEventArgs<string?> args)
    {
        var data = (AIModelDto)args.Item;
        if (data.Provider != null)
        {
            // Use this code to prevent default item selection for group items.
            args.Disabled = true;
            args.Attributes.Add("style", "opacity: 1");
        }
        else
        {
            args.Attributes.Add("style", "margin-inline-start: 1rem");
        }
    }

    void ChangeUrl(Guid sessionId)
    {
        Navigation.NavigateTo($"/chat/{sessionId}", false);
    }

    void NewChatSession()
    {
        ChangeUrl(Guid.NewGuid());
    }

    protected override async Task OnParametersSetAsync()
    {
        base.OnParametersSet();

        if (Guid.TryParse(Id, out Guid sessionId))
        {
            var session = await ChatAppService.GetChatSessionAsync(sessionId);
            if (session is null)
            {
                _chatSession = await ChatAppService.SaveSessionAsync(
                    new SaveSessionDto
                    {
                        Id = sessionId,
                        Model = "llama3.2",
                        Service = ModelProviderNames.Ollama,
                        Title = "新聊天",
                        PromptSettings = new PromptSettings
                        {
                            Temperature = executionSettings.Temperature,
                        },
                    }
                );
                chatSessions.Insert(0, _chatSession);
            }
            else
            {
                _chatSession = session;
            }

            foreach (var message in _chatSession.Messages)
            {
                chatHistory.Add(
                    new ChatMessageContent(
                        new AuthorRole(message.Role),
                        message.Content,
                        _chatSession.Model
                    )
                );
            }
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
        if (string.IsNullOrEmpty(_selectedModel))
        {
            await Message.Warn("请选择聊天模型");
            return;
        }

        if (string.IsNullOrEmpty(question))
        {
            return;
        }

        if (_chatSession.Messages.IsNullOrEmpty())
        {
            await ChatAppService.UpdateChatSessionTitleAsync(_chatSession.Id, question);
            var session = chatSessions.FirstOrDefault(c => c.Id == _chatSession.Id);
            if (session is not null)
            {
                session.Title = question;
                StateHasChanged();
            }
        }

        var user = new ChatMessageDto { Role = AuthorRole.User.Label, Content = question };
        _chatSession.Messages.Add(user);
        chatHistory.AddUserMessage(user.Content);

        question = null;
        _visible = false;

        await ChatAppService.CreateMessageAsync(
            new CreateMessageDto
            {
                SessionId = _chatSession.Id,
                Role = user.Role,
                Content = user.Content,
            }
        );
        _ = Task.Factory.StartNew(async () => await ReceiveChatContentAsync());
    }

    async Task ReceiveChatContentAsync()
    {
        var assistant = new ChatMessageDto
        {
            Role = AuthorRole.Assistant.Label,
            Content = "思考中...",
        };
        _chatSession.Messages.Add(assistant);
        answer.Clear();
        await InvokeAsync(StateHasChanged);

        await foreach (
            var message in chatService.GetStreamingChatMessageContentsAsync(
                chatHistory,
                executionSettings,
                _kernel,
                _cts.Token
            )
        )
        {
            if (_cts.IsCancellationRequested)
            {
                Logger.LogInformation("user cancel AI generation");
                break;
            }

            answer.Append(message.Content);
            assistant.Content = answer.ToString();
            await InvokeAsync(StateHasChanged);
            await JS.InvokeVoidAsync("scrollToBottom", "dataList");
        }

        _visible = true;
        await InvokeAsync(StateHasChanged);

        chatHistory.AddAssistantMessage(assistant.Content);
        await ChatAppService.CreateMessageAsync(
            new CreateMessageDto
            {
                SessionId = _chatSession.Id,
                Role = assistant.Role,
                Content = assistant.Content,
            }
        );
    }

    async Task StopAsync()
    {
        await _cts.CancelAsync();
        _visible = true;
    }
}
