using System;
using System.Threading.Tasks;
using FluentChat.Chats.Dtos;
using FluentChat.Models;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace FluentChat.Chats;

public class ChatAppService(
    IRepository<ChatSession, Guid> chatSessionRepository,
    IRepository<ChatMessage, Guid> chatMessageRepository
) : FluentChatAppService, IChatAppService
{
    public async Task<ChatSessionDto> SaveSessionAsync(SaveSessionDto input)
    {
        if (
            string.IsNullOrEmpty(input.Title)
            || string.IsNullOrEmpty(input.Model)
            || input.PromptSettings is null
            || input.Id == Guid.Empty
        )
        {
            throw new UserFriendlyException(L[FluentChatDomainErrorCodes.RequestParamterInvalid]);
        }

        var chatSession = await chatSessionRepository.FindAsync(input.Id);
        if (chatSession == null)
        {
            chatSession = ObjectMapper.Map(input, new ChatSession(input.Id));
            await chatSessionRepository.InsertAsync(chatSession);
        }
        else
        {
            ObjectMapper.Map(input, chatSession);
            await chatSessionRepository.UpdateAsync(chatSession);
        }

        return ObjectMapper.Map(chatSession, new ChatSessionDto());
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
