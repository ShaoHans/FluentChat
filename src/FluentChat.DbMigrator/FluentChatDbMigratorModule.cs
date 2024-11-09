using FluentChat.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FluentChat.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(FluentChatEntityFrameworkCoreModule),
    typeof(FluentChatApplicationContractsModule)
)]
public class FluentChatDbMigratorModule : AbpModule
{
}
