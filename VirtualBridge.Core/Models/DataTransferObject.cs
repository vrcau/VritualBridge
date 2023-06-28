using System.Text.Json.Serialization;
using VirtualBridge.PluginApi;

namespace VirtualBridge.Core.Models;

public record DataTransferObject : IDataTransferObject
{
    [JsonPropertyName("version")] public string Version { get; init; } = "1";
    [JsonPropertyName("timestamp")] public DateTimeOffset TimeStamp { get; init; } = DateTimeOffset.MinValue;
    [JsonPropertyName("type")] public string Type { get; init; } = "unknown";
    [JsonPropertyName("data")] public object? Data  { get; init; }
}

public record DataTransferObject<T> : DataTransferObject, IDataTransferObject<T>
{
    [JsonPropertyName("data")] public new T? Data  { get; init; }
}