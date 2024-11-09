using Volo.Abp.Modularity;

namespace FluentChat;

public abstract class FluentChatApplicationTestBase<TStartupModule> : FluentChatTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
