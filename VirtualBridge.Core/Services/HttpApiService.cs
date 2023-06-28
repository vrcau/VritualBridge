using System.Reflection;
using System.Text.Json;
using HttpServerLite;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualBridge.Core.Models;
using HttpMethod = HttpServerLite.HttpMethod;

namespace VirtualBridge.Core.Services;

public class HttpApiService : IHostedService
{
    private readonly Webserver _webserver; 
    private readonly ILogger<HttpApiService> _logger;
    private readonly DataTransferService _dataTransferService;

    public HttpApiService(ILogger<HttpApiService> logger, DataTransferService dataTransferService)
    {
        _logger = logger;
        _dataTransferService = dataTransferService;

        _webserver = new Webserver("localhost", 6547, false, null, null, MetaDataRoute);
        _webserver.Events.Logger += log => _logger.LogInformation(log);
        _webserver.Events.RequestReceived += (sender, args) =>
            _logger.LogDebug("{Client}:{Port} {Method} {Url} {@Query}",
            args.Ip, args.Port, args.Method, args.Url, args.Query);
    }

    private async Task MetaDataRoute(HttpContext context)
    {
        var content = JsonSerializer.Serialize(new
        {
            name = $"VirtualBridge v{Assembly.GetExecutingAssembly().GetName().Version}",
            protocolVersion = DataTransferService.SupportedProtocolVersion,
        });
        
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = content.Length;
        try
        {
            await context.Response.SendAsync(content);
        }
        catch
        {
            // ignored
        }
    }

    [StaticRoute(HttpMethod.GET, "/messages/pull")]
    public static async Task PullMessage(HttpContext context)
    {
        var content = JsonSerializer.Serialize(Array.Empty<DataTransferObject>());

        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = content.Length;
        try
        {
            await context.Response.SendAsync(content);
        }
        catch
        {
            // ignored
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Http Api Server started at http://{Url}:{Port}", _webserver.Settings.Hostname, _webserver.Settings.Port);
        _webserver.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _webserver.Stop();;
        _webserver.Dispose();
        return Task.CompletedTask;
    }
}