using FluentChat.Chat;

using System;

namespace FluentChat.Chats.Dtos;

public class SaveSessionDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Service { get; set; } = string.Empty;

    public string Model { get; set; } = default!;

    public PromptSettings PromptSettings { get; set; } = new();
}
