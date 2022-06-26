using BlogPhone.Models.LogViewer.Database;
using static LogViewer.Models.LogTypes;

namespace BlogPhone.Models.LogViewer
{
    public static class LogActions
    {
        public static List<LogString> Logs { get; set; } = new();
        public static void SaveLog(int userId, string userName, LogType logType, string message)
        {
            LogString log = new()
            {
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                CreatedAtDateString = DateTime.UtcNow.ToShortDateString(),
                UserId = userId,
                UserName = userName,
                Type = logTypesKeyEnum.GetValueOrDefault(logType)!,
                Message = message
            };
            Logs.Add(log);
        }
    }
}
