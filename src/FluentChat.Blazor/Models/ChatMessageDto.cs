namespace FluentChat.Blazor.Models;

public class ChatMessageDto
{
    public bool IsAssistant { get; set; }

    public string Content { get; set; } = string.Empty;
}
