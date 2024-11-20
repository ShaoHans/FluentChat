using FluentChat.AI.Providers;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FluentChat.AI;

[DependsOn(typeof(AbpAutofacModule))]
public class FluentChatAIModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IModelProvider, OllamaProvider>();
    }
}
