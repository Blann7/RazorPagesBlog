using BlogPhone.Models.LogViewer.Database;
using static LogViewer.Models.LogTypes;
using Newtonsoft.Json;
using System.Text;

namespace BlogPhone.Models.LogViewer
{
    public static class LogActions
    {
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
        }
        private static void WriteLog(LogString log)
        {
            List<LogString>? deserializedJson;

            using (StreamReader sr = new StreamReader($"{Environment.CurrentDirectory}\\LogViewer_TempLog.txt", Encoding.UTF8))
            {
                string allJson = sr.ReadToEnd();
                deserializedJson = JsonConvert.DeserializeObject<List<LogString>>(allJson);
            }

            using (StreamWriter sw = new StreamWriter($"{Environment.CurrentDirectory}\\LogViewer_TempLog.txt", false, Encoding.UTF8))
            {
                if (deserializedJson is null) deserializedJson = new List<LogString>() { log }; // if log file was empty
                else deserializedJson.Add(log);

                string s = JsonConvert.SerializeObject(deserializedJson, Formatting.Indented);
                sw.WriteLine(s);
            }
        }
    }
}
