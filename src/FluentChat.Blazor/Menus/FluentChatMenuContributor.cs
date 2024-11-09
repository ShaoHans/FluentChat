using System.Threading.Tasks;
using FluentChat.Localization;
using Volo.Abp.UI.Navigation;
using static Abp.RadzenUI.Menus.RadzenUI;

namespace FluentChat.Blazor.Menus;

public class FluentChatMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<FluentChatResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                FluentChatMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "home",
                order: 1
            )
        );

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 100;

        //if (MultiTenancyConsts.IsEnabled)
        //{
        //    administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        //}
        //else
        //{
        //    administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        //}

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        //administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);
        //administration.SetSubItemOrder(AuditLoggingMenuNames.Default, 3);

        return Task.CompletedTask;
    }
}
