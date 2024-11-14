using AutoMapper;
using FluentChat.Chat;
using FluentChat.Chats.Dtos;
using FluentChat.Models;

namespace FluentChat;

public class FluentChatApplicationAutoMapperProfile : Profile
{
    public FluentChatApplicationAutoMapperProfile()
    {
        CreateMap<CreateSessionDto, ChatSession>()
            .ForMember(
                dest => dest.Title,
                opt =>
                    opt.MapFrom(src =>
                        src.Title.Substring(0, ChatConsts.ChatSession_Title_MaxLength)
                    )
            )
            .ForMember(
                dest => dest.Service,
                opt =>
                    opt.MapFrom(src =>
                        src.Service.Substring(0, ChatConsts.ChatSession_Service_MaxLength)
                    )
            )
            .ForMember(
                dest => dest.Model,
                opt =>
                    opt.MapFrom(src =>
                        src.Model.Substring(0, ChatConsts.ChatSession_Model_MaxLength)
                    )
            );
        CreateMap<CreateMessageDto, ChatMessage>()
            .ForMember(
                dest => dest.Role,
                opt =>
                    opt.MapFrom(src => src.Role.Substring(0, ChatConsts.ChatMessage_Role_MaxLength))
            );
    }
}
