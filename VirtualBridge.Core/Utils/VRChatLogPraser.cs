using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VirtualBridge.Core.Models.VRChat;

namespace VirtualBridge.Core.Utils;

public partial class VRChatLogPraser
{
    public static List<LogInfo> Parse(string log)
    {
        var matches = LogRegex().Matches(log);

        var logs = new List<LogInfo>();

        foreach (Match match in matches)
        {
            var time = DateTimeOffset.Parse(match.Groups[1].Value);
            switch (match.Groups[2].Value)
            {
                case nameof(VRChatLogLevel.Error):
                    logs.Add(new LogInfo(time, VRChatLogLevel.Error, match.Groups[3].Value));
                    break;
                case nameof(VRChatLogLevel.Log):
                    logs.Add(new LogInfo(time, VRChatLogLevel.Log, match.Groups[3].Value));
                    break;
                case nameof(VRChatLogLevel.Exception):
                    logs.Add(new LogInfo(time, VRChatLogLevel.Exception, match.Groups[3].Value));
                    break;
                case nameof(VRChatLogLevel.Warning):
                    logs.Add(new LogInfo(time, VRChatLogLevel.Warning, match.Groups[3].Value));
                    break;
                default:
                    logs.Add(new LogInfo(time, VRChatLogLevel.Unknown, match.Groups[3].Value));
                    break;
            }
        }

        return logs;
    }

    [GeneratedRegex("([0-9]{4}\\.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}) (\\S+)\\s+\\-  (.*)")]
    private static partial Regex LogRegex();
}