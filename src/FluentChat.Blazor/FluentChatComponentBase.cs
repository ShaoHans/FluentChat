using FluentChat.Localization;
using Volo.Abp.AspNetCore.Components;

namespace FluentChat.Blazor;

public abstract class FluentChatComponentBase : AbpComponentBase
{
    protected FluentChatComponentBase()
    {
        LocalizationResource = typeof(FluentChatResource);
    }
}
