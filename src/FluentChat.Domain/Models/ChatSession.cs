using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace FluentChat.Models;

public class ChatSession : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public string Service { get; set; } = string.Empty;

    public string Model { get; set; } = default!;

    public List<ChatMessage> Messages { get; set; } = [];
}
