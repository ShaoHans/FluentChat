using System.Threading.Tasks;
using FluentChat.Chats.Dtos;
using FluentChat.Models;
using Volo.Abp.Domain.Repositories;

namespace FluentChat.Chats;

public class ChatAppService(IRepository<ChatSession> chatSessionRepository)
    : FluentChatAppService,
        IChatAppService
{
    public async Task CreateSessionAsync(CreateSessionDto input) {

    }
}
