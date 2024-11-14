using System;

namespace FluentChat.Chats.Dtos;

public class CreateMessageDto
{
    public Guid SessionId { get; set; }

    public string Role { get; set; } = default!;

    public string Content { get; set; } = default!;
}
