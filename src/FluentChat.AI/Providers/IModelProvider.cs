using Volo.Abp.DependencyInjection;

namespace FluentChat.AI.Providers;

public interface IModelProvider : ITransientDependency
{
    string Name { get; }
}
