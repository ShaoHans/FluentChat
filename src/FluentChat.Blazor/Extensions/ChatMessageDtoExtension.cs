using Microsoft.SemanticKernel.ChatCompletion;

namespace FluentChat.Chats.Dtos;

public static class ChatMessageDtoExtension
{
    public static bool IsUser(this ChatMessageDto message)
    {
        return message.Role == AuthorRole.User.Label;
    }
}
