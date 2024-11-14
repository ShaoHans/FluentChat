using System.Threading.Tasks;
using FluentChat.Chats.Dtos;
using Volo.Abp.Application.Services;

namespace FluentChat.Chats;

public interface IChatAppService : IApplicationService
{
    Task<ChatSessionDto> CreateSessionAsync(CreateSessionDto input);

    Task CreateMessageAsync(CreateMessageDto input);
}
