using ExtensionManager.Features.Export;
using ExtensionManager.Features.Install;
using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;

using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureExtensionManager(this IServiceCollection services, IThisVsixInfo thisVsixInfo)
    {
        return services
            .AddDialogService()
            .AddManifestService()
            .AddSingleton(thisVsixInfo)
            .AddTransient<IFeatureExecutor, FeatureExecutor>()
            .AddTransient<IExtensionInstaller, ExtensionInstaller>();
    }
}
