using Microsoft.Extensions.Localization;
using FluentChat.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace FluentChat.Blazor;

[Dependency(ReplaceServices = true)]
public class FluentChatBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<FluentChatResource> _localizer;

    public FluentChatBrandingProvider(IStringLocalizer<FluentChatResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
