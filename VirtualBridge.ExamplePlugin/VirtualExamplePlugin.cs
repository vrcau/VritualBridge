﻿using Microsoft.Extensions.Logging;
using VirtualBridge.PluginApi;

namespace VirtualBridge.ExamplePlugin;

public class VirtualExamplePlugin : IVirtualBridgePlugin
{
    public string DisplayName => "VirtualBridge Example Plugin";
    public string Description => "a example plugin";

    private IDataTransferService _dataTransferService;
    private ILogger _logger;

    public Task LoadAsync(IDataTransferService dataTransferService, ILogger logger)
    {
        _dataTransferService = dataTransferService;
        _logger = logger;
        
        _logger.LogInformation("VirtualExamplePlugin loaded!");
        
        
        for (var i = 0; i < 10; i++)
        {
            _dataTransferService.SendDataToGame($"data: {i}", "test");
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _logger.LogInformation("VirtualExamplePlugin unload!");
        return Task.CompletedTask;
    }
}