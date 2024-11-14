using System.Threading.Tasks;
using FluentChat.Chats;
using FluentChat.Jobs.Args;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace FluentChat.Jobs;

public class SaveChatMessageJob(IChatAppService chatAppService)
    : AsyncBackgroundJob<SaveChatMessageArg>,
        ITransientDependency
{
    public override async Task ExecuteAsync(SaveChatMessageArg args)
    {
        await chatAppService.CreateMessageAsync(
            new Chats.Dtos.CreateMessageDto
            {
                SessionId = args.SessionId,
                Role = args.Role,
                Content = args.Content,
            }
        );
    }
}
