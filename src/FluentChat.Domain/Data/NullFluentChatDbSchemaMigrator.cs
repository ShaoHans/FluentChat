using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FluentChat.Data;

/* This is used if database provider does't define
 * IFluentChatDbSchemaMigrator implementation.
 */
public class NullFluentChatDbSchemaMigrator : IFluentChatDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
