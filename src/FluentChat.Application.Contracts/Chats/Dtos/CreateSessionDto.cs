using FluentChat.Models;

namespace FluentChat.Chats.Dtos;

public class CreateSessionDto
{
    public string Title { get; set; } = default!;

    public string Service { get; set; } = string.Empty;

    public string Model { get; set; } = default!;

    public PromptSettings PromptSettings { get; set; } = new();
}
