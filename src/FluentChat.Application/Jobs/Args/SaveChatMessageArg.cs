using System;

namespace FluentChat.Jobs.Args;

public class SaveChatMessageArg
{
    public Guid SessionId { get; set; }

    public string Role { get; set; } = default!;

    public string Content { get; set; } = default!;
}
