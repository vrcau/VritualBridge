using Microsoft.Extensions.Logging;

namespace VirtualBridge.PluginApi;

public interface IVirtualBridgePlugin
{
    public string DisplayName { get; }
    public string Description { get; }
    public Task LoadAsync(IDataTransferService dataTransferService, ILogger logger);
    public Task StopAsync();
}