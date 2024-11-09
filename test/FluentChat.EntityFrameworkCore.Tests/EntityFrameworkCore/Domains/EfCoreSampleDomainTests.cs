using FluentChat.Samples;
using Xunit;

namespace FluentChat.EntityFrameworkCore.Domains;

[Collection(FluentChatTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<FluentChatEntityFrameworkCoreTestModule>
{

}
