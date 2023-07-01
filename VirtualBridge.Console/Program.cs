using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using VirtualBridge.Core;

var logTemplate =
    "[{@t:yyyy-MM-dd HH:mm:ss} {@l:u3}] [{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}";

var builder = Host.CreateDefaultBuilder(args)
#if DEBUG
    .UseEnvironment("Development")
#endif
    .AddVirtualBridge()
    .UseSerilog()
    .ConfigureServices((context, _) =>
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration)
            .WriteTo.File(new ExpressionTemplate(logTemplate), "logs/app-.log", rollingInterval: RollingInterval.Day)
            .WriteTo.Console(new ExpressionTemplate(logTemplate, theme: TemplateTheme.Literate))
            .WriteTo.Debug(new ExpressionTemplate(logTemplate))
            .CreateLogger();
    });

await builder.Build().RunAsync();
