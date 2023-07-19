using Microsoft.Extensions.Logging;
using VirtualBridge.PluginApi;

namespace VirtualAware.Plugin;

public class VirtualAwarePlugin : IVirtualBridgePlugin
{
    public string DisplayName => "VirtualAware";
    public string Description => "VirtualAware";

    private IDataTransferService _dataTransferService;
    private ILogger _logger;

    public Task LoadAsync(IDataTransferService dataTransferService, ILogger logger)
    {
        _dataTransferService = dataTransferService;
        _logger = logger;
        
        _dataTransferService.RegisterReceiver<string>("virtualaware.trackdata", (dto) =>
        {
            if (dto.Data != null)
                _logger.LogInformation("{Data}", dto.Data);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }
}