using NLog.Targets;

namespace Gitmanik.BaseCode.Logging
{
    public class DatabaseLogger
    {
        public static string Guid;

        public static WebServiceTarget GenerateTarget(string targetUrl)
        {
            return new WebServiceTarget()
            {
                Parameters =
                {
                    new MethodCallParameter("guid", Guid),
                    new MethodCallParameter("data", @"${callsite}:${callsite-linenumber} ${level}: ${message} ${exception}")
                },
                Protocol = WebServiceProtocol.HttpPost,
                Url = new System.Uri(targetUrl),
            };
        }
    }
}
