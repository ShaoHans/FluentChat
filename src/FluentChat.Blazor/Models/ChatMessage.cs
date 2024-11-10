using System;

namespace FluentChat.Blazor.Models;

public class ChatMessage
{
    public string Content { get; set; } = default!;
    public bool IsUser { get; set; } // true if message is from user, false if from ChatGPT
    public DateTime Timestamp { get; set; }
}
