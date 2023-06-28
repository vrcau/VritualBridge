using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualBridge.Core.Models.VRChat;
using VirtualBridge.Core.Utils;

namespace VirtualBridge.Core.Services;

public class VRChatLogWatcherService : IHostedService, IDisposable
{
    private readonly FileSystemWatcher _watcher = new();

    private readonly ILogger<VRChatLogWatcherService> _logger;

    private readonly string _vrchatDataPath = "";
    private string _vrchatLogPath = "";

    private string _lastRawLog = "";
    private List<LogInfo> _logs = new();
    public IReadOnlyList<LogInfo> Logs => _logs.AsReadOnly();

    public event EventHandler<IReadOnlyList<LogInfo>>? LogChanged; 

    public VRChatLogWatcherService(ILogger<VRChatLogWatcherService> logger)
    {
        _logger = logger;
        
        _vrchatDataPath = GetVRChatDataPath();
        _vrchatLogPath = GetVRChatLogPath(_vrchatDataPath);
        
        _watcher.Path = _vrchatDataPath;
        _watcher.Filter = "*.txt";
        _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;
        _watcher.Created += HandleFileSystemCreatedEvent;
        _watcher.IncludeSubdirectories = false;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _watcher.EnableRaisingEvents = true;
        CallParseLog();

        Task.Run(async () =>
        {
            while (true)
            {
                CallParseLog();
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }, cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    private void HandleFileSystemCreatedEvent(object sender, FileSystemEventArgs args)
    {
        _vrchatLogPath = GetVRChatLogPath(_vrchatDataPath);
        CallParseLog();
    }
    
    private async void CallParseLog()
    {
        await using var fileStream = new FileStream(_vrchatLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fileStream);
        var logRaw = await reader.ReadToEndAsync();
        
        // return if logs has not changed
        if (_lastRawLog == logRaw) return;
        _logs = VRChatLogPraser.Parse(logRaw);

        if (_lastRawLog == "")
        {
            _lastRawLog = logRaw;
            return;
        }
        
        var difference = logRaw.Replace(_lastRawLog, "");
        _lastRawLog = logRaw;
        
        var differenceLogs = VRChatLogPraser.Parse(difference);
        LogChanged?.Invoke(this, differenceLogs);
        differenceLogs.ForEach(log => _logger.LogDebug("[VRChat] {LogContent}", log));
    }

    private string GetVRChatLogPath(string vrchatDataPath)
    {
        var logFiles = Directory.GetFiles(vrchatDataPath).ToList()
            .Where(file => Path.GetFileName(file).StartsWith("output_log_"))
            .OrderBy(Path.GetFileName)
            .ToArray();

        var logPath = logFiles.Last();;
        _logger.LogInformation("Update VRChat Log Path: {VRChatLogPath}", logPath);
        return logPath;
    }

    private string GetVRChatDataPath()
    {
        var vrchatDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData", "LocalLow", "VRChat", "VRChat");
        _logger.LogInformation("Update VRChat Data Path: {VRChatDataPath}", vrchatDataPath);

        return vrchatDataPath;
    }

    public void Dispose()
    {
        _watcher.Dispose();
        GC.SuppressFinalize(this);
    }
}