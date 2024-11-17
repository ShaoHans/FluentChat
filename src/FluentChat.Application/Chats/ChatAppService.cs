using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentChat.Chats.Dtos;
using FluentChat.Models;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
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

    public async Task UpdateChatSessionTitleAsync(Guid id, string title)
    {
        var chatSession = await chatSessionRepository.FindAsync(id);
        if (chatSession is null)
        {
            throw new UserFriendlyException(L[FluentChatDomainErrorCodes.DataNotExist]);
        }

        chatSession.Title = title;
        await chatSessionRepository.UpdateAsync(chatSession);
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

    public async Task<PagedResultDto<ChatSessionDto>> GetPagedAsync(GetSessionPagedRequestDto input)
    {
        var query = (await chatSessionRepository.GetQueryableAsync())
            .Where(x => x.CreatorId == CurrentUser.Id)
            .AsNoTracking();

        var count = await query.CountAsync();
        var list = await query.OrderByDescending(c => c.CreationTime).PageBy(input).ToListAsync();
        return new PagedResultDto<ChatSessionDto>(
            count,
            ObjectMapper.Map<List<ChatSession>, IReadOnlyList<ChatSessionDto>>(list)
        );
    }

    public async Task<ChatSessionDto?> GetChatSessionAsync(Guid id)
    {
        var chatSession = await (await chatSessionRepository.WithDetailsAsync(x => x.Messages))
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (chatSession is null)
        {
            return null;
        }
        return ObjectMapper.Map<ChatSession, ChatSessionDto>(chatSession);
    }
}
