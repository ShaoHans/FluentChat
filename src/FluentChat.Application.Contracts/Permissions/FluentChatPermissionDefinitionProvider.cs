using FluentChat.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace FluentChat.Permissions;

public class FluentChatPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(FluentChatPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(FluentChatPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FluentChatResource>(name);
    }
}
