using FluentChat.Localization;
using Volo.Abp.Application.Services;

namespace FluentChat;

/* Inherit your application services from this class.
 */
public abstract class FluentChatAppService : ApplicationService
{
    protected FluentChatAppService()
    {
        LocalizationResource = typeof(FluentChatResource);
    }
}
