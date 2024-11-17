using AutoMapper;
using FluentChat.Chat;
using FluentChat.Chats.Dtos;
using FluentChat.Models;

namespace FluentChat;

public class FluentChatApplicationAutoMapperProfile : Profile
{
    public FluentChatApplicationAutoMapperProfile()
    {
        CreateMap<SaveSessionDto, ChatSession>()
            .ForMember(
                dest => dest.Title,
                opt =>
                    opt.MapFrom(src =>
                        src.Title.Length > ChatConsts.ChatSession_Title_MaxLength
                            ? src.Title.Substring(0, ChatConsts.ChatSession_Title_MaxLength)
                            : src.Title
                    )
            )
            .ForMember(
                dest => dest.Service,
                opt =>
                    opt.MapFrom(src =>
                        src.Service.Length > ChatConsts.ChatSession_Service_MaxLength
                            ? src.Service.Substring(0, ChatConsts.ChatSession_Service_MaxLength)
                            : src.Service
                    )
            )
            .ForMember(
                dest => dest.Model,
                opt =>
                    opt.MapFrom(src =>
                        src.Model.Length > ChatConsts.ChatSession_Model_MaxLength
                            ? src.Model.Substring(0, ChatConsts.ChatSession_Model_MaxLength)
                            : src.Model
                    )
            );

        CreateMap<ChatSession, ChatSessionDto>();

        CreateMap<CreateMessageDto, ChatMessage>()
            .ForMember(
                dest => dest.Role,
                opt =>
                    opt.MapFrom(src =>
                        src.Role.Length > ChatConsts.ChatMessage_Role_MaxLength
                            ? src.Role.Substring(0, ChatConsts.ChatMessage_Role_MaxLength)
                            : src.Role
                    )
            );

        CreateMap<ChatMessage, ChatMessageDto>();
    }
}
