using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentChat.Data;
using Volo.Abp.DependencyInjection;

namespace FluentChat.EntityFrameworkCore;

public class EntityFrameworkCoreFluentChatDbSchemaMigrator
    : IFluentChatDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreFluentChatDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the FluentChatDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<FluentChatDbContext>()
            .Database
            .MigrateAsync();
    }
}
