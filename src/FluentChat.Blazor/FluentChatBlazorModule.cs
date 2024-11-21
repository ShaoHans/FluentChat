using System;
using System.IO;
using Abp.RadzenUI;
using Abp.RadzenUI.Localization;

using FluentChat.AI;
using FluentChat.Blazor.Components.Pages;
using FluentChat.Blazor.Menus;
using FluentChat.EntityFrameworkCore;
using FluentChat.Localization;
using FluentChat.MultiTenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace FluentChat.Blazor;

[DependsOn(
    typeof(FluentChatApplicationModule),
    typeof(FluentChatEntityFrameworkCoreModule),
    typeof(FluentChatHttpApiModule),
    typeof(FluentChatAIModule),
    typeof(AbpRadzenUIModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class FluentChatBlazorModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(FluentChatResource),
                typeof(FluentChatDomainModule).Assembly,
                typeof(FluentChatDomainSharedModule).Assembly,
                typeof(FluentChatApplicationModule).Assembly,
                typeof(FluentChatApplicationContractsModule).Assembly,
                typeof(FluentChatBlazorModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("FluentChat");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate(
                    "openiddict.pfx",
                    "4b540bf3-3d0d-4f79-9467-16bec3a83617"
                );
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });
        }

        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false;
        });

        ConfigureAbpRadzenUI();
        ConfigureAuthentication(context);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureSwaggerServices(context.Services);
        ConfigureAutoApiControllers();
        //ConfigureOllama(context.Services);
    }

    private void ConfigureAbpRadzenUI()
    {
        // Configure AbpRadzenUI
        Configure<AbpRadzenUIOptions>(options =>
        {
            // this is very imporant to set current web application's pages to the AbpRadzenUI module
            options.RouterAdditionalAssemblies = [typeof(Home).Assembly];

            // other settings
            options.TitleBar = new TitleBarSettings
            {
                Title = "Fluent Chat",
                ShowGithubLink = false,
            };
            //options.LoginPage = new LoginPageSettings
            //{
            //    LogoPath = "xxx/xx.png"
            //};
            options.Theme = new ThemeSettings { EnablePremiumTheme = true, };
        });

        // Configure AbpMultiTenancyOptions, this will affect login page that whether need to switch tenants
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        // Configure AbpLocalizationOptions
        Configure<AbpLocalizationOptions>(options =>
        {
            // set AbpRadzenUIResource as BaseTypes for your application's localization resources
            var crmResource = options.Resources.Get<FluentChatResource>();
            crmResource.AddBaseTypes(typeof(AbpRadzenUIResource));

            // if you don't want to use the default language list, you can clear it and add your own languages
            options.Languages.Clear();
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
        });

        // Configure your web application's navigation menu
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new FluentChatMenuContributor());
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(
            OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
        );
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<FluentChatDomainSharedModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}FluentChat.Domain.Shared"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<FluentChatDomainModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}FluentChat.Domain"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<FluentChatApplicationContractsModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}FluentChat.Application.Contracts"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<FluentChatApplicationModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}FluentChat.Application"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<FluentChatBlazorModule>(
                    hostingEnvironment.ContentRootPath
                );
            });
        }
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "FluentChat API", Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(FluentChatApplicationModule).Assembly);
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<FluentChatBlazorModule>();
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
            app.UseHsts();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();
        app.UseAbpSecurityHeaders();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FluentChat API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        app.UseRadzenUI();
    }
}
