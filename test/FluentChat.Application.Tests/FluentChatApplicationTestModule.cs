using Volo.Abp.Modularity;

namespace FluentChat;

[DependsOn(
    typeof(FluentChatApplicationModule),
    typeof(FluentChatDomainTestModule)
)]
public class FluentChatApplicationTestModule : AbpModule
{

}
