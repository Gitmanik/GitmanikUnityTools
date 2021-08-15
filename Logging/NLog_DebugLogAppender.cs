using NLog;
using NLog.Targets;

namespace Gitmanik.BaseCode.Logging
{
    [Target("DebugLog")]
    public sealed class NLog_DebugLogAppender : TargetWithLayout
    {
        public static NLog_DebugLogAppender GenerateTarget()
        {
            return new NLog_DebugLogAppender()
            {
                Layout = @"<i>${callsite}:${callsite-linenumber}</i> ${level}: ${message} ${exception}"
            };
        }
        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Level == LogLevel.Warn)
                UnityEngine.Debug.LogWarning($"{Layout.Render(logEvent)}");
            else if (logEvent.Level == LogLevel.Error || logEvent.Level == LogLevel.Fatal)
                UnityEngine.Debug.LogError($"{Layout.Render(logEvent)}");
            else
                UnityEngine.Debug.Log(Layout.Render(logEvent));
        }
    }
}