using FluentChat.Samples;
using Xunit;

namespace FluentChat.EntityFrameworkCore.Applications;

[Collection(FluentChatTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<FluentChatEntityFrameworkCoreTestModule>
{

}
