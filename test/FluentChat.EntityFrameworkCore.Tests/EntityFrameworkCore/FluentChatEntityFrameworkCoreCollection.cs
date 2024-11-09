using Xunit;

namespace FluentChat.EntityFrameworkCore;

[CollectionDefinition(FluentChatTestConsts.CollectionDefinitionName)]
public class FluentChatEntityFrameworkCoreCollection : ICollectionFixture<FluentChatEntityFrameworkCoreFixture>
{

}
