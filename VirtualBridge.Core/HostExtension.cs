using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualBridge.Core.Services;

namespace VirtualBridge.Core;

public static class HostExtension
{
    public static IHostBuilder AddVirtualBridge(this IHostBuilder builder)
    {
        builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<DataTransferService>();
            services.AddHostedService<PluginLoadService>();
            services.AddSingleton<VRChatLogWatcherService>()
                .AddHostedService<VRChatLogWatcherService>(provider =>
                    provider.GetRequiredService<VRChatLogWatcherService>());

            services.AddSingleton<HttpApiService>();
            services.AddHostedService<HttpApiService>(provider =>
                provider.GetRequiredService<HttpApiService>());
        });

        return builder;
    }
}