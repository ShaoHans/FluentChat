using System.Threading.Tasks;

namespace FluentChat.Data;

public interface IFluentChatDbSchemaMigrator
{
    Task MigrateAsync();
}
