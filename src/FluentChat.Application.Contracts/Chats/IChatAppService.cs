using System.Threading.Tasks;
using FluentChat.Chats.Dtos;
using Volo.Abp.Application.Services;

namespace FluentChat.Chats;

public interface IChatAppService : IApplicationService
{
    Task<ChatSessionDto> SaveSessionAsync(SaveSessionDto input);

    Task CreateMessageAsync(CreateMessageDto input);
}
