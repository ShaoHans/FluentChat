using AutoMapper;
using FluentChat.Chats.Dtos;
using FluentChat.Models;

namespace FluentChat;

public class FluentChatApplicationAutoMapperProfile : Profile
{
    public FluentChatApplicationAutoMapperProfile()
    {
        CreateMap<CreateSessionDto, ChatSession>();
        CreateMap<CreateMessageDto, ChatMessage>();
    }
}
