using System;

namespace VirtualBridge.Core.Models.VRChat;

public record LogInfo(DateTimeOffset time, VRChatLogLevel level, string content)
{
    public override string ToString()
    {
        return $"[{time}][{level}] {content}";
    }
}

public enum VRChatLogLevel
{
    Log,
    Exception,
    Warning,
    Error,
    Unknown
}