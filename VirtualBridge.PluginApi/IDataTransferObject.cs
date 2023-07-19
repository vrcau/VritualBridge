﻿using System.Text.Json.Serialization;

namespace VirtualBridge.PluginApi;

public interface IDataTransferObject
{
    [JsonPropertyName("version")] public string Version { get; }
    [JsonPropertyName("timestamp")] public long TimeStamp { get; }
    [JsonPropertyName("type")] public string Type { get; }
    [JsonPropertyName("data")] public object? Data { get; }
}

public interface IDataTransferObject<T> : IDataTransferObject
{
    [JsonPropertyName("data")] public new T? Data { get; }
}