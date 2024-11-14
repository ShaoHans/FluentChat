using System;
using System.Threading.Tasks;
using FluentChat.Chats.Dtos;
using FluentChat.Models;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace FluentChat.Chats;

public class ChatAppService(
    IRepository<ChatSession> chatSessionRepository,
    IRepository<ChatMessage> chatMessageRepository
) : FluentChatAppService, IChatAppService
{
    public async Task CreateSessionAsync(CreateSessionDto input)
    {
        if (
            string.IsNullOrEmpty(input.Title)
            || string.IsNullOrEmpty(input.Model)
            || input.PromptSettings is null
        )
        {
            throw new UserFriendlyException(L[FluentChatDomainErrorCodes.RequestParamterInvalid]);
        }

        var chatSession = ObjectMapper.Map(input, new ChatSession());
        await chatSessionRepository.InsertAsync(chatSession);
    }

    public async Task CreateMessageAsync(CreateMessageDto input)
    {
        if (
            string.IsNullOrEmpty(input.Content)
            || string.IsNullOrEmpty(input.Role)
            || input.SessionId == Guid.Empty
        )
        {
            throw new UserFriendlyException(L[FluentChatDomainErrorCodes.RequestParamterInvalid]);
        }

        var chatMessage = ObjectMapper.Map(input, new ChatMessage());
        await chatMessageRepository.InsertAsync(chatMessage);
    }
}
