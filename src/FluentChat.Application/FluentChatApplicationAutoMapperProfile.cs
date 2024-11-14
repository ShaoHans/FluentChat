using AutoMapper;
using FluentChat.Chats.Dtos;

namespace FluentChat;

public class FluentChatApplicationAutoMapperProfile : Profile
{
    public FluentChatApplicationAutoMapperProfile()
    {
        CreateMap<CreateSessionDto, CreateSessionDto>();
    }
}
