using System.Text.Json.Serialization;

namespace VirtualAware.Plugin.Models;

public record VirtualAwareTrackData(
    [property: JsonPropertyName("callsign")] string Callsign,
    [property: JsonPropertyName("typeCode")] string TypeCode,
    [property: JsonPropertyName("registration")] string Registration,
    [property: JsonPropertyName("instanceId")] string InstanceId,
    [property: JsonPropertyName("worldId")] string WorldId,
    [property: JsonPropertyName("latitude")] float Latitude,
    [property: JsonPropertyName("longitude")] float Longitude,
    [property: JsonPropertyName("altitude")] int Altitude,
    [property: JsonPropertyName("heading")] int Heading,
    [property: JsonPropertyName("groundspeed")] int Groundspeed,
    [property: JsonPropertyName("verticalSpeed")] int VerticalSpeed,
    [property: JsonPropertyName("squawkCode")] int SquawkCode,
    [property: JsonPropertyName("track")] int Track);