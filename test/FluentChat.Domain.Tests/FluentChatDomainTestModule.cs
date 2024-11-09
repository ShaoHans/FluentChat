using Volo.Abp.Modularity;

namespace FluentChat;

[DependsOn(
    typeof(FluentChatDomainModule),
    typeof(FluentChatTestBaseModule)
)]
public class FluentChatDomainTestModule : AbpModule
{

}
