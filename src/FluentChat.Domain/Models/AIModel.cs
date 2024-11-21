using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace FluentChat.Models;

public class AIModel : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public string Provider { get; set; } = default!;

    public string Name { get; set; } = default!;
}
