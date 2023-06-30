using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualBridge.PluginApi;

namespace VirtualBridge.Core.Services;

public class PluginLoadService : IHostedService
{
    private readonly ILogger<PluginLoadService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly DataTransferService _dataTransferService;

    private readonly List<IVirtualBridgePlugin> _plugins = new();

    public PluginLoadService(ILogger<PluginLoadService> logger,
        DataTransferService dataTransferService,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _dataTransferService = dataTransferService;
        _loggerFactory = loggerFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Directory.Exists("plugins")) Directory.CreateDirectory("plugins");

            foreach (var pluginFile in Directory.GetFiles("plugins")
                         .Where(file => file.EndsWith(".dll"))
                         .Select(Path.GetFullPath))
            {
                _logger.LogDebug("Load Plugin Assembly: {FileName}", pluginFile);
                try
                {
                    var pluginAssembly = Assembly.LoadFile(pluginFile);
                    var pluginTypes = pluginAssembly.GetTypes()
                        .Where(type => typeof(IVirtualBridgePlugin).IsAssignableFrom(type));
                    
                    foreach (var pluginType in pluginTypes)
                    {
                        try
                        {
                            if (Activator.CreateInstance(pluginType) is not IVirtualBridgePlugin plugin) continue;
                            try
                            {
                                await plugin.LoadAsync(_dataTransferService, _loggerFactory.CreateLogger(pluginType.Name));
                                _plugins.Add(plugin);
                                _logger.LogInformation("Plugin Loaded: {DisplayName} - {Description}",
                                    plugin.DisplayName, plugin.Description);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Failed to load plugin: {TypeName}", pluginType.FullName);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to create plugin instance: {TypeName} File: {File}",
                                pluginType.FullName, pluginFile);
                        }
                    }
                    
                    _logger.LogInformation("# Successfully load {PluginCount} plugins:", _plugins.Count);
                    _plugins.ForEach(plugin => _logger.LogInformation("{DisplayName} - {Description}",
                        plugin.DisplayName, plugin.Description));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to load plugin assembly: {File}", pluginFile);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load plugins");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var plugin in _plugins)
        {
            _logger.LogInformation("Stop Plugin: {Plugin}", plugin.DisplayName);
            await plugin.StopAsync();
        }
    }
}