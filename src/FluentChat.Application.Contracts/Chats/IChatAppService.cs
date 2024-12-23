﻿using System;
using System.Threading.Tasks;
using FluentChat.Chats.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FluentChat.Chats;

public interface IChatAppService : IApplicationService
{
    Task<ChatSessionDto> SaveSessionAsync(SaveSessionDto input);

    Task UpdateChatSessionTitleAsync(Guid id, string title);

    Task CreateMessageAsync(CreateMessageDto input);

    Task<PagedResultDto<ChatSessionDto>> GetPagedAsync(GetSessionPagedRequestDto input);

    Task<ChatSessionDto?> GetChatSessionAsync(Guid id);
}
