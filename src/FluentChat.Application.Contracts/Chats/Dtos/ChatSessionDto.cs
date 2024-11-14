using System;
using System.Collections.Generic;
using FluentChat.Chat;
using Volo.Abp.Application.Dtos;

namespace FluentChat.Chats.Dtos;

public class ChatSessionDto : FullAuditedEntityDto<Guid>
{
    public Guid? TenantId { get; set; }

    public string Title { get; set; } = default!;

    public string Service { get; set; } = string.Empty;

    public string Model { get; set; } = default!;

    public PromptSettings PromptSettings { get; set; } = new();

    public List<ChatMessageDto> Messages { get; set; } = [];
}

public class ChatMessageDto : CreationAuditedEntityDto<Guid>
{
    public Guid SessionId { get; set; }

    public string Role { get; set; } = default!;

    public string Content { get; set; } = default!;
}
