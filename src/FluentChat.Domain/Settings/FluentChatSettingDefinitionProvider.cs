using Volo.Abp.Settings;

namespace FluentChat.Settings;

public class FluentChatSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(FluentChatSettings.MySetting1));
    }
}
