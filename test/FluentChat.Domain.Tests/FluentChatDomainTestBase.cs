using Volo.Abp.Modularity;

namespace FluentChat;

/* Inherit from this class for your domain layer tests. */
public abstract class FluentChatDomainTestBase<TStartupModule> : FluentChatTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
