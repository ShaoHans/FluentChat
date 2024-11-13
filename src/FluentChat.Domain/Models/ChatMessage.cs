using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FluentChat.Models;

public class ChatMessage : CreationAuditedEntity<Guid> 
{
    public Guid SessionId { get; set; }

    public ChatSession Session { get; set; } = default!;

    public string Role { get; set; } = default!;

    public string Content { get; set; } = default!;
}
