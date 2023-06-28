using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using VirtualBridge.Core.Services;

var logTemplate =
    "[{@t:yyyy-MM-dd HH:mm:ss} {@l:u3}] [{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(new ExpressionTemplate(logTemplate), "logs/app-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Console(new ExpressionTemplate(logTemplate, theme: TemplateTheme.Literate))
    .WriteTo.Debug(new ExpressionTemplate(logTemplate))
    .CreateLogger();

var host = new HostBuilder()
    .ConfigureServices((_, services) =>
    {
        services.AddHostedService<VRChatLogWatcherService>();
        services.AddHostedService<PluginLoadService>();
        services.AddHostedService<HttpApiService>();
        services.AddSingleton<DataTransferService>();
    })
    .UseSerilog()
    .Build();
    
await host.RunAsync();
