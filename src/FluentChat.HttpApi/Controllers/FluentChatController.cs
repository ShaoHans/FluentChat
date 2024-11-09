using FluentChat.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FluentChat.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class FluentChatController : AbpControllerBase
{
    protected FluentChatController()
    {
        LocalizationResource = typeof(FluentChatResource);
    }
}
