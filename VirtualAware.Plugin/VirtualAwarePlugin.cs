using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using VirtualAware.Plugin.Models;
using VirtualBridge.PluginApi;

namespace VirtualAware.Plugin;

public class VirtualAwarePlugin : IVirtualBridgePlugin
{
    public string DisplayName => "VirtualAware";
    public string Description => "VirtualAware";

    private IDataTransferService _dataTransferService;
    private ILogger _logger;

    private const string BaseUrl = "http://localhost:5035";
    private readonly HttpClient _httpClient = new();

    public Task LoadAsync(IDataTransferService dataTransferService, ILogger logger)
    {
        _dataTransferService = dataTransferService;
        _logger = logger;
        
        _dataTransferService.RegisterReceiver<VirtualAwareTrackData>("virtualaware.trackdata", (dto) =>
        {
            if (dto.Data is not {} data) return;

            _httpClient.PostAsync(BaseUrl + "/flightradar/track", JsonContent.Create(data));
        });

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }
}