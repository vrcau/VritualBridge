using System.Text.Json;
using Microsoft.Extensions.Logging;
using VirtualBridge.Core.Models;
using VirtualBridge.PluginApi;

namespace VirtualBridge.Core.Services;

public class DataTransferService : IDataTransferService
{
    public const string SupportedProtocolVersion = "1";

    private readonly ILogger<DataTransferService> _logger;
    private readonly VRChatLogWatcherService _vrchatLogWatcherService;

    private readonly List<DataTransferObject> _dataTransferObjects = new();
    public IReadOnlyList<IDataTransferObject> DataTransferObjects => _dataTransferObjects.AsReadOnly();

    public event EventHandler<IDataTransferObject>? ReceivedData;

    public DataTransferService(ILogger<DataTransferService> logger, VRChatLogWatcherService vrchatLogWatcherService)
    {
        _logger = logger;
        _vrchatLogWatcherService = vrchatLogWatcherService;

        _vrchatLogWatcherService.LogChanged += (sender, list) =>
        {
            foreach (var logInfo in list.OrderBy(log => log.time))
            {
                ProcessData(logInfo.content);
            }
        };
    }

    public void ClearAllData() => _dataTransferObjects.Clear();

    public void SendDataToGame(object content, string type)
    {
        var dto = new DataTransferObject
        {
            Version = SupportedProtocolVersion,
            Data = content,
            TimeStamp = DateTimeOffset.Now,
            Type = type
        };
        
        _dataTransferObjects.Add(dto);
        _logger.LogInformation("Send data to game: {@Dto}", dto);
    }

    private void ProcessData(string rawContent)
    {
        if (!rawContent.StartsWith("[vbdt]")) return;

        var content = rawContent["[vbdt]".Length..];
        try
        {
            var dto = JsonSerializer.Deserialize<DataTransferObject>(content);
            if (dto == null)
            {
                _logger.LogWarning("Received Invalid Data: {RawContent}", rawContent);
                return;
            }
            
            _logger.LogInformation("Received Data: {Dto}", dto);
            ReceivedData?.Invoke(this, dto);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Received Invalid Data or Fail to Process Data: {RawContent}", rawContent);
        }
    }
}